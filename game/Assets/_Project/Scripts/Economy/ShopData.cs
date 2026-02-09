using UnityEngine;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.Economy
{
    /// <summary>
    /// Represents an item available for sale in a shop, with optional stock limits.
    /// </summary>
    [System.Serializable]
    public struct ShopItem
    {
        [Tooltip("The item for sale")]
        public ItemData item;

        [Tooltip("Custom buy price override (-1 to use item's default buyPrice)")]
        public int priceOverride;

        [Tooltip("Maximum stock available per day (-1 for unlimited)")]
        public int dailyStock;

        /// <summary>
        /// Returns the effective buy price, using the override if set.
        /// </summary>
        public int GetPrice()
        {
            return priceOverride >= 0 ? priceOverride : (item != null ? item.BuyPrice : 0);
        }
    }

    /// <summary>
    /// ScriptableObject defining a shop's inventory and properties.
    /// Each NPC merchant can reference one of these.
    /// </summary>
    [CreateAssetMenu(fileName = "NewShop", menuName = "HavenwoodHollow/Shop Data")]
    public class ShopData : ScriptableObject
    {
        [Header("Identification")]
        [SerializeField] private string id;
        [SerializeField] private string shopName;
        [TextArea(2, 3)]
        [SerializeField] private string description;

        [Header("Merchant")]
        [Tooltip("NPC ID of the shopkeeper")]
        [SerializeField] private string merchantNpcId;

        [Header("Inventory")]
        [SerializeField] private ShopItem[] itemsForSale;

        [Header("Buy-Back")]
        [Tooltip("Multiplier applied to item sellPrice when player sells to this shop (0.0-1.0)")]
        [Range(0f, 1f)]
        [SerializeField] private float sellPriceMultiplier = 1f;

        [Header("Availability")]
        [Tooltip("Season when this shop operates (None = all seasons)")]
        [SerializeField] private Farming.Season availableSeason = Farming.Season.None;

        #region Properties

        public string ID => id;
        public string ShopName => shopName;
        public string Description => description;
        public string MerchantNpcId => merchantNpcId;
        public ShopItem[] ItemsForSale => itemsForSale;
        public float SellPriceMultiplier => sellPriceMultiplier;
        public Farming.Season AvailableSeason => availableSeason;

        #endregion
    }
}
