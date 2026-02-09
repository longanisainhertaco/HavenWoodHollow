using UnityEngine;

namespace HavenwoodHollow.AI.GOAP.Actions
{
    /// <summary>
    /// GOAP action: consume food to recover stamina.
    /// Always available with no preconditions.
    /// Simple timer-based consumption action.
    /// Reference: Plan Section 4.2.2 - Goals and Actions.
    /// </summary>
    public class EatFoodAction : GOAPAction
    {
        #region Serialized Fields

        [Header("Eat Settings")]
        [Tooltip("Time in seconds to consume the food")]
        [SerializeField] private float eatDuration = 2f;

        #endregion

        #region Private Fields

        private float elapsedTime;

        #endregion

        #region GOAPAction Implementation

        /// <summary>
        /// Preconditions: none — eating is always available.
        /// </summary>
        public override WorldState GetPreconditions()
        {
            return new WorldState();
        }

        /// <summary>
        /// Effects: stamina is recovered.
        /// </summary>
        public override WorldState GetEffects()
        {
            var effects = new WorldState();
            effects.SetBool("StaminaRecovered", true);
            return effects;
        }

        /// <summary>
        /// Begins eating food.
        /// </summary>
        public override void OnActionStart(GOAPAgent agent)
        {
            elapsedTime = 0f;
            Debug.Log($"[EatFoodAction] {agent.AgentName} is eating...");
        }

        /// <summary>
        /// Waits for the eat timer to complete.
        /// Returns true when the food has been consumed.
        /// </summary>
        public override bool OnActionUpdate(GOAPAgent agent)
        {
            elapsedTime += Time.deltaTime;
            return elapsedTime >= eatDuration;
        }

        /// <summary>
        /// Updates the agent's local state to reflect recovered stamina.
        /// </summary>
        public override void OnActionEnd(GOAPAgent agent)
        {
            agent.LocalState.SetBool("StaminaRecovered", true);
            Debug.Log($"[EatFoodAction] {agent.AgentName} finished eating.");
        }

        #endregion
    }
}
