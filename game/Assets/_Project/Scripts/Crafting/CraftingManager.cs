using System;
using System.Collections.Generic;
using UnityEngine;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.Crafting
{
    /// <summary>
    /// Singleton manager for the crafting system.
    /// Handles recipe discovery, crafting execution, and station-based crafting.
    /// Reference: Plan Section 9 Phase 2 - ScriptableObject database for items.
    /// </summary>
    public class CraftingManager : MonoBehaviour
    {
        public static CraftingManager Instance { get; private set; }

        [Header("Recipe Database")]
        [Tooltip("All recipes available in the game")]
        [SerializeField] private CraftingRecipe[] allRecipes;

        private HashSet<string> unlockedRecipeIds;

        /// <summary>Fired when a recipe is successfully crafted. Parameter is the recipe.</summary>
        public event Action<CraftingRecipe> OnItemCrafted;

        /// <summary>Fired when a new recipe is unlocked. Parameter is the recipe.</summary>
        public event Action<CraftingRecipe> OnRecipeUnlocked;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeUnlockedRecipes();
        }

        private void InitializeUnlockedRecipes()
        {
            unlockedRecipeIds = new HashSet<string>();

            if (allRecipes == null) return;

            for (int i = 0; i < allRecipes.Length; i++)
            {
                if (allRecipes[i] != null && allRecipes[i].UnlockedByDefault)
                {
                    unlockedRecipeIds.Add(allRecipes[i].ID);
                }
            }
        }

        /// <summary>
        /// Returns all recipes the player has unlocked.
        /// </summary>
        public List<CraftingRecipe> GetUnlockedRecipes()
        {
            var result = new List<CraftingRecipe>();

            if (allRecipes == null) return result;

            for (int i = 0; i < allRecipes.Length; i++)
            {
                if (allRecipes[i] != null && unlockedRecipeIds.Contains(allRecipes[i].ID))
                {
                    result.Add(allRecipes[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all unlocked recipes for a specific crafting station.
        /// </summary>
        public List<CraftingRecipe> GetRecipesForStation(CraftingStation station)
        {
            var result = new List<CraftingRecipe>();

            if (allRecipes == null) return result;

            for (int i = 0; i < allRecipes.Length; i++)
            {
                if (allRecipes[i] != null &&
                    allRecipes[i].RequiredStation == station &&
                    unlockedRecipeIds.Contains(allRecipes[i].ID))
                {
                    result.Add(allRecipes[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Attempts to craft a recipe, consuming ingredients and adding the output to inventory.
        /// </summary>
        /// <returns>True if crafting was successful.</returns>
        public bool TryCraft(CraftingRecipe recipe)
        {
            if (recipe == null) return false;

            var inventory = InventoryManager.Instance;
            if (inventory == null) return false;

            if (!unlockedRecipeIds.Contains(recipe.ID))
            {
                Debug.LogWarning($"[CraftingManager] Recipe '{recipe.DisplayName}' is not unlocked.");
                return false;
            }

            if (!recipe.CanCraft(inventory))
            {
                Debug.LogWarning($"[CraftingManager] Missing ingredients for '{recipe.DisplayName}'.");
                return false;
            }

            // Consume ingredients
            for (int i = 0; i < recipe.Ingredients.Length; i++)
            {
                var ingredient = recipe.Ingredients[i];
                if (ingredient.item != null)
                {
                    inventory.RemoveItem(ingredient.item.ID, ingredient.quantity);
                }
            }

            // Add output
            bool added = inventory.AddItem(recipe.OutputItem, recipe.OutputQuantity);

            if (!added)
            {
                Debug.LogWarning($"[CraftingManager] Inventory full, could not add '{recipe.OutputItem.DisplayName}'.");
                // Ingredients already consumed — design decision: refund or drop on ground
            }

            OnItemCrafted?.Invoke(recipe);
            Debug.Log($"[CraftingManager] Crafted {recipe.OutputQuantity}x {recipe.OutputItem.DisplayName}");

            return true;
        }

        /// <summary>
        /// Unlocks a recipe by ID, allowing the player to craft it.
        /// </summary>
        public void UnlockRecipe(string recipeId)
        {
            if (string.IsNullOrEmpty(recipeId)) return;

            if (unlockedRecipeIds.Add(recipeId))
            {
                CraftingRecipe recipe = FindRecipeById(recipeId);
                if (recipe != null)
                {
                    OnRecipeUnlocked?.Invoke(recipe);
                    Debug.Log($"[CraftingManager] Recipe unlocked: {recipe.DisplayName}");
                }
            }
        }

        /// <summary>
        /// Checks whether a recipe has been unlocked.
        /// </summary>
        public bool IsRecipeUnlocked(string recipeId)
        {
            return unlockedRecipeIds.Contains(recipeId);
        }

        /// <summary>
        /// Finds a recipe by its ID.
        /// </summary>
        public CraftingRecipe FindRecipeById(string recipeId)
        {
            if (allRecipes == null || string.IsNullOrEmpty(recipeId)) return null;

            for (int i = 0; i < allRecipes.Length; i++)
            {
                if (allRecipes[i] != null && allRecipes[i].ID == recipeId)
                    return allRecipes[i];
            }

            return null;
        }
    }
}
