using UnityEngine;

namespace HavenwoodHollow.Character
{
    /// <summary>
    /// Categories representing attachment slots on a character body.
    /// Reference: Plan Section 7 - Modular Character System (Frankenstein Mechanic).
    /// </summary>
    public enum BodyPartCategory
    {
        Head,
        Torso,
        ArmLeft,
        ArmRight,
        LegLeft,
        LegRight,
        WingLeft,
        WingRight,
        Tail
    }

    /// <summary>
    /// The visual/thematic type of a body part, determining its origin.
    /// </summary>
    public enum BodyPartType
    {
        Human,
        Skeleton,
        Werewolf,
        Mechanical,
        Vampire,
        Eldritch
    }

    /// <summary>
    /// ScriptableObject defining a single swappable body part for the Frankenstein Mechanic.
    /// Each part belongs to a category (slot) and type (origin), carries stat modifiers,
    /// and may grant special abilities such as night vision or flight.
    /// Reference: Plan Section 7 - Modular Character System (Frankenstein Mechanic).
    /// </summary>
    [CreateAssetMenu(fileName = "NewBodyPart", menuName = "HavenwoodHollow/Body Part Data")]
    public class BodyPartData : ScriptableObject
    {
        #region Serialized Fields

        [Header("Identity")]
        [Tooltip("Unique identifier for this body part")]
        [SerializeField] private string id;

        [Tooltip("Localized display name shown in the UI")]
        [SerializeField] private string displayName;

        [Tooltip("Which slot this part occupies on the character body")]
        [SerializeField] private BodyPartCategory category;

        [Tooltip("The origin type of this body part")]
        [SerializeField] private BodyPartType partType;

        [Header("Visuals")]
        [Tooltip("Sprite rendered for this body part")]
        [SerializeField] private Sprite partSprite;

        [Tooltip("Label used by SpriteResolver to select the correct sprite from a Sprite Library")]
        [SerializeField] private string spriteLibraryLabel;

        [Header("Stat Modifiers")]
        [Tooltip("Additive modifier applied to the character's health pool")]
        [SerializeField] private float healthModifier;

        [Tooltip("Additive modifier applied to the character's attack power")]
        [SerializeField] private float attackModifier;

        [Tooltip("Additive modifier applied to the character's defense rating")]
        [SerializeField] private float defenseModifier;

        [Tooltip("Additive modifier applied to the character's movement speed")]
        [SerializeField] private float speedModifier;

        [Header("Special Effects")]
        [Tooltip("Whether this part grants night vision ability")]
        [SerializeField] private bool grantsNightVision;

        [Tooltip("Whether this part grants flight ability")]
        [SerializeField] private bool grantsFlight;

        [Tooltip("Whether this part grants a venom attack ability")]
        [SerializeField] private bool grantsVenomAttack;

        #endregion

        #region Properties

        /// <summary>Unique identifier for this body part.</summary>
        public string Id => id;

        /// <summary>Localized display name shown in the UI.</summary>
        public string DisplayName => displayName;

        /// <summary>Which slot this part occupies on the character body.</summary>
        public BodyPartCategory Category => category;

        /// <summary>The origin type of this body part.</summary>
        public BodyPartType PartType => partType;

        /// <summary>Sprite rendered for this body part.</summary>
        public Sprite PartSprite => partSprite;

        /// <summary>Label used by SpriteResolver to select the correct sprite from a Sprite Library.</summary>
        public string SpriteLibraryLabel => spriteLibraryLabel;

        /// <summary>Additive modifier applied to the character's health pool.</summary>
        public float HealthModifier => healthModifier;

        /// <summary>Additive modifier applied to the character's attack power.</summary>
        public float AttackModifier => attackModifier;

        /// <summary>Additive modifier applied to the character's defense rating.</summary>
        public float DefenseModifier => defenseModifier;

        /// <summary>Additive modifier applied to the character's movement speed.</summary>
        public float SpeedModifier => speedModifier;

        /// <summary>Whether this part grants night vision ability.</summary>
        public bool GrantsNightVision => grantsNightVision;

        /// <summary>Whether this part grants flight ability.</summary>
        public bool GrantsFlight => grantsFlight;

        /// <summary>Whether this part grants a venom attack ability.</summary>
        public bool GrantsVenomAttack => grantsVenomAttack;

        #endregion
    }
}
