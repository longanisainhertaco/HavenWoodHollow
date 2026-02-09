using UnityEngine;

namespace HavenwoodHollow.AI.GOAP
{
    /// <summary>
    /// Defines an action that a GOAP agent can perform.
    /// Each action has preconditions (what must be true to execute)
    /// and effects (what changes in the world state after execution).
    /// Reference: Plan Section 4.2.2 - Goals and Actions.
    /// </summary>
    public abstract class GOAPAction : MonoBehaviour
    {
        [Header("Action Settings")]
        [SerializeField] private string actionName;
        [SerializeField] private float cost = 1f;
        [SerializeField] private float duration = 1f;

        /// <summary>Name of this action.</summary>
        public string ActionName => actionName;

        /// <summary>
        /// Cost of this action for the planner.
        /// The planner prefers lower-cost plans.
        /// </summary>
        public float Cost => cost;

        /// <summary>Time in seconds to perform this action.</summary>
        public float Duration => duration;

        /// <summary>
        /// The preconditions that must be met in the world state
        /// before this action can be included in a plan.
        /// </summary>
        public abstract WorldState GetPreconditions();

        /// <summary>
        /// The effects this action has on the world state when executed.
        /// </summary>
        public abstract WorldState GetEffects();

        /// <summary>
        /// Whether this action is currently achievable given the agent and world.
        /// Can perform runtime checks (e.g., is a weapon nearby?).
        /// </summary>
        public virtual bool IsAchievable(GOAPAgent agent)
        {
            return true;
        }

        /// <summary>
        /// Called when the action starts executing.
        /// </summary>
        public abstract void OnActionStart(GOAPAgent agent);

        /// <summary>
        /// Called each frame while the action is executing.
        /// Returns true when the action is complete.
        /// </summary>
        public abstract bool OnActionUpdate(GOAPAgent agent);

        /// <summary>
        /// Called when the action completes or is cancelled.
        /// </summary>
        public abstract void OnActionEnd(GOAPAgent agent);
    }
}
