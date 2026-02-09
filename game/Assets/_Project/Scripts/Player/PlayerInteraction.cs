using UnityEngine;
using UnityEngine.InputSystem;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.Player
{
    /// <summary>
    /// Handles player interaction with world objects and tool usage.
    /// Uses Physics2D.OverlapCircleAll to detect nearby interactables.
    /// Reference: Plan Section 3.3.1 - Tool Action using Physics2D.OverlapCircle.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Interaction Settings")]
        [Tooltip("Radius for detecting interactable objects")]
        [SerializeField] private float interactionRadius = 1.5f;
        [Tooltip("Layer mask for interactable objects")]
        [SerializeField] private LayerMask interactionLayerMask;
        [Tooltip("Offset in facing direction for interaction origin")]
        [SerializeField] private float facingOffset = 0.5f;

        #endregion

        #region Private Fields

        private PlayerStats playerStats;
        private ITool currentTool;
        private Vector2 facingDirection = Vector2.down;

        #endregion

        #region Properties

        /// <summary>Current interaction prompt to display in the UI.</summary>
        public string CurrentPrompt { get; private set; } = string.Empty;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            UpdateInteractionPrompt();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called by the Input System when the interact button is pressed.
        /// Finds the nearest IInteractable and calls Interact().
        /// </summary>
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            IInteractable nearest = FindNearestInteractable();
            if (nearest != null)
            {
                nearest.Interact(gameObject);
            }
        }

        /// <summary>
        /// Called by the Input System when the tool use button is pressed.
        /// Checks stamina, consumes it, and applies tool action.
        /// </summary>
        public void OnUseTool(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (currentTool == null) return;
            if (playerStats == null) return;

            if (!playerStats.HasEnoughStamina(currentTool.StaminaCost)) return;

            playerStats.UseStamina(currentTool.StaminaCost);

            Vector2 targetPosition = (Vector2)transform.position + facingDirection * facingOffset;
            currentTool.UseTool(targetPosition, gameObject);

            ApplyToolDamage(targetPosition);
        }

        /// <summary>
        /// Equips a tool for use.
        /// </summary>
        public void EquipTool(ITool tool)
        {
            currentTool = tool;
        }

        /// <summary>
        /// Unequips the current tool.
        /// </summary>
        public void UnequipTool()
        {
            currentTool = null;
        }

        /// <summary>
        /// Sets the facing direction for interaction targeting.
        /// Called by movement system to keep interaction origin aligned.
        /// </summary>
        public void SetFacingDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                facingDirection = direction.normalized;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Scans for nearby interactables and updates the prompt string.
        /// </summary>
        private void UpdateInteractionPrompt()
        {
            IInteractable nearest = FindNearestInteractable();
            CurrentPrompt = nearest != null ? nearest.InteractionPrompt : string.Empty;
        }

        /// <summary>
        /// Finds the nearest IInteractable within interaction radius.
        /// </summary>
        private IInteractable FindNearestInteractable()
        {
            Vector2 origin = (Vector2)transform.position + facingDirection * facingOffset;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, interactionRadius, interactionLayerMask);

            IInteractable nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider2D hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();
                if (interactable == null) continue;

                float distance = Vector2.Distance(origin, hit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = interactable;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Applies damage to the nearest IDamageable at the target position.
        /// </summary>
        private void ApplyToolDamage(Vector2 targetPosition)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, interactionRadius, interactionLayerMask);

            IDamageable nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider2D hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable == null || !damageable.IsAlive) continue;

                float distance = Vector2.Distance(targetPosition, hit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = damageable;
                }
            }

            if (nearest != null)
            {
                nearest.TakeDamage(currentTool.TierLevel);
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector2 origin = (Vector2)transform.position + facingDirection * facingOffset;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, interactionRadius);
        }
        #endif

        #endregion
    }
}
