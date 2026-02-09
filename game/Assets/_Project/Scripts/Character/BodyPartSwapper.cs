using System;
using System.Collections.Generic;
using UnityEngine;

namespace HavenwoodHollow.Character
{
    /// <summary>
    /// Aggregate stat modifiers from all equipped body parts.
    /// </summary>
    [Serializable]
    public struct StatModifiers
    {
        public float health;
        public float attack;
        public float defense;
        public float speed;
    }

    /// <summary>
    /// Implements the Frankenstein Mechanic: hot-swapping modular body parts at runtime.
    /// Each <see cref="BodyPartCategory"/> maps to a <see cref="SpriteRenderer"/> slot on the character.
    /// Equipping a <see cref="BodyPartData"/> swaps the sprite while preserving bone weights,
    /// updates stat modifiers, and fires change events for dependent systems.
    /// Reference: Plan Section 7.2 - spriteResolver.SetCategoryAndLabel pattern.
    /// </summary>
    public class BodyPartSwapper : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// Serializable mapping from a <see cref="BodyPartCategory"/> to its target renderer.
        /// </summary>
        [Serializable]
        public struct BodyPartSlot
        {
            [Tooltip("The body-part category this slot represents")]
            public BodyPartCategory category;

            [Tooltip("The SpriteRenderer that displays this body part")]
            public SpriteRenderer renderer;
        }

        [Header("Slot Configuration")]
        [Tooltip("Maps each body-part category to its SpriteRenderer on the character rig")]
        [SerializeField] private BodyPartSlot[] bodyPartSlots;

        #endregion

        #region Events

        /// <summary>
        /// Fired when a body part is equipped or removed.
        /// Parameters: category that changed, new part data (null if removed).
        /// </summary>
        public event Action<BodyPartCategory, BodyPartData> OnPartChanged;

        #endregion

        #region Private Fields

        private Dictionary<BodyPartCategory, BodyPartData> equippedParts;
        private Dictionary<BodyPartCategory, SpriteRenderer> slotRenderers;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            equippedParts = new Dictionary<BodyPartCategory, BodyPartData>();
            slotRenderers = new Dictionary<BodyPartCategory, SpriteRenderer>();

            if (bodyPartSlots != null)
            {
                foreach (var slot in bodyPartSlots)
                {
                    if (slot.renderer != null)
                    {
                        slotRenderers[slot.category] = slot.renderer;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Equips a body part into its matching category slot, replacing any existing part.
        /// The sprite is applied directly to the <see cref="SpriteRenderer"/> to preserve bone weights.
        /// Reference: Plan Section 7.2 - spriteResolver.SetCategoryAndLabel pattern.
        /// </summary>
        /// <param name="part">The body part data to equip.</param>
        public void EquipPart(BodyPartData part)
        {
            if (part == null) return;

            BodyPartCategory category = part.Category;

            if (!slotRenderers.TryGetValue(category, out SpriteRenderer renderer))
            {
                Debug.LogWarning($"[BodyPartSwapper] No renderer mapped for category {category}.");
                return;
            }

            equippedParts[category] = part;
            renderer.sprite = part.PartSprite;

            OnPartChanged?.Invoke(category, part);
        }

        /// <summary>
        /// Removes the body part from the specified category slot and clears the sprite.
        /// </summary>
        /// <param name="category">The category slot to clear.</param>
        public void RemovePart(BodyPartCategory category)
        {
            if (!equippedParts.ContainsKey(category)) return;

            equippedParts.Remove(category);

            if (slotRenderers.TryGetValue(category, out SpriteRenderer renderer))
            {
                renderer.sprite = null;
            }

            OnPartChanged?.Invoke(category, null);
        }

        /// <summary>
        /// Returns the currently equipped part for the given category, or null if empty.
        /// </summary>
        /// <param name="category">The category slot to query.</param>
        /// <returns>The equipped <see cref="BodyPartData"/>, or null.</returns>
        public BodyPartData GetEquippedPart(BodyPartCategory category)
        {
            equippedParts.TryGetValue(category, out BodyPartData part);
            return part;
        }

        /// <summary>
        /// Computes the aggregate stat modifiers from all currently equipped body parts.
        /// </summary>
        /// <returns>A <see cref="StatModifiers"/> struct with the summed values.</returns>
        public StatModifiers GetTotalStatModifiers()
        {
            StatModifiers total = default;

            foreach (var part in equippedParts.Values)
            {
                total.health += part.HealthModifier;
                total.attack += part.AttackModifier;
                total.defense += part.DefenseModifier;
                total.speed += part.SpeedModifier;
            }

            return total;
        }

        /// <summary>
        /// Checks whether any equipped body part grants the specified ability.
        /// Supported ability names: "NightVision", "Flight", "VenomAttack".
        /// </summary>
        /// <param name="ability">The ability name to check for.</param>
        /// <returns>True if any equipped part grants the ability.</returns>
        public bool HasAbility(string ability)
        {
            foreach (var part in equippedParts.Values)
            {
                switch (ability)
                {
                    case "NightVision" when part.GrantsNightVision:
                    case "Flight" when part.GrantsFlight:
                    case "VenomAttack" when part.GrantsVenomAttack:
                        return true;
                }
            }

            return false;
        }

        #endregion
    }
}
