namespace HavenwoodHollow.AI.GOAP
{
    /// <summary>
    /// Defines a goal that a GOAP agent wants to achieve.
    /// Goals have a priority and a set of conditions that constitute success.
    /// Reference: Plan Section 4.2.2, Table 2 - GOAP Configuration for 'Village Guard'.
    /// </summary>
    [System.Serializable]
    public class GOAPGoal
    {
        /// <summary>Name of the goal (e.g., "DefendTown", "Survive").</summary>
        public string Name;

        /// <summary>
        /// Priority of the goal. Higher = more important.
        /// Survive=100, DefendTown=80, MaintainEnergy=40, Socialize=20.
        /// </summary>
        public int Priority;

        /// <summary>
        /// The world state conditions that must be true for this goal to be satisfied.
        /// </summary>
        public WorldState DesiredState;

        /// <summary>
        /// The world state conditions that must be true for this goal to become active.
        /// E.g., DefendTown requires RaidActive == true.
        /// </summary>
        public WorldState ActivationConditions;

        /// <summary>
        /// Checks if this goal should be active given the current world state.
        /// </summary>
        public bool IsActive(WorldState currentState)
        {
            if (ActivationConditions == null) return true;
            return currentState.SatisfiesConditions(ActivationConditions);
        }
    }
}
