using UnityEngine;

namespace HavenwoodHollow.Inventory
{
    /// <summary>
    /// Interface for objects that can be interacted with by the player.
    /// Reference: Plan Section 3.3.1 - Clean Room Tool Logic.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Display name shown in the interaction prompt.</summary>
        string InteractionPrompt { get; }

        /// <summary>Called when the player interacts with this object.</summary>
        bool Interact(GameObject interactor);
    }
}
