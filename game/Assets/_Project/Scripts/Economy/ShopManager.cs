using System;
using System.Collections.Generic;
using UnityEngine;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.Economy
{
    /// <summary>
    /// Singleton manager for the shop/trading system.
    /// Handles buying items from shops and selling player items.
    /// Integrates with InventoryManager for item transfers and gold tracking.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }

        [Header("Player Economy")]
        [SerializeField] private int playerGold = 500;

        [Header("Shop Database")]
        [Tooltip("All shops available in the game")]
        [SerializeField] private ShopData[] allShops;

        /// <summary>Current shop being browsed (null if no shop is open).</summary>
        private ShopData currentShop;

        /// <summary>Tracks remaining daily stock per shop per item index.</summary>
        private Dictionary<string, int[]> dailyStockRemaining;

        /// <summary>Fired when the player's gold changes. Parameter is the new total.</summary>
        public event Action<int> OnGoldChanged;

        /// <summary>Fired when the player opens a shop.</summary>
        public event Action<ShopData> OnShopOpened;

        /// <summary>Fired when the player closes a shop.</summary>
        public event Action OnShopClosed;

        /// <summary>Fired when a purchase is completed.</summary>
        public event Action<ItemData, int> OnItemPurchased;

        /// <summary>Fired when a sale is completed.</summary>
        public event Action<ItemData, int> OnItemSold;

        /// <summary>The player's current gold.</summary>
        public int PlayerGold => playerGold;

        /// <summary>The shop currently open, or null.</summary>
        public ShopData CurrentShop => currentShop;

        /// <summary>Whether a shop is currently open.</summary>
        public bool IsShopOpen => currentShop != null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            dailyStockRemaining = new Dictionary<string, int[]>();
        }

        /// <summary>
        /// Resets daily stock for all shops. Call this at the start of each day.
        /// </summary>
        public void ResetDailyStock()
        {
            dailyStockRemaining.Clear();

            if (allShops == null) return;

            for (int s = 0; s < allShops.Length; s++)
            {
                if (allShops[s] == null || allShops[s].ItemsForSale == null) continue;

                int[] stock = new int[allShops[s].ItemsForSale.Length];
                for (int i = 0; i < stock.Length; i++)
                {
                    stock[i] = allShops[s].ItemsForSale[i].dailyStock;
                }

                dailyStockRemaining[allShops[s].ID] = stock;
            }
        }

        /// <summary>
        /// Opens a shop for browsing.
        /// </summary>
        public void OpenShop(ShopData shop)
        {
            if (shop == null) return;

            currentShop = shop;
            OnShopOpened?.Invoke(shop);
            Debug.Log($"[ShopManager] Opened shop: {shop.ShopName}");
        }

        /// <summary>
        /// Closes the currently open shop.
        /// </summary>
        public void CloseShop()
        {
            currentShop = null;
            OnShopClosed?.Invoke();
        }

        /// <summary>
        /// Attempts to buy an item from the current shop.
        /// </summary>
        /// <param name="itemIndex">Index of the item in the shop's inventory.</param>
        /// <param name="quantity">Number of items to buy.</param>
        /// <returns>True if the purchase was successful.</returns>
        public bool BuyItem(int itemIndex, int quantity = 1)
        {
            if (currentShop == null || currentShop.ItemsForSale == null) return false;
            if (itemIndex < 0 || itemIndex >= currentShop.ItemsForSale.Length) return false;
            if (quantity <= 0) return false;

            ShopItem shopItem = currentShop.ItemsForSale[itemIndex];
            if (shopItem.item == null) return false;

            // Check stock
            if (shopItem.dailyStock >= 0)
            {
                int remaining = GetRemainingStock(currentShop.ID, itemIndex);
                if (remaining >= 0 && remaining < quantity)
                {
                    Debug.LogWarning($"[ShopManager] Not enough stock for '{shopItem.item.DisplayName}'.");
                    return false;
                }
            }

            // Check gold
            int totalCost = shopItem.GetPrice() * quantity;
            if (playerGold < totalCost)
            {
                Debug.LogWarning($"[ShopManager] Not enough gold. Need {totalCost}, have {playerGold}.");
                return false;
            }

            // Check inventory space
            var inventory = Inventory.InventoryManager.Instance;
            if (inventory == null) return false;

            if (!inventory.AddItem(shopItem.item, quantity))
            {
                Debug.LogWarning($"[ShopManager] Inventory full, cannot buy '{shopItem.item.DisplayName}'.");
                return false;
            }

            // Deduct gold
            playerGold -= totalCost;
            OnGoldChanged?.Invoke(playerGold);

            // Reduce stock
            if (shopItem.dailyStock >= 0)
            {
                ReduceStock(currentShop.ID, itemIndex, quantity);
            }

            OnItemPurchased?.Invoke(shopItem.item, quantity);
            Debug.Log($"[ShopManager] Purchased {quantity}x {shopItem.item.DisplayName} for {totalCost} gold.");

            return true;
        }

        /// <summary>
        /// Sells an item from the player's inventory to the current shop.
        /// </summary>
        /// <param name="itemId">ID of the item to sell.</param>
        /// <param name="quantity">Number to sell.</param>
        /// <returns>True if the sale was successful.</returns>
        public bool SellItem(string itemId, int quantity = 1)
        {
            if (currentShop == null) return false;
            if (string.IsNullOrEmpty(itemId) || quantity <= 0) return false;

            var inventory = Inventory.InventoryManager.Instance;
            if (inventory == null) return false;

            if (!inventory.HasItem(itemId, quantity))
            {
                Debug.LogWarning($"[ShopManager] Player doesn't have {quantity}x item '{itemId}'.");
                return false;
            }

            // Find the item to get sell price
            ItemData item = FindItemInInventory(itemId);
            if (item == null) return false;

            int sellValue = Mathf.RoundToInt(item.SellPrice * currentShop.SellPriceMultiplier) * quantity;

            inventory.RemoveItem(itemId, quantity);

            playerGold += sellValue;
            OnGoldChanged?.Invoke(playerGold);

            OnItemSold?.Invoke(item, quantity);
            Debug.Log($"[ShopManager] Sold {quantity}x {item.DisplayName} for {sellValue} gold.");

            return true;
        }

        /// <summary>
        /// Adds gold to the player's balance.
        /// </summary>
        public void AddGold(int amount)
        {
            if (amount <= 0) return;

            playerGold += amount;
            OnGoldChanged?.Invoke(playerGold);
        }

        /// <summary>
        /// Removes gold from the player's balance.
        /// </summary>
        /// <returns>True if the player had enough gold.</returns>
        public bool SpendGold(int amount)
        {
            if (amount <= 0) return false;
            if (playerGold < amount) return false;

            playerGold -= amount;
            OnGoldChanged?.Invoke(playerGold);
            return true;
        }

        /// <summary>
        /// Gets remaining daily stock for a shop item.
        /// Returns -1 for unlimited stock items.
        /// </summary>
        public int GetRemainingStock(string shopId, int itemIndex)
        {
            if (dailyStockRemaining.TryGetValue(shopId, out int[] stock))
            {
                if (itemIndex >= 0 && itemIndex < stock.Length)
                    return stock[itemIndex];
            }

            return -1;
        }

        private void ReduceStock(string shopId, int itemIndex, int quantity)
        {
            if (dailyStockRemaining.TryGetValue(shopId, out int[] stock))
            {
                if (itemIndex >= 0 && itemIndex < stock.Length)
                {
                    stock[itemIndex] = Mathf.Max(0, stock[itemIndex] - quantity);
                }
            }
        }

        /// <summary>
        /// Finds an ItemData reference from the player's inventory by ID.
        /// </summary>
        private ItemData FindItemInInventory(string itemId)
        {
            var inventory = Inventory.InventoryManager.Instance;
            if (inventory == null) return null;

            for (int i = 0; i < inventory.InventorySize; i++)
            {
                var slot = inventory.GetItem(i);
                if (slot != null && !slot.IsEmpty && slot.ItemData.ID == itemId)
                    return slot.ItemData;
            }

            return null;
        }

        /// <summary>
        /// Finds a shop by its ID.
        /// </summary>
        public ShopData FindShopById(string shopId)
        {
            if (allShops == null || string.IsNullOrEmpty(shopId)) return null;

            for (int i = 0; i < allShops.Length; i++)
            {
                if (allShops[i] != null && allShops[i].ID == shopId)
                    return allShops[i];
            }

            return null;
        }
    }
}
