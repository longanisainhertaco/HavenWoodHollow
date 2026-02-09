using UnityEngine;

namespace HavenwoodHollow.Visual
{
    /// <summary>
    /// Emits footprint particle effects for invisible enemies.
    /// Footprints are spawned at timed intervals while the entity is moving,
    /// providing players with a visual cue even when the sprite is disabled.
    /// Decoupled from the sprite renderer so particles appear regardless of visibility.
    /// Reference: Plan Section 5.2 - Particle Interaction: Invisible enemies emit footprint particles.
    /// </summary>
    public class FootprintEmitter : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Particle Settings")]
        [Tooltip("ParticleSystem used to emit footprint bursts")]
        [SerializeField] private ParticleSystem footprintParticles;

        [Tooltip("Time in seconds between footprint emissions")]
        [SerializeField] private float emitInterval = 0.5f;

        [Tooltip("How long each footprint particle remains visible")]
        [SerializeField] private float footprintLifetime = 3f;

        #endregion

        #region Private Fields

        private float emitTimer;
        private bool isMoving;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (footprintParticles != null)
            {
                var main = footprintParticles.main;
                main.startLifetime = footprintLifetime;
            }
        }

        /// <summary>
        /// Counts down the emit timer and spawns a footprint burst when the entity is moving.
        /// </summary>
        private void Update()
        {
            if (!isMoving || footprintParticles == null) return;

            emitTimer -= Time.deltaTime;

            if (emitTimer <= 0f)
            {
                emitTimer = emitInterval;
                footprintParticles.transform.position = transform.position;
                footprintParticles.Emit(1);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets whether the entity is currently moving.
        /// Footprints are only emitted while moving.
        /// </summary>
        /// <param name="moving">True if the entity is moving.</param>
        public void SetMoving(bool moving)
        {
            isMoving = moving;

            if (moving)
            {
                emitTimer = 0f;
            }
        }

        #endregion
    }
}
