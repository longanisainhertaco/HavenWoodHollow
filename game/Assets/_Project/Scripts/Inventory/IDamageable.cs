namespace HavenwoodHollow.Inventory
{
    /// <summary>
    /// Interface for objects that can receive damage from tools or attacks.
    /// When a tool hits an object implementing IDamageable, damage is applied.
    /// Reference: Plan Section 3.3.1 - Tool Action.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>Current health of the object.</summary>
        float CurrentHealth { get; }

        /// <summary>Maximum health of the object.</summary>
        float MaxHealth { get; }

        /// <summary>Whether this object is still alive/intact.</summary>
        bool IsAlive { get; }

        /// <summary>Applies damage to the object.</summary>
        void TakeDamage(float damage);

        /// <summary>Called when health reaches zero.</summary>
        void OnDestroyed();
    }
}
