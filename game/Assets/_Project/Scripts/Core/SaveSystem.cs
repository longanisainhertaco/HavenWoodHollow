using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using HavenwoodHollow.Player;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.Core
{
    /// <summary>
    /// JSON-based save/load system using Application.persistentDataPath.
    /// Serializes dynamic game state (player position, calendar, inventory)
    /// via Unity's JsonUtility.
    /// Reference: Plan Section 3.2 - JSON serialization for dynamic save data.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        /// <summary>Fired after a game is successfully saved.</summary>
        public event Action OnGameSaved;

        /// <summary>Fired after a game is successfully loaded.</summary>
        public event Action OnGameLoaded;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Saves the current game state to a JSON file.
        /// </summary>
        /// <param name="slotName">Name of the save slot (used as file name).</param>
        public void SaveGame(string slotName)
        {
            SaveData data = GatherSaveData();
            string json = JsonUtility.ToJson(data, true);
            string path = GetSavePath(slotName);

            try
            {
                File.WriteAllText(path, json);
                Debug.Log($"[SaveSystem] Game saved to {path}");
                OnGameSaved?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to save game: {e.Message}");
            }
        }

        /// <summary>
        /// Loads game state from a JSON file and applies it.
        /// </summary>
        /// <param name="slotName">Name of the save slot to load.</param>
        public void LoadGame(string slotName)
        {
            string path = GetSavePath(slotName);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"[SaveSystem] Save file not found: {path}");
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                ApplySaveData(data);
                Debug.Log($"[SaveSystem] Game loaded from {path}");
                OnGameLoaded?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to load game: {e.Message}");
            }
        }

        /// <summary>
        /// Deletes a save file from disk.
        /// </summary>
        /// <param name="slotName">Name of the save slot to delete.</param>
        public void DeleteSave(string slotName)
        {
            string path = GetSavePath(slotName);

            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"[SaveSystem] Save deleted: {path}");
            }
        }

        /// <summary>
        /// Checks whether a save file exists for the given slot.
        /// </summary>
        public bool SaveExists(string slotName)
        {
            return File.Exists(GetSavePath(slotName));
        }

        /// <summary>
        /// Builds the full file path for a save slot.
        /// </summary>
        private string GetSavePath(string slotName)
        {
            return Path.Combine(Application.persistentDataPath, slotName + ".json");
        }

        /// <summary>
        /// Gathers current game state from the active managers.
        /// Player and inventory fields are populated when their managers are implemented.
        /// </summary>
        private SaveData GatherSaveData()
        {
            SaveData data = new SaveData();

            if (SeasonManager.Instance != null)
            {
                data.currentDay = SeasonManager.Instance.CurrentDay;
                data.currentSeason = (int)SeasonManager.Instance.CurrentSeason;
                data.currentYear = SeasonManager.Instance.CurrentYear;
            }

            if (TimeManager.Instance != null)
            {
                data.currentTimeOfDay = TimeManager.Instance.CurrentHour;
            }

            var player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                data.playerPosition = player.transform.position;

                var stats = player.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    data.playerHealth = stats.CurrentHealth;
                    data.playerStamina = stats.CurrentStamina;
                }
            }

            if (InventoryManager.Instance != null)
            {
                data.inventoryData.Clear();
                for (int i = 0; i < InventoryManager.Instance.InventorySize; i++)
                {
                    InventorySlot slot = InventoryManager.Instance.GetItem(i);
                    if (slot != null && !slot.IsEmpty)
                    {
                        data.inventoryData.Add(new InventorySaveSlot
                        {
                            itemId = slot.ItemData.ID,
                            quantity = slot.Quantity,
                            slotIndex = i
                        });
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Applies loaded data to the active managers.
        /// Player and inventory restoration added when their managers are implemented.
        /// </summary>
        private void ApplySaveData(SaveData data)
        {
            if (SeasonManager.Instance != null)
            {
                SeasonManager.Instance.SetSeason((Farming.Season)data.currentSeason);
                SeasonManager.Instance.SetDay(data.currentDay);
                SeasonManager.Instance.SetYear(data.currentYear);
            }

            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.SetTime(data.currentTimeOfDay);
            }

            var player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.transform.position = data.playerPosition;

                var stats = player.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.SetHealth(data.playerHealth);
                    stats.SetStamina(data.playerStamina);
                }
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.Clear();
                // Note: Inventory items are identified by string ID.
                // Actual ItemData ScriptableObject lookup requires a runtime catalog.
                // When an item catalog is implemented, iterate data.inventoryData
                // and call InventoryManager.Instance.AddItem(catalogItem, slot.quantity)
                // for each saved slot.
            }
        }
    }

    /// <summary>
    /// Serializable container for all persistent game state.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        [Header("Player")]
        public Vector3 playerPosition;
        public float playerHealth = 100f;
        public float playerStamina = 100f;

        [Header("Calendar")]
        public int currentDay = 1;
        public int currentSeason;
        public int currentYear = 1;

        [Header("Time")]
        public float currentTimeOfDay;

        [Header("Inventory")]
        public List<InventorySaveSlot> inventoryData = new List<InventorySaveSlot>();
    }

    /// <summary>
    /// Lightweight struct representing one inventory slot in a save file.
    /// </summary>
    [Serializable]
    public struct InventorySaveSlot
    {
        [Tooltip("Unique item identifier (e.g., 'crop_parsnip')")]
        public string itemId;

        [Tooltip("Stack count")]
        public int quantity;

        [Tooltip("Inventory slot position")]
        public int slotIndex;
    }
}
