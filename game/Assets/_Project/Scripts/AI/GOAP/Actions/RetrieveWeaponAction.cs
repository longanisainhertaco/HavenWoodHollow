using UnityEngine;

namespace HavenwoodHollow.AI.GOAP.Actions
{
    /// <summary>
    /// GOAP action: move to a weapon storage location and equip a weapon.
    /// Sets the agent's local state HasWeapon to true upon completion.
    /// Reference: Plan Section 4.2.2 - Goals and Actions.
    /// </summary>
    public class RetrieveWeaponAction : GOAPAction
    {
        #region Serialized Fields

        [Header("Weapon Storage")]
        [Tooltip("Location where the NPC retrieves a weapon")]
        [SerializeField] private Transform weaponStorageLocation;

        #endregion

        #region Private Fields

        private NPCController npcController;
        private bool hasArrived;

        #endregion

        #region GOAPAction Implementation

        /// <summary>
        /// Preconditions: the NPC does not have a weapon.
        /// </summary>
        public override WorldState GetPreconditions()
        {
            var preconditions = new WorldState();
            preconditions.SetBool("HasWeapon", false);
            return preconditions;
        }

        /// <summary>
        /// Effects: the NPC now has a weapon.
        /// </summary>
        public override WorldState GetEffects()
        {
            var effects = new WorldState();
            effects.SetBool("HasWeapon", true);
            return effects;
        }

        /// <summary>
        /// Begins moving to the weapon storage location.
        /// </summary>
        public override void OnActionStart(GOAPAgent agent)
        {
            npcController = agent.GetComponent<NPCController>();
            hasArrived = false;

            if (weaponStorageLocation != null)
            {
                npcController.MoveTo(weaponStorageLocation.position);
            }
            else
            {
                Debug.LogWarning($"[RetrieveWeaponAction] No weapon storage location set on {agent.AgentName}");
                hasArrived = true;
            }
        }

        /// <summary>
        /// Waits until the NPC reaches the storage location,
        /// then sets HasWeapon on the agent's local state.
        /// </summary>
        public override bool OnActionUpdate(GOAPAgent agent)
        {
            if (hasArrived || npcController.HasReachedTarget)
            {
                agent.LocalState.SetBool("HasWeapon", true);
                return true;
            }

            return false;
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
