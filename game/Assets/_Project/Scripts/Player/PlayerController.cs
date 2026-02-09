using UnityEngine;
using UnityEngine.InputSystem;

namespace HavenwoodHollow.Player
{
    /// <summary>
    /// Player locomotion controller using Kinematic Rigidbody2D.
    /// Replicates snappy, deterministic movement with no inertia.
    /// Reference: Plan Section 3.1.2 - Unity 6 Clean Implementation.
    ///
    /// Component Configuration:
    ///   - Rigidbody2D: Body Type = Kinematic
    ///   - Collision Detection: Continuous
    ///   - Interpolation: Enabled
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 2.0f;
        [SerializeField] private float runSpeed = 5.0f;

        [Header("Buff Modifiers")]
        [Tooltip("Additive speed from Coffee buff")]
        [SerializeField] private float coffeeBonus = 1.0f;
        [Tooltip("Additive speed from Horse mount")]
        [SerializeField] private float horseBonus = 1.6f;
        [Tooltip("Additive speed on path tiles")]
        [SerializeField] private float pathBonus = 0.1f;
        [Tooltip("Speed penalty for grass/crop tiles")]
        [SerializeField] private float terrainPenalty = 1.0f;

        #endregion

        #region Private Fields

        private const float MovementThreshold = 0.01f;
        private Rigidbody2D rb;
        private Vector2 moveInput;
        private bool isRunning;

        // Active buff flags
        private bool hasCoffeeBuff;
        private bool isOnHorse;
        private bool isOnPath;
        private bool isInTerrain;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        /// <summary>
        /// Movement uses FixedUpdate for deterministic physics.
        /// MovePosition sweeps for collisions along the path,
        /// replicating the integer-collision logic of the reference.
        /// </summary>
        private void FixedUpdate()
        {
            // 1. Input Processing
            // Normalize to prevent diagonal speed boost (Pythagorean theorem)
            Vector2 moveDirection = moveInput.normalized;

            if (moveDirection.sqrMagnitude < MovementThreshold) return;

            // 2. Speed Calculation
            float baseSpeed = isRunning ? runSpeed : walkSpeed;
            float modifiers = CalculateBuffs();
            float activeSpeed = baseSpeed + modifiers;

            // 3. Application via RigidBody API
            // MovePosition teleports the RB to the new spot but sweeps for collisions
            Vector2 targetPosition = rb.position + (moveDirection * activeSpeed * Time.fixedDeltaTime);
            rb.MovePosition(targetPosition);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called by the Input System to set movement direction.
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Called by the Input System to toggle running.
        /// </summary>
        public void OnRun(InputAction.CallbackContext context)
        {
            isRunning = context.performed;
        }

        public void SetCoffeeBuff(bool active) => hasCoffeeBuff = active;
        public void SetHorseMount(bool mounted) => isOnHorse = mounted;
        public void SetOnPath(bool onPath) => isOnPath = onPath;
        public void SetInTerrain(bool inTerrain) => isInTerrain = inTerrain;

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates additive speed modifiers from buffs and terrain.
        /// Coffee: +1 Speed, Horse: +1.6 Speed, Pathing: +0.1 Speed.
        /// Terrain Penalty: -1 Speed for grass/crops (unless running).
        /// </summary>
        private float CalculateBuffs()
        {
            float modifier = 0f;

            if (hasCoffeeBuff) modifier += coffeeBonus;
            if (isOnHorse) modifier += horseBonus;
            if (isOnPath) modifier += pathBonus;
            if (isInTerrain && !isRunning) modifier -= terrainPenalty;

            return modifier;
        }

        #endregion
    }
}
