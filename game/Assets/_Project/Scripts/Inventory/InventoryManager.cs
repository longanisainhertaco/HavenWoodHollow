using System;
using UnityEngine;

namespace HavenwoodHollow.Inventory
{
    /// <summary>
    /// Singleton manager for the player's inventory.
    /// Handles adding, removing, swapping items and hotbar selection.
    /// Reference: Plan Section 3.3 and Phase 2.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Inventory Layout")]
        [Tooltip("Total number of inventory slots (3 rows of 12).")]
        [SerializeField] private int inventorySize = 36;

        [Tooltip("Number of hotbar slots (first row).")]
        [SerializeField] private int hotbarSize = 12;

        private InventorySlot[] slots;
        private int selectedSlotIndex;

        /// <summary>Fired when a slot's contents change. Parameter is the slot index.</summary>
        public event Action<int> OnInventoryChanged;

        /// <summary>Fired when the active hotbar slot changes. Parameter is the new index.</summary>
        public event Action<int> OnSlotSelected;

        /// <summary>Total number of inventory slots.</summary>
        public int InventorySize => inventorySize;

        /// <summary>Number of hotbar slots.</summary>
        public int HotbarSize => hotbarSize;

        /// <summary>Read-only access to all inventory slots.</summary>
        public ReadOnlySpan<InventorySlot> Slots => slots;

        /// <summary>Currently selected hotbar slot index.</summary>
        public int SelectedSlotIndex => selectedSlotIndex;

        /// <summary>The inventory slot currently selected in the hotbar.</summary>
        public InventorySlot SelectedSlot => GetItem(selectedSlotIndex);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            slots = new InventorySlot[inventorySize];
            for (int i = 0; i < inventorySize; i++)
                slots[i] = new InventorySlot(null, 0);
        }

        /// <summary>
        /// Adds an item to the inventory. Tries to stack onto existing slots first,
        /// then places into the first empty slot.
        /// </summary>
        /// <returns>True if the entire quantity was added.</returns>
        public bool AddItem(ItemData item, int quantity = 1)
        {
            if (item == null || quantity <= 0)
                return false;

            int remaining = quantity;

            // First pass: stack onto existing slots with the same item
            for (int i = 0; i < slots.Length && remaining > 0; i++)
            {
                if (!slots[i].IsEmpty && slots[i].ItemData == item && !slots[i].IsFull)
                {
                    remaining = slots[i].AddQuantity(remaining);
                    OnInventoryChanged?.Invoke(i);
                }
            }

            // Second pass: place into empty slots
            for (int i = 0; i < slots.Length && remaining > 0; i++)
            {
                if (slots[i].IsEmpty)
                {
                    int toAdd = Mathf.Min(remaining, item.MaxStackSize);
                    slots[i] = new InventorySlot(item, toAdd);
                    remaining -= toAdd;
                    OnInventoryChanged?.Invoke(i);
                }
            }

            return remaining <= 0;
        }

        /// <summary>
        /// Removes a quantity of the item matching the given ID.
        /// </summary>
        /// <returns>True if the full quantity was removed.</returns>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0)
                return false;

            if (!HasItem(itemId, quantity))
                return false;

            int remaining = quantity;

            for (int i = 0; i < slots.Length && remaining > 0; i++)
            {
                if (!slots[i].IsEmpty && slots[i].ItemData.ID == itemId)
                {
                    int removed = slots[i].RemoveQuantity(remaining);
                    remaining -= removed;
                    OnInventoryChanged?.Invoke(i);
                }
            }

            return remaining <= 0;
        }

        /// <summary>
        /// Gets the inventory slot at the specified index.
        /// </summary>
        public InventorySlot GetItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length)
                return null;

            return slots[slotIndex];
        }

        /// <summary>
        /// Swaps the contents of two inventory slots.
        /// </summary>
        public void SwapSlots(int indexA, int indexB)
        {
            if (indexA < 0 || indexA >= slots.Length || indexB < 0 || indexB >= slots.Length)
                return;

            (slots[indexA], slots[indexB]) = (slots[indexB], slots[indexA]);
            OnInventoryChanged?.Invoke(indexA);
            OnInventoryChanged?.Invoke(indexB);
        }

        /// <summary>
        /// Selects a hotbar slot by index.
        /// </summary>
        public void SelectSlot(int index)
        {
            if (index < 0 || index >= hotbarSize)
                return;

            selectedSlotIndex = index;
            OnSlotSelected?.Invoke(index);
        }

        /// <summary>
        /// Checks whether the inventory contains at least the specified quantity of an item.
        /// </summary>
        public bool HasItem(string itemId, int quantity = 1)
        {
            return GetItemCount(itemId) >= quantity;
        }

        /// <summary>
        /// Returns the total quantity of the item with the given ID across all slots.
        /// </summary>
        public int GetItemCount(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return 0;

            int count = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsEmpty && slots[i].ItemData.ID == itemId)
                    count += slots[i].Quantity;
            }

            return count;
        }

        /// <summary>
        /// Clears all inventory slots.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Clear();
                OnInventoryChanged?.Invoke(i);
            }
        }
    }
}
