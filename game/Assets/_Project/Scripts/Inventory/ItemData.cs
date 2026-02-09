using UnityEngine;

namespace HavenwoodHollow.Inventory
{
    /// <summary>
    /// ScriptableObject defining static data for any item in the game.
    /// Used by crops, tools, consumables, and crafting materials.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "HavenwoodHollow/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Identification")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea(2, 4)]
        [SerializeField] private string description;

        [Header("Visual")]
        [SerializeField] private Sprite icon;

        [Header("Stacking")]
        [SerializeField] private int maxStackSize = 99;

        [Header("Value")]
        [SerializeField] private int buyPrice;
        [SerializeField] private int sellPrice;

        [Header("Category")]
        [SerializeField] private ItemCategory category;

        public string ID => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public int MaxStackSize => maxStackSize;
        public int BuyPrice => buyPrice;
        public int SellPrice => sellPrice;
        public ItemCategory Category => category;
    }

    public enum ItemCategory
    {
        None,
        Seed,
        Crop,
        Tool,
        Consumable,
        Material,
        Furniture,
        Quest,
        Monster
    }
}
