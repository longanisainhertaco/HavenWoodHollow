using UnityEngine;

namespace HavenwoodHollow.AI.GOAP.Actions
{
    /// <summary>
    /// GOAP action: move to a gate and barricade it during a raid.
    /// The NPC travels to the gate, then spends time reinforcing it.
    /// Reference: Plan Section 4.3 - Procedural Raid Generation.
    /// </summary>
    public class BarricadeGateAction : GOAPAction
    {
        #region Serialized Fields

        [Header("Barricade Settings")]
        [Tooltip("Location of the gate to barricade")]
        [SerializeField] private Transform gateLocation;

        [Tooltip("Time in seconds to barricade the gate")]
        [SerializeField] private float barricadeTime = 3f;

        #endregion

        #region Private Fields

        private NPCController npcController;
        private bool hasArrived;
        private float elapsedTime;

        #endregion

        #region GOAPAction Implementation

        /// <summary>
        /// Preconditions: a raid must be active.
        /// </summary>
        public override WorldState GetPreconditions()
        {
            var preconditions = new WorldState();
            preconditions.SetBool("RaidActive", true);
            return preconditions;
        }

        /// <summary>
        /// Effects: the gate is barricaded.
        /// </summary>
        public override WorldState GetEffects()
        {
            var effects = new WorldState();
            effects.SetBool("GateBarricaded", true);
            return effects;
        }

        /// <summary>
        /// Begins moving to the gate location.
        /// </summary>
        public override void OnActionStart(GOAPAgent agent)
        {
            npcController = agent.GetComponent<NPCController>();
            hasArrived = false;
            elapsedTime = 0f;

            if (gateLocation != null)
            {
                npcController.MoveTo(gateLocation.position);
            }
            else
            {
                Debug.LogWarning($"[BarricadeGateAction] No gate location set on {agent.AgentName}");
                hasArrived = true;
            }
        }

        /// <summary>
        /// Moves to the gate, then spends time barricading it.
        /// Returns true when the barricade is complete.
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

            // Barricading in progress
            elapsedTime += Time.deltaTime;
            return elapsedTime >= barricadeTime;
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
