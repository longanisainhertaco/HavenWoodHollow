using System.Collections.Generic;
using System.Linq;

namespace HavenwoodHollow.AI.GOAP
{
    /// <summary>
    /// GOAP Planner that works backwards from a Goal to find a sequence
    /// of Actions that satisfy it. Uses A*-like search.
    /// Reference: Plan Section 4.2.2 - The planner works backwards from a Goal.
    ///
    /// Example Plan (from spec):
    ///   Goal: DefendTown (Requires EnemyDead)
    ///   -> AttackEnemy (Requires HasWeapon)
    ///   -> RetrieveSword (Effect: HasWeapon = true)
    ///   Resulting Plan: RetrieveSword -> AttackEnemy
    /// </summary>
    public class GOAPPlanner
    {
        private class PlanNode
        {
            public PlanNode Parent;
            public GOAPAction Action;
            public WorldState State;
            public float RunningCost;
        }

        /// <summary>
        /// Formulates a plan (ordered list of actions) to achieve the given goal
        /// from the current world state using the available actions.
        /// Returns null if no plan can be found.
        /// </summary>
        public List<GOAPAction> FormulatePlan(
            WorldState currentState,
            GOAPGoal goal,
            List<GOAPAction> availableActions,
            GOAPAgent agent)
        {
            if (goal == null || goal.DesiredState == null)
                return null;

            // Filter to achievable actions
            var usableActions = availableActions
                .Where(a => a.IsAchievable(agent))
                .ToList();

            var leaves = new List<PlanNode>();
            var startNode = new PlanNode
            {
                Parent = null,
                Action = null,
                State = currentState.Clone(),
                RunningCost = 0f
            };

            // Build the plan graph
            bool success = BuildGraph(startNode, leaves, usableActions, goal.DesiredState, agent);

            if (!success || leaves.Count == 0)
                return null;

            // Find the cheapest plan
            PlanNode cheapest = leaves.OrderBy(n => n.RunningCost).First();

            // Build the action list by walking back from the leaf
            var plan = new List<GOAPAction>();
            var node = cheapest;
            while (node.Parent != null)
            {
                plan.Add(node.Action);
                node = node.Parent;
            }

            plan.Reverse();
            return plan;
        }

        private bool BuildGraph(
            PlanNode parent,
            List<PlanNode> leaves,
            List<GOAPAction> availableActions,
            WorldState goalState,
            GOAPAgent agent)
        {
            bool foundPlan = false;

            foreach (var action in availableActions)
            {
                // Check if preconditions are met in the current state
                WorldState preconditions = action.GetPreconditions();
                if (preconditions != null && !parent.State.SatisfiesConditions(preconditions))
                    continue;

                // Apply effects to get new state
                WorldState newState = parent.State.Clone();
                WorldState effects = action.GetEffects();
                if (effects != null)
                {
                    newState.ApplyEffects(effects);
                }

                var node = new PlanNode
                {
                    Parent = parent,
                    Action = action,
                    State = newState,
                    RunningCost = parent.RunningCost + action.Cost
                };

                if (newState.SatisfiesConditions(goalState))
                {
                    // Goal is reached
                    leaves.Add(node);
                    foundPlan = true;
                }
                else
                {
                    // Recurse with remaining actions (exclude current to prevent loops)
                    var remaining = availableActions.Where(a => a != action).ToList();
                    if (BuildGraph(node, leaves, remaining, goalState, agent))
                    {
                        foundPlan = true;
                    }
                }
            }

            return foundPlan;
        }
    }
}
