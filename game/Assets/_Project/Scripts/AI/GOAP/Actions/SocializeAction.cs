using UnityEngine;

namespace HavenwoodHollow.AI.GOAP.Actions
{
    /// <summary>
    /// GOAP action: travel to the tavern and socialize during the evening.
    /// Fulfills the NPC's social needs when the day's work is done.
    /// Reference: Plan Section 4.2.2 - Goals and Actions.
    /// </summary>
    public class SocializeAction : GOAPAction
    {
        #region Serialized Fields

        [Header("Socialize Settings")]
        [Tooltip("Location of the tavern or social gathering spot")]
        [SerializeField] private Transform tavernLocation;

        [Tooltip("Time in seconds spent socializing")]
        [SerializeField] private float socializeDuration = 5f;

        #endregion

        #region Private Fields

        private NPCController npcController;
        private bool hasArrived;
        private float elapsedTime;

        #endregion

        #region GOAPAction Implementation

        /// <summary>
        /// Preconditions: it must be evening.
        /// </summary>
        public override WorldState GetPreconditions()
        {
            var preconditions = new WorldState();
            preconditions.SetBool("IsEvening", true);
            return preconditions;
        }

        /// <summary>
        /// Effects: the NPC has socialized.
        /// </summary>
        public override WorldState GetEffects()
        {
            var effects = new WorldState();
            effects.SetBool("Socialized", true);
            return effects;
        }

        /// <summary>
        /// Begins moving to the tavern.
        /// </summary>
        public override void OnActionStart(GOAPAgent agent)
        {
            npcController = agent.GetComponent<NPCController>();
            hasArrived = false;
            elapsedTime = 0f;

            if (tavernLocation != null)
            {
                npcController.MoveTo(tavernLocation.position);
            }
            else
            {
                Debug.LogWarning($"[SocializeAction] No tavern location set on {agent.AgentName}");
                hasArrived = true;
            }
        }

        /// <summary>
        /// Moves to the tavern, then waits for the socializing duration.
        /// Returns true when socializing is complete.
        /// </summary>
        public override bool OnActionUpdate(GOAPAgent agent)
        {
            if (!hasArrived)
            {
                if (npcController.HasReachedTarget)
                {
                    hasArrived = true;
                    npcController.Stop();
                }
                return false;
            }

            // Socializing in progress
            elapsedTime += Time.deltaTime;
            return elapsedTime >= socializeDuration;
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
