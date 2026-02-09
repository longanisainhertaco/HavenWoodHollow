using System.Collections.Generic;

namespace HavenwoodHollow.AI.GOAP
{
    /// <summary>
    /// Represents the world state as a dictionary of boolean/integer values.
    /// The GOAP planner uses this to determine which actions are available
    /// and which goals are satisfiable.
    /// Reference: Plan Section 4.2.1 - World State Definition.
    /// </summary>
    public class WorldState
    {
        private Dictionary<string, bool> boolStates = new Dictionary<string, bool>();
        private Dictionary<string, int> intStates = new Dictionary<string, int>();

        #region Boolean State

        public void SetBool(string key, bool value)
        {
            boolStates[key] = value;
        }

        public bool GetBool(string key)
        {
            return boolStates.TryGetValue(key, out bool value) && value;
        }

        public bool HasBoolKey(string key)
        {
            return boolStates.ContainsKey(key);
        }

        #endregion

        #region Integer State

        public void SetInt(string key, int value)
        {
            intStates[key] = value;
        }

        public int GetInt(string key)
        {
            return intStates.TryGetValue(key, out int value) ? value : 0;
        }

        public bool HasIntKey(string key)
        {
            return intStates.ContainsKey(key);
        }

        #endregion

        /// <summary>
        /// Creates a deep copy of this world state.
        /// Used by the planner to simulate applying actions.
        /// </summary>
        public WorldState Clone()
        {
            var clone = new WorldState();
            foreach (var kvp in boolStates)
                clone.boolStates[kvp.Key] = kvp.Value;
            foreach (var kvp in intStates)
                clone.intStates[kvp.Key] = kvp.Value;
            return clone;
        }

        /// <summary>
        /// Checks whether this state satisfies all conditions in the target state.
        /// </summary>
        public bool SatisfiesConditions(WorldState conditions)
        {
            foreach (var kvp in conditions.boolStates)
            {
                if (!boolStates.TryGetValue(kvp.Key, out bool value) || value != kvp.Value)
                    return false;
            }
            foreach (var kvp in conditions.intStates)
            {
                if (!intStates.TryGetValue(kvp.Key, out int value) || value < kvp.Value)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Applies the effects of an action to this world state.
        /// </summary>
        public void ApplyEffects(WorldState effects)
        {
            foreach (var kvp in effects.boolStates)
                boolStates[kvp.Key] = kvp.Value;
            foreach (var kvp in effects.intStates)
                intStates[kvp.Key] = kvp.Value;
        }
    }
}
