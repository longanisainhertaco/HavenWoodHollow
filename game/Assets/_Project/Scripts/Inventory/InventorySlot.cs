using UnityEngine;

namespace HavenwoodHollow.Inventory
{
    /// <summary>
    /// Data structure representing a single inventory slot.
    /// Holds an item reference and a stack quantity.
    /// Reference: Plan Section 3.3 - Inventory System.
    /// </summary>
    [System.Serializable]
    public class InventorySlot
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private int quantity;

        /// <summary>The item stored in this slot, or null if empty.</summary>
        public ItemData ItemData => itemData;

        /// <summary>Current stack quantity.</summary>
        public int Quantity => quantity;

        /// <summary>Whether this slot contains no item.</summary>
        public bool IsEmpty => itemData == null || quantity <= 0;

        /// <summary>Whether this slot has reached the item's max stack size.</summary>
        public bool IsFull => !IsEmpty && quantity >= itemData.MaxStackSize;

        /// <summary>
        /// Creates a new inventory slot with the specified item and quantity.
        /// </summary>
        public InventorySlot(ItemData item, int qty)
        {
            itemData = item;
            quantity = item != null ? Mathf.Clamp(qty, 0, item.MaxStackSize) : 0;
        }

        /// <summary>
        /// Adds quantity to this slot. Returns the leftover amount that didn't fit.
        /// </summary>
        public int AddQuantity(int amount)
        {
            if (itemData == null || amount <= 0)
                return amount;

            int spaceAvailable = itemData.MaxStackSize - quantity;
            int toAdd = Mathf.Min(amount, spaceAvailable);
            quantity += toAdd;
            return amount - toAdd;
        }

        /// <summary>
        /// Removes quantity from this slot. Returns the actual amount removed.
        /// </summary>
        public int RemoveQuantity(int amount)
        {
            if (amount <= 0)
                return 0;

            int toRemove = Mathf.Min(amount, quantity);
            quantity -= toRemove;

            if (quantity <= 0)
                Clear();

            return toRemove;
        }

        /// <summary>
        /// Clears this slot, removing the item and resetting quantity.
        /// </summary>
        public void Clear()
        {
            itemData = null;
            quantity = 0;
        }
    }
}
