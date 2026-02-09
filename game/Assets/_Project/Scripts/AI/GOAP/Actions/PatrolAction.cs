using UnityEngine;

namespace HavenwoodHollow.AI.GOAP.Actions
{
    /// <summary>
    /// GOAP action: patrol a set of waypoints to secure an area.
    /// Used by guards during raids to sweep for enemies.
    /// Reference: Plan Section 4.2.2 - Goals and Actions.
    /// </summary>
    public class PatrolAction : GOAPAction
    {
        #region Serialized Fields

        [Header("Patrol Settings")]
        [Tooltip("Waypoints to visit in sequence")]
        [SerializeField] private Transform[] patrolPoints;

        [Tooltip("Maximum time before the patrol auto-completes")]
        [SerializeField] private float timeout = 30f;

        #endregion

        #region Private Fields

        private NPCController npcController;
        private int currentPointIndex;
        private float elapsedTime;

        #endregion

        #region GOAPAction Implementation

        /// <summary>
        /// Preconditions: a raid must be active and the NPC must be armed.
        /// </summary>
        public override WorldState GetPreconditions()
        {
            var preconditions = new WorldState();
            preconditions.SetBool("RaidActive", true);
            preconditions.SetBool("HasWeapon", true);
            return preconditions;
        }

        /// <summary>
        /// Effects: the patrol area is considered secured.
        /// </summary>
        public override WorldState GetEffects()
        {
            var effects = new WorldState();
            effects.SetBool("AreaSecured", true);
            return effects;
        }

        /// <summary>
        /// Begins the patrol by moving to the first waypoint.
        /// </summary>
        public override void OnActionStart(GOAPAgent agent)
        {
            npcController = agent.GetComponent<NPCController>();
            currentPointIndex = 0;
            elapsedTime = 0f;

            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                npcController.MoveTo(patrolPoints[currentPointIndex].position);
            }
        }

        /// <summary>
        /// Advances through patrol points. Returns true when all points
        /// have been visited or the timeout is reached.
        /// </summary>
        public override bool OnActionUpdate(GOAPAgent agent)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= timeout)
            {
                return true;
            }

            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                return true;
            }

            if (npcController.HasReachedTarget)
            {
                currentPointIndex++;

                if (currentPointIndex >= patrolPoints.Length)
                {
                    return true;
                }

                npcController.MoveTo(patrolPoints[currentPointIndex].position);
            }

            return false;
        }

        /// <summary>
        /// Stops the NPC when the patrol ends.
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
