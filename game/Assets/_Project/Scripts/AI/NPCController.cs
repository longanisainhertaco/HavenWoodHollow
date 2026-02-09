using UnityEngine;
using HavenwoodHollow.AI.GOAP;

namespace HavenwoodHollow.AI
{
    /// <summary>
    /// NPC locomotion controller using Kinematic Rigidbody2D.
    /// Provides movement API for GOAP actions and other AI systems.
    /// Reference: Plan Section 4 - Goal-Oriented Action Planning.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class NPCController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 2f;

        [Header("NPC Identity")]
        [Tooltip("Display name for this NPC")]
        [SerializeField] private string npcName;

        #endregion

        #region Private Fields

        private const float ArrivalThreshold = 0.1f;

        private Rigidbody2D rb;
        private GOAPAgent goapAgent;

        private Vector2 targetPosition;
        private bool isMoving;
        private bool hasReachedTarget;

        #endregion

        #region Properties

        /// <summary>Whether the NPC is currently moving towards a target.</summary>
        public bool IsMoving => isMoving;

        /// <summary>Whether the NPC has reached its current target position.</summary>
        public bool HasReachedTarget => hasReachedTarget;

        /// <summary>Display name for this NPC.</summary>
        public string NPCName => npcName;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            goapAgent = GetComponent<GOAPAgent>();
        }

        /// <summary>
        /// Movement uses FixedUpdate for deterministic physics.
        /// MovePosition sweeps for collisions along the path,
        /// replicating the kinematic style of PlayerController.
        /// </summary>
        private void FixedUpdate()
        {
            if (!isMoving) return;

            Vector2 currentPosition = rb.position;
            Vector2 direction = (targetPosition - currentPosition).normalized;
            float distance = Vector2.Distance(currentPosition, targetPosition);

            if (distance <= ArrivalThreshold)
            {
                isMoving = false;
                hasReachedTarget = true;
                return;
            }

            Vector2 newPosition = currentPosition + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets a target position for the NPC to move towards.
        /// </summary>
        public void MoveTo(Vector2 position)
        {
            targetPosition = position;
            isMoving = true;
            hasReachedTarget = false;
        }

        /// <summary>
        /// Stops the NPC immediately.
        /// </summary>
        public void Stop()
        {
            isMoving = false;
        }

        /// <summary>
        /// Initializes the GOAPAgent with a shared global world state.
        /// Call this during scene setup to wire GOAP planning.
        /// </summary>
        public void SetupGOAP(WorldState globalState)
        {
            if (goapAgent == null)
            {
                goapAgent = GetComponent<GOAPAgent>();
            }

            if (goapAgent != null)
            {
                goapAgent.SetGlobalState(globalState);
                Debug.Log($"[NPCController] GOAP initialized for {npcName}");
            }
            else
            {
                Debug.LogWarning($"[NPCController] No GOAPAgent found on {npcName}");
            }
        }

        #endregion
    }
}
