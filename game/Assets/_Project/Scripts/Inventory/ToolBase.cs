using UnityEngine;

namespace HavenwoodHollow.Inventory
{
    /// <summary>
    /// Abstract MonoBehaviour base class implementing ITool.
    /// Uses composition model with Physics2D.OverlapCircle for tool actions.
    /// Subclasses implement OnToolUsed for specific behavior.
    /// Reference: Plan Section 3.3.1 - Clean Room Tool Logic.
    /// </summary>
    public abstract class ToolBase : MonoBehaviour, ITool
    {
        [Header("Tool Configuration")]
        [Tooltip("The type of tool (Hoe, Axe, etc.)")]
        [SerializeField] private ToolType toolType;

        [Tooltip("Tool tier level (Copper=1, Iron=2, Gold=3, Iridium=4).")]
        [SerializeField] private int tierLevel = 1;

        [Tooltip("Stamina cost per use.")]
        [SerializeField] private float staminaCost = 2f;

        [Header("Action Parameters")]
        [Tooltip("Base damage before tier scaling.")]
        [SerializeField] private float baseDamage = 10f;

        [Tooltip("Radius of the Physics2D.OverlapCircle used for hit detection.")]
        [SerializeField] private float actionRadius = 0.5f;

        /// <inheritdoc/>
        public ToolType Type => toolType;

        /// <inheritdoc/>
        public int TierLevel => tierLevel;

        /// <inheritdoc/>
        public float StaminaCost => staminaCost;

        /// <summary>Base damage before tier scaling.</summary>
        public float BaseDamage => baseDamage;

        /// <summary>Radius used for overlap detection.</summary>
        public float ActionRadius => actionRadius;

        /// <summary>
        /// Uses the tool at the target position. Detects nearby objects
        /// with Physics2D.OverlapCircleAll, applies damage to IDamageable
        /// targets, interacts with IInteractable targets, then delegates
        /// to the subclass via OnToolUsed.
        /// </summary>
        public void UseTool(Vector2 targetPosition, GameObject user)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, actionRadius);
            float damage = baseDamage * tierLevel;

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == user)
                    continue;

                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.TakeDamage(damage);

                IInteractable interactable = hit.GetComponent<IInteractable>();
                if (interactable != null)
                    interactable.Interact(user);
            }

            OnToolUsed(targetPosition, hits);
        }

        /// <summary>
        /// Called after the base tool action completes.
        /// Override in subclasses to implement tool-specific behavior.
        /// </summary>
        protected abstract void OnToolUsed(Vector2 position, Collider2D[] hits);
    }
}
