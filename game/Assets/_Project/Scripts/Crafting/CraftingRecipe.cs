using UnityEngine;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.Crafting
{
    /// <summary>
    /// Represents a single ingredient requirement for a crafting recipe.
    /// </summary>
    [System.Serializable]
    public struct CraftingIngredient
    {
        [Tooltip("The item required")]
        public ItemData item;

        [Tooltip("Quantity needed")]
        public int quantity;
    }

    /// <summary>
    /// Defines the crafting station type required for a recipe.
    /// </summary>
    public enum CraftingStation
    {
        None,
        Workbench,
        Forge,
        AlchemyTable,
        Loom,
        OccultAltar
    }

    /// <summary>
    /// ScriptableObject defining a crafting recipe.
    /// Specifies ingredients, output, required station, and unlock conditions.
    /// Reference: Plan Section 9 Phase 2 - Build Inventory UI and ScriptableObject database.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "HavenwoodHollow/Crafting Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        [Header("Identification")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea(2, 4)]
        [SerializeField] private string description;

        [Header("Requirements")]
        [Tooltip("Items consumed when crafting")]
        [SerializeField] private CraftingIngredient[] ingredients;

        [Tooltip("Crafting station required (None = hand-craft)")]
        [SerializeField] private CraftingStation requiredStation;

        [Header("Output")]
        [Tooltip("Item produced by this recipe")]
        [SerializeField] private ItemData outputItem;

        [Tooltip("Quantity produced per craft")]
        [SerializeField] private int outputQuantity = 1;

        [Header("Unlock")]
        [Tooltip("Whether this recipe is available from the start")]
        [SerializeField] private bool unlockedByDefault = true;

        #region Properties

        public string ID => id;
        public string DisplayName => displayName;
        public string Description => description;
        public CraftingIngredient[] Ingredients => ingredients;
        public CraftingStation RequiredStation => requiredStation;
        public ItemData OutputItem => outputItem;
        public int OutputQuantity => outputQuantity;
        public bool UnlockedByDefault => unlockedByDefault;

        #endregion

        /// <summary>
        /// Checks whether the player has all required ingredients in their inventory.
        /// </summary>
        public bool CanCraft(InventoryManager inventory)
        {
            if (inventory == null || ingredients == null)
                return false;

            for (int i = 0; i < ingredients.Length; i++)
            {
                if (ingredients[i].item == null)
                    continue;

                if (!inventory.HasItem(ingredients[i].item.ID, ingredients[i].quantity))
                    return false;
            }

            return true;
        }
    }
}
