using UnityEngine;

namespace HavenwoodHollow.Inventory
{
    /// <summary>
    /// Interface for items that can be consumed (food, potions).
    /// Reference: Plan Section 3.3.1 - Clean Room Tool Logic.
    /// </summary>
    public interface IConsumable
    {
        /// <summary>Stamina restored on consumption.</summary>
        float StaminaRestore { get; }

        /// <summary>Health restored on consumption.</summary>
        float HealthRestore { get; }

        /// <summary>Duration of any applied buff in seconds.</summary>
        float BuffDuration { get; }

        /// <summary>Consumes the item and applies its effects.</summary>
        void Consume(GameObject consumer);
    }
}
