using UnityEngine;

namespace HavenwoodHollow.Creatures
{
    /// <summary>
    /// ScriptableObject defining a creature species' base data and default genetics.
    /// The CreatureVisualizer reads expressed traits from the genome to swap sprite parts.
    /// Reference: Plan Section 6.2.4 - CreatureVisualizer reads expressed traits.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCreature", menuName = "HavenwoodHollow/Creature Data")]
    public class CreatureData : ScriptableObject
    {
        #region Serialized Fields

        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;

        [Header("Base Stats")]
        [SerializeField] private float baseHealth = 50f;
        [SerializeField] private float baseAttack = 5f;
        [SerializeField] private float baseDefense = 3f;
        [SerializeField] private float moveSpeed = 2f;

        [Header("Genetics")]
        [Tooltip("Default genetic makeup for this creature species")]
        [SerializeField] private CreatureGenome defaultGenome;

        [Header("Visuals")]
        [Tooltip("Base body sprite variants")]
        [SerializeField] private Sprite[] bodySprites;
        [SerializeField] private RuntimeAnimatorController animatorController;

        #endregion

        #region Properties

        /// <summary>Unique identifier for this creature species.</summary>
        public string Id => id;

        /// <summary>Localized display name.</summary>
        public string DisplayName => displayName;

        /// <summary>Creature description text.</summary>
        public string Description => description;

        /// <summary>UI icon sprite.</summary>
        public Sprite Icon => icon;

        /// <summary>Base health pool.</summary>
        public float BaseHealth => baseHealth;

        /// <summary>Base attack power.</summary>
        public float BaseAttack => baseAttack;

        /// <summary>Base defense rating.</summary>
        public float BaseDefense => baseDefense;

        /// <summary>Base movement speed (tiles/second).</summary>
        public float MoveSpeed => moveSpeed;

        /// <summary>Default genome for newly spawned creatures of this species.</summary>
        public CreatureGenome DefaultGenome => defaultGenome;

        /// <summary>Available body sprite variants.</summary>
        public Sprite[] BodySprites => bodySprites;

        /// <summary>Animator controller for this creature.</summary>
        public RuntimeAnimatorController AnimatorController => animatorController;

        #endregion
    }
}
