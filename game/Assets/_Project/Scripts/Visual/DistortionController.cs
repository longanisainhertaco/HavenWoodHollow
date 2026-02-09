using UnityEngine;

namespace HavenwoodHollow.Visual
{
    /// <summary>
    /// Controls a "Predator"-style shimmer/distortion effect on a <see cref="SpriteRenderer"/>.
    /// Used by invisible enemies (Phantasm) to give players a subtle visual cue.
    /// Animates noise offset over time and uses a <see cref="MaterialPropertyBlock"/>
    /// to avoid modifying the shared material instance.
    /// Reference: Plan Section 5.2 - Distortion Shader for "Phantasm" enemies.
    /// </summary>
    public class DistortionController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Distortion Material")]
        [Tooltip("Material with _DistortionStrength, _NoiseScale, and _NoiseSpeed shader properties")]
        [SerializeField] private Material distortionMaterial;

        [Header("Distortion Settings")]
        [Tooltip("Intensity of the distortion displacement")]
        [SerializeField] private float distortionStrength = 0.02f;

        [Tooltip("Scale of the noise pattern used for distortion")]
        [SerializeField] private float noiseScale = 10f;

        [Tooltip("Speed at which the noise pattern scrolls")]
        [SerializeField] private float noiseSpeed = 1f;

        [Header("Target")]
        [Tooltip("The SpriteRenderer this distortion effect is applied to")]
        [SerializeField] private SpriteRenderer targetRenderer;

        #endregion

        #region Private Fields

        private MaterialPropertyBlock propertyBlock;
        private bool isActive;
        private float noiseOffset;

        private static readonly int DistortionStrengthId = Shader.PropertyToID("_DistortionStrength");
        private static readonly int NoiseScaleId = Shader.PropertyToID("_NoiseScale");
        private static readonly int NoiseSpeedId = Shader.PropertyToID("_NoiseSpeed");
        private static readonly int NoiseOffsetId = Shader.PropertyToID("_NoiseOffset");

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();

            if (targetRenderer != null && distortionMaterial != null)
            {
                targetRenderer.material = distortionMaterial;
            }
        }

        /// <summary>
        /// Animates the noise offset over time to produce the shimmer effect.
        /// Only the time-varying offset is updated per frame; static properties
        /// are applied in <see cref="ApplyStaticProperties"/>.
        /// </summary>
        private void Update()
        {
            if (!isActive || targetRenderer == null) return;

            noiseOffset += noiseSpeed * Time.deltaTime;

            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(NoiseOffsetId, noiseOffset);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enables or disables the distortion shimmer effect.
        /// When disabled, the distortion strength is set to zero.
        /// When enabled, static shader properties are applied immediately.
        /// </summary>
        /// <param name="active">True to enable the distortion effect.</param>
        public void SetDistortionActive(bool active)
        {
            isActive = active;

            if (targetRenderer == null) return;

            targetRenderer.GetPropertyBlock(propertyBlock);

            if (active)
            {
                ApplyStaticProperties();
            }
            else
            {
                propertyBlock.SetFloat(DistortionStrengthId, 0f);
                targetRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        /// <summary>
        /// Adjusts the distortion strength at runtime and re-applies static properties.
        /// </summary>
        /// <param name="strength">New distortion strength value.</param>
        public void SetDistortionStrength(float strength)
        {
            distortionStrength = strength;

            if (isActive && targetRenderer != null)
            {
                targetRenderer.GetPropertyBlock(propertyBlock);
                ApplyStaticProperties();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies non-animated shader properties to the <see cref="MaterialPropertyBlock"/>.
        /// Called once when the effect is activated or settings change, not every frame.
        /// </summary>
        private void ApplyStaticProperties()
        {
            propertyBlock.SetFloat(DistortionStrengthId, distortionStrength);
            propertyBlock.SetFloat(NoiseScaleId, noiseScale);
            propertyBlock.SetFloat(NoiseSpeedId, noiseSpeed);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }

        #endregion
    }
}
