using UnityEngine;
using UnityEngine.U2D.Animation;

namespace HavenwoodHollow.Creatures
{
    /// <summary>
    /// Reads expressed traits from a CreatureGenome and applies visual changes.
    /// Enables/disables overlays, tints sprites, and adds glow effects
    /// based on the creature's phenotype.
    /// Reference: Plan Section 6.2.4 - The CreatureVisualizer script reads expressed traits
    /// and swaps sprite parts.
    /// </summary>
    public class CreatureVisualizer : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Sprite References")]
        [SerializeField] private SpriteRenderer mainRenderer;
        [SerializeField] private SpriteRenderer wingsOverlay;
        [SerializeField] private SpriteRenderer scaleOverlay;

        [Header("Sprite Resolver")]
        [Tooltip("Optional SpriteResolver for body part swapping")]
        [SerializeField] private SpriteResolver spriteResolver;

        [Header("Glow Settings")]
        [Tooltip("Color used for bioluminescent emission glow")]
        [SerializeField] private Color bioluminescentGlow = new Color(0.5f, 1f, 0.8f, 1f);
        [Tooltip("Color used for NightVision eye glow")]
        [SerializeField] private Color nightVisionGlow = new Color(1f, 0.9f, 0.3f, 1f);

        #endregion

        #region Private Fields

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private MaterialPropertyBlock propertyBlock;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates all visual elements based on the creature's expressed traits.
        /// Call this after initialization or when the genome changes.
        /// </summary>
        public void UpdateVisuals(CreatureGenome genome)
        {
            GeneticTrait expressed = genome.GetExpressedTraits();

            // Reset overlays
            if (wingsOverlay != null) wingsOverlay.enabled = false;
            if (scaleOverlay != null) scaleOverlay.enabled = false;

            // Wings: enable wing overlay
            if ((expressed & GeneticTrait.Wings) != 0 && wingsOverlay != null)
            {
                wingsOverlay.enabled = true;
            }

            // HardenedScale: enable scale overlay with green tint
            if ((expressed & GeneticTrait.HardenedScale) != 0 && scaleOverlay != null)
            {
                scaleOverlay.enabled = true;
                scaleOverlay.color = Color.green;
            }

            // Bioluminescence: add emission glow effect
            if ((expressed & GeneticTrait.Bioluminescence) != 0 && mainRenderer != null)
            {
                mainRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(EmissionColor, bioluminescentGlow);
                mainRenderer.SetPropertyBlock(propertyBlock);
            }

            // Venomous: tint purple
            if ((expressed & GeneticTrait.Venomous) != 0 && mainRenderer != null)
            {
                mainRenderer.color = new Color(0.7f, 0.3f, 1f, mainRenderer.color.a);
            }

            // ShadowForm: set alpha to 0.3
            if ((expressed & GeneticTrait.ShadowForm) != 0 && mainRenderer != null)
            {
                Color c = mainRenderer.color;
                mainRenderer.color = new Color(c.r, c.g, c.b, 0.3f);
            }

            // NightVision: subtle eye glow via emission
            if ((expressed & GeneticTrait.NightVision) != 0 && mainRenderer != null)
            {
                mainRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(EmissionColor, nightVisionGlow * 0.4f);
                mainRenderer.SetPropertyBlock(propertyBlock);
            }

            ApplyTraitColor(expressed);
        }

        /// <summary>
        /// Applies composite trait-based color adjustments to the main renderer.
        /// Called at the end of UpdateVisuals to layer final color tweaks.
        /// </summary>
        public void ApplyTraitColor(GeneticTrait traits)
        {
            if (mainRenderer == null) return;

            // ShadowForm creatures get a dark tint on top of their alpha
            if ((traits & GeneticTrait.ShadowForm) != 0)
            {
                Color c = mainRenderer.color;
                mainRenderer.color = new Color(c.r * 0.4f, c.g * 0.4f, c.b * 0.5f, c.a);
            }
        }

        #endregion
    }
}
