using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HavenwoodHollow.AI.GOAP
{
    /// <summary>
    /// GOAP Agent component attached to NPCs.
    /// Manages goals, available actions, and plan execution.
    /// Reference: Plan Section 4 - Goal-Oriented Action Planning.
    /// </summary>
    public class GOAPAgent : MonoBehaviour
    {
        [Header("Agent Settings")]
        [SerializeField] private string agentName;

        /// <summary>Local world state for this agent (HasWeapon, HealthLow, etc.).</summary>
        private WorldState localState = new WorldState();

        /// <summary>Goals sorted by priority. Set up via AddGoal().</summary>
        private List<GOAPGoal> goals = new List<GOAPGoal>();

        /// <summary>Available actions collected from child components.</summary>
        private List<GOAPAction> availableActions = new List<GOAPAction>();

        /// <summary>The current plan being executed.</summary>
        private Queue<GOAPAction> currentPlan = new Queue<GOAPAction>();

        /// <summary>The action currently being executed.</summary>
        private GOAPAction currentAction;

        private GOAPPlanner planner = new GOAPPlanner();

        /// <summary>Reference to the shared global world state.</summary>
        private WorldState globalState;

        public WorldState LocalState => localState;
        public string AgentName => agentName;

        private void Start()
        {
            // Collect all actions from child GameObjects
            availableActions = new List<GOAPAction>(GetComponentsInChildren<GOAPAction>());
        }

        private void Update()
        {
            if (currentAction != null)
            {
                // Execute current action
                if (currentAction.OnActionUpdate(this))
                {
                    // Action complete
                    currentAction.OnActionEnd(this);
                    currentAction = null;
                    ExecuteNextAction();
                }
            }
            else if (currentPlan.Count == 0)
            {
                // No plan - create one
                CreatePlan();
            }
            else
            {
                ExecuteNextAction();
            }
        }

        /// <summary>
        /// Sets the global world state reference (shared across all agents).
        /// </summary>
        public void SetGlobalState(WorldState state)
        {
            globalState = state;
        }

        /// <summary>
        /// Adds a goal to this agent.
        /// </summary>
        public void AddGoal(GOAPGoal goal)
        {
            goals.Add(goal);
            goals = goals.OrderByDescending(g => g.Priority).ToList();
        }

        /// <summary>
        /// Merges global and local state into a combined world state for planning.
        /// </summary>
        private WorldState GetCombinedState()
        {
            WorldState combined = globalState != null ? globalState.Clone() : new WorldState();
            combined.ApplyEffects(localState);
            return combined;
        }

        /// <summary>
        /// Creates a plan by finding the highest-priority active goal and planning for it.
        /// </summary>
        private void CreatePlan()
        {
            WorldState combinedState = GetCombinedState();

            // Find the highest priority active goal
            foreach (var goal in goals)
            {
                if (!goal.IsActive(combinedState))
                    continue;

                // Already satisfied?
                if (combinedState.SatisfiesConditions(goal.DesiredState))
                    continue;

                var plan = planner.FormulatePlan(combinedState, goal, availableActions, this);
                if (plan != null && plan.Count > 0)
                {
                    currentPlan = new Queue<GOAPAction>(plan);
                    Debug.Log($"[GOAP] {agentName} created plan for goal '{goal.Name}': " +
                              string.Join(" -> ", plan.Select(a => a.ActionName)));
                    return;
                }
            }
        }

        private void ExecuteNextAction()
        {
            if (currentPlan.Count == 0) return;

            currentAction = currentPlan.Dequeue();
            currentAction.OnActionStart(this);
        }

        /// <summary>
        /// Forces the agent to re-plan (e.g., when world state changes significantly).
        /// </summary>
        public void Replan()
        {
            if (currentAction != null)
            {
                currentAction.OnActionEnd(this);
                currentAction = null;
            }
            currentPlan.Clear();
        }
    }
}
