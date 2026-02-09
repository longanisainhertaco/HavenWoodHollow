using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HavenwoodHollow.Creatures
{
    /// <summary>
    /// Runtime creature component handling movement, health, and trait effects.
    /// Uses Kinematic Rigidbody2D for deterministic movement (same pattern as PlayerController).
    /// Implements IDamageable for combat integration.
    /// Reference: Plan Sections 6.1-6.2.
    ///
    /// Component Configuration:
    ///   - Rigidbody2D: Body Type = Kinematic
    ///   - Collision Detection: Continuous
    ///   - Interpolation: Enabled
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class CreatureController : MonoBehaviour, Inventory.IDamageable
    {
        #region Serialized Fields

        [Header("Creature Data")]
        [SerializeField] private CreatureData creatureData;

        [Header("Wander Behavior")]
        [Tooltip("Maximum distance from spawn point the creature will wander")]
        [SerializeField] private float wanderRadius = 3f;
        [Tooltip("Time in seconds between direction changes")]
        [SerializeField] private float wanderInterval = 3f;

        #endregion

        #region Private Fields

        private CreatureGenome genome;
        private float currentHealth;
        private Rigidbody2D rb;

        // Wander state
        private Vector2 spawnPosition;
        private Vector2 wanderDirection;
        private float wanderTimer;

        // Initialization flag
        private bool isInitialized;

        // Trait-applied state
        private float defenseBonus;
        private float speedBonus;
        private Light2D bioluminescentLight;

        #endregion

        #region Events

        /// <summary>Fired when this creature's health reaches zero.</summary>
        public event Action<CreatureController> OnCreatureDied;

        #endregion

        #region Properties

        /// <summary>Species data asset for this creature.</summary>
        public CreatureData CreatureData => creatureData;

        /// <summary>This creature's genome instance.</summary>
        public CreatureGenome Genome => genome;

        /// <summary>Current health value.</summary>
        public float CurrentHealth => currentHealth;

        /// <summary>Maximum health derived from base stats.</summary>
        public float MaxHealth => creatureData != null ? creatureData.BaseHealth : 0f;

        /// <summary>Whether this creature is still alive.</summary>
        public bool IsAlive => currentHealth > 0f;

        /// <summary>Currently expressed genetic traits.</summary>
        public GeneticTrait ExpressedTraits => genome.GetExpressedTraits();

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void Start()
        {
            spawnPosition = rb.position;

            // Auto-initialize from inspector-assigned data if not yet initialized
            if (creatureData != null && !isInitialized)
            {
                Initialize(creatureData, creatureData.DefaultGenome);
            }
        }

        /// <summary>
        /// Wander movement uses FixedUpdate for deterministic physics.
        /// MovePosition sweeps for collisions along the path.
        /// </summary>
        private void FixedUpdate()
        {
            if (!IsAlive) return;

            UpdateWander();

            float activeSpeed = creatureData.MoveSpeed + speedBonus;
            Vector2 targetPosition = rb.position + (wanderDirection * activeSpeed * Time.fixedDeltaTime);

            // Clamp within wander radius
            if (Vector2.Distance(targetPosition, spawnPosition) > wanderRadius)
            {
                wanderDirection = (spawnPosition - rb.position).normalized;
            }

            rb.MovePosition(rb.position + (wanderDirection * activeSpeed * Time.fixedDeltaTime));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the creature with species data and a specific genome.
        /// Applies trait effects (speed bonus, defense, bioluminescence light).
        /// </summary>
        public void Initialize(CreatureData data, CreatureGenome creatureGenome)
        {
            creatureData = data;
            genome = creatureGenome;
            currentHealth = data.BaseHealth;
            isInitialized = true;

            ApplyTraitEffects();
        }

        /// <summary>
        /// Applies damage to the creature, reduced by defense.
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;

            float effectiveDefense = creatureData.BaseDefense + defenseBonus;
            float effectiveDamage = Mathf.Max(damage - effectiveDefense, 0f);
            currentHealth = Mathf.Max(currentHealth - effectiveDamage, 0f);

            if (!IsAlive)
            {
                OnDestroyed();
            }
        }

        /// <summary>
        /// Called when health reaches zero. Fires death event and disables the creature.
        /// </summary>
        public void OnDestroyed()
        {
            OnCreatureDied?.Invoke(this);
            gameObject.SetActive(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Picks a new random wander direction at each interval.
        /// </summary>
        private void UpdateWander()
        {
            wanderTimer -= Time.fixedDeltaTime;
            if (wanderTimer <= 0f)
            {
                wanderTimer = wanderInterval;
                float angle = UnityEngine.Random.Range(0f, 360f);
                wanderDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),
                                              Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            }
        }

        /// <summary>
        /// Applies gameplay effects based on expressed genetic traits.
        /// - Wings: slight speed bonus.
        /// - HardenedScale: +5 defense bonus.
        /// - Bioluminescence: spawns a Light2D child object.
        /// </summary>
        private void ApplyTraitEffects()
        {
            GeneticTrait expressed = genome.GetExpressedTraits();

            // Wings: slight speed bonus
            if ((expressed & GeneticTrait.Wings) != 0)
            {
                speedBonus += 0.5f;
            }

            // HardenedScale: +5 defense
            if ((expressed & GeneticTrait.HardenedScale) != 0)
            {
                defenseBonus += 5f;
            }

            // Bioluminescence: emit light via Light2D child
            if ((expressed & GeneticTrait.Bioluminescence) != 0)
            {
                SpawnBioluminescentLight();
            }
        }

        /// <summary>
        /// Creates a Light2D child object for bioluminescent creatures.
        /// </summary>
        private void SpawnBioluminescentLight()
        {
            if (bioluminescentLight != null) return;

            var lightObj = new GameObject("BioluminescentLight");
            lightObj.transform.SetParent(transform, false);

            bioluminescentLight = lightObj.AddComponent<Light2D>();
            bioluminescentLight.lightType = Light2D.LightType.Point;
            bioluminescentLight.intensity = 0.8f;
            bioluminescentLight.pointLightOuterRadius = 2.5f;
            bioluminescentLight.color = new Color(0.5f, 1f, 0.8f); // Pale green glow
        }

        #endregion
    }
}
