using UnityEngine;

namespace HavenwoodHollow.AI.GOAP.Actions
{
    /// <summary>
    /// GOAP action: flee to a safe location (bunker) when health is low.
    /// Reference: Plan Section 4.2.2 - Goals and Actions.
    /// </summary>
    public class FleeAction : GOAPAction
    {
        #region Serialized Fields

        [Header("Flee Settings")]
        [Tooltip("Safe location the NPC flees to (e.g., bunker)")]
        [SerializeField] private Transform safeLocation;

        #endregion

        #region Private Fields

        private NPCController npcController;

        #endregion

        #region GOAPAction Implementation

        /// <summary>
        /// Preconditions: the NPC's health is low.
        /// </summary>
        public override WorldState GetPreconditions()
        {
            var preconditions = new WorldState();
            preconditions.SetBool("HealthLow", true);
            return preconditions;
        }

        /// <summary>
        /// Effects: the NPC is safe.
        /// </summary>
        public override WorldState GetEffects()
        {
            var effects = new WorldState();
            effects.SetBool("IsSafe", true);
            return effects;
        }

        /// <summary>
        /// Begins fleeing to the safe location.
        /// </summary>
        public override void OnActionStart(GOAPAgent agent)
        {
            npcController = agent.GetComponent<NPCController>();

            if (safeLocation != null)
            {
                npcController.MoveTo(safeLocation.position);
            }
            else
            {
                Debug.LogWarning($"[FleeAction] No safe location set on {agent.AgentName}");
            }
        }

        /// <summary>
        /// Returns true when the NPC reaches the safe location.
        /// </summary>
        public override bool OnActionUpdate(GOAPAgent agent)
        {
            return npcController.HasReachedTarget;
        }

        /// <summary>
        /// Stops the NPC when the action ends.
        /// </summary>
        public override void OnActionEnd(GOAPAgent agent)
        {
            if (npcController != null)
            {
                npcController.Stop();
            }
        }

        #endregion
    }
}
