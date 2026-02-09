using System;
using System.Collections.Generic;
using UnityEngine;
using HavenwoodHollow.Crafting;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.Quests
{
    /// <summary>
    /// Tracks runtime progress for a single quest.
    /// </summary>
    [System.Serializable]
    public class QuestProgress
    {
        public string QuestId;
        public int[] ObjectiveCounts;
        public bool IsCompleted;
        public bool RewardsClaimed;

        public QuestProgress(QuestData quest)
        {
            QuestId = quest.ID;
            ObjectiveCounts = new int[quest.ObjectiveCount];
            IsCompleted = false;
            RewardsClaimed = false;
        }
    }

    /// <summary>
    /// Singleton manager for the quest system.
    /// Tracks active quests, objective progress, completion, and rewards.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quest Database")]
        [Tooltip("All quests available in the game")]
        [SerializeField] private QuestData[] allQuests;

        private Dictionary<string, QuestProgress> activeQuests;
        private HashSet<string> completedQuestIds;

        /// <summary>Fired when a new quest is accepted.</summary>
        public event Action<QuestData> OnQuestAccepted;

        /// <summary>Fired when a quest objective makes progress.</summary>
        public event Action<QuestData, int> OnObjectiveProgress;

        /// <summary>Fired when a quest is completed.</summary>
        public event Action<QuestData> OnQuestCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            activeQuests = new Dictionary<string, QuestProgress>();
            completedQuestIds = new HashSet<string>();
        }

        /// <summary>
        /// Accepts a quest, adding it to the active quest list.
        /// </summary>
        /// <returns>True if the quest was successfully accepted.</returns>
        public bool AcceptQuest(QuestData quest)
        {
            if (quest == null) return false;

            if (activeQuests.ContainsKey(quest.ID))
            {
                Debug.LogWarning($"[QuestManager] Quest '{quest.DisplayName}' is already active.");
                return false;
            }

            if (completedQuestIds.Contains(quest.ID))
            {
                Debug.LogWarning($"[QuestManager] Quest '{quest.DisplayName}' is already completed.");
                return false;
            }

            if (!ArePrerequisitesMet(quest))
            {
                Debug.LogWarning($"[QuestManager] Prerequisites not met for '{quest.DisplayName}'.");
                return false;
            }

            var progress = new QuestProgress(quest);
            activeQuests.Add(quest.ID, progress);

            OnQuestAccepted?.Invoke(quest);
            Debug.Log($"[QuestManager] Quest accepted: {quest.DisplayName}");

            return true;
        }

        /// <summary>
        /// Reports progress toward a quest objective.
        /// Call this when the player collects an item, kills an enemy, etc.
        /// </summary>
        /// <param name="objectiveType">The type of objective being progressed.</param>
        /// <param name="targetId">The target ID (item ID, enemy type, etc.).</param>
        /// <param name="count">Amount to add to progress.</param>
        public void ReportProgress(QuestObjectiveType objectiveType, string targetId, int count = 1)
        {
            if (string.IsNullOrEmpty(targetId) || count <= 0) return;

            foreach (var kvp in activeQuests)
            {
                QuestData quest = FindQuestById(kvp.Key);
                if (quest == null) continue;

                QuestProgress progress = kvp.Value;
                if (progress.IsCompleted) continue;

                for (int i = 0; i < quest.Objectives.Length; i++)
                {
                    var objective = quest.Objectives[i];

                    if (objective.type == objectiveType && objective.targetId == targetId)
                    {
                        int previousCount = progress.ObjectiveCounts[i];
                        progress.ObjectiveCounts[i] = Mathf.Min(
                            progress.ObjectiveCounts[i] + count,
                            objective.requiredCount
                        );

                        if (progress.ObjectiveCounts[i] != previousCount)
                        {
                            OnObjectiveProgress?.Invoke(quest, i);
                        }
                    }
                }

                CheckQuestCompletion(quest, progress);
            }
        }

        /// <summary>
        /// Gets the active progress for a quest.
        /// </summary>
        public QuestProgress GetProgress(string questId)
        {
            activeQuests.TryGetValue(questId, out QuestProgress progress);
            return progress;
        }

        /// <summary>
        /// Returns whether a quest has been completed.
        /// </summary>
        public bool IsQuestCompleted(string questId)
        {
            return completedQuestIds.Contains(questId);
        }

        /// <summary>
        /// Returns whether a quest is currently active.
        /// </summary>
        public bool IsQuestActive(string questId)
        {
            return activeQuests.ContainsKey(questId);
        }

        /// <summary>
        /// Returns all currently active quests.
        /// </summary>
        public List<QuestData> GetActiveQuests()
        {
            var result = new List<QuestData>();

            foreach (var kvp in activeQuests)
            {
                QuestData quest = FindQuestById(kvp.Key);
                if (quest != null)
                    result.Add(quest);
            }

            return result;
        }

        /// <summary>
        /// Checks whether all objectives of a quest are fulfilled.
        /// </summary>
        private void CheckQuestCompletion(QuestData quest, QuestProgress progress)
        {
            if (progress.IsCompleted) return;

            for (int i = 0; i < quest.Objectives.Length; i++)
            {
                if (progress.ObjectiveCounts[i] < quest.Objectives[i].requiredCount)
                    return;
            }

            progress.IsCompleted = true;
            OnQuestCompleted?.Invoke(quest);
            Debug.Log($"[QuestManager] Quest completed: {quest.DisplayName}");
        }

        /// <summary>
        /// Claims rewards for a completed quest.
        /// </summary>
        /// <returns>True if rewards were successfully claimed.</returns>
        public bool ClaimRewards(string questId)
        {
            if (!activeQuests.TryGetValue(questId, out QuestProgress progress))
                return false;

            if (!progress.IsCompleted || progress.RewardsClaimed)
                return false;

            QuestData quest = FindQuestById(questId);
            if (quest == null || quest.Rewards == null) return false;

            var inventory = Inventory.InventoryManager.Instance;

            for (int i = 0; i < quest.Rewards.Length; i++)
            {
                var reward = quest.Rewards[i];

                if (reward.rewardItem != null && inventory != null)
                {
                    inventory.AddItem(reward.rewardItem, reward.quantity);
                }

                if (reward.recipeUnlock != null)
                {
                    var crafting = Crafting.CraftingManager.Instance;
                    if (crafting != null)
                    {
                        crafting.UnlockRecipe(reward.recipeUnlock.ID);
                    }
                }
            }

            progress.RewardsClaimed = true;
            completedQuestIds.Add(questId);
            activeQuests.Remove(questId);

            return true;
        }

        /// <summary>
        /// Checks whether all prerequisite quests have been completed.
        /// </summary>
        private bool ArePrerequisitesMet(QuestData quest)
        {
            if (quest.Prerequisites == null || quest.Prerequisites.Length == 0)
                return true;

            for (int i = 0; i < quest.Prerequisites.Length; i++)
            {
                if (quest.Prerequisites[i] != null &&
                    !completedQuestIds.Contains(quest.Prerequisites[i].ID))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds a quest by its ID.
        /// </summary>
        public QuestData FindQuestById(string questId)
        {
            if (allQuests == null || string.IsNullOrEmpty(questId)) return null;

            for (int i = 0; i < allQuests.Length; i++)
            {
                if (allQuests[i] != null && allQuests[i].ID == questId)
                    return allQuests[i];
            }

            return null;
        }
    }
}
