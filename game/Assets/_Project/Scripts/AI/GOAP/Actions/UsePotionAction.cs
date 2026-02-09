using UnityEngine;

namespace HavenwoodHollow.AI.GOAP.Actions
{
    /// <summary>
    /// GOAP action: consume a health potion to recover from low health.
    /// Timer-based action that restores the NPC after a brief duration.
    /// Reference: Plan Section 4.2.2 - Goals and Actions.
    /// </summary>
    public class UsePotionAction : GOAPAction
    {
        #region Serialized Fields

        [Header("Potion Settings")]
        [Tooltip("Time in seconds to consume the potion")]
        [SerializeField] private float consumeTime = 1.5f;

        #endregion

        #region Private Fields

        private float elapsedTime;

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
        /// Effects: the NPC's health is no longer low.
        /// </summary>
        public override WorldState GetEffects()
        {
            var effects = new WorldState();
            effects.SetBool("HealthLow", false);
            return effects;
        }

        /// <summary>
        /// Begins consuming the potion.
        /// </summary>
        public override void OnActionStart(GOAPAgent agent)
        {
            elapsedTime = 0f;
            Debug.Log($"[UsePotionAction] {agent.AgentName} is consuming a potion...");
        }

        /// <summary>
        /// Waits for the consume timer to complete.
        /// Returns true when the potion has been fully consumed.
        /// </summary>
        public override bool OnActionUpdate(GOAPAgent agent)
        {
            elapsedTime += Time.deltaTime;
            return elapsedTime >= consumeTime;
        }

        /// <summary>
        /// Updates the agent's local state to reflect restored health.
        /// </summary>
        public override void OnActionEnd(GOAPAgent agent)
        {
            agent.LocalState.SetBool("HealthLow", false);
            Debug.Log($"[UsePotionAction] {agent.AgentName} consumed a potion.");
        }

        #endregion
    }
}
