using UnityEngine;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.Quests
{
    /// <summary>
    /// Defines the type of objective a quest can have.
    /// </summary>
    public enum QuestObjectiveType
    {
        Collect,
        Deliver,
        Kill,
        Talk,
        Craft,
        Harvest,
        Breed,
        Explore,
        Survive
    }

    /// <summary>
    /// Defines the quest category for UI filtering.
    /// </summary>
    public enum QuestCategory
    {
        Main,
        Side,
        Bounty,
        Daily
    }

    /// <summary>
    /// Represents a single objective within a quest.
    /// </summary>
    [System.Serializable]
    public struct QuestObjective
    {
        [Tooltip("Description shown to the player")]
        public string description;

        [Tooltip("Type of objective")]
        public QuestObjectiveType type;

        [Tooltip("Target ID (item ID, NPC ID, enemy type, location name, etc.)")]
        public string targetId;

        [Tooltip("Quantity required to complete this objective")]
        public int requiredCount;
    }

    /// <summary>
    /// Represents a reward given upon quest completion.
    /// </summary>
    [System.Serializable]
    public struct QuestReward
    {
        [Tooltip("Item rewarded (null for gold-only rewards)")]
        public ItemData rewardItem;

        [Tooltip("Quantity of the item")]
        public int quantity;

        [Tooltip("Gold rewarded")]
        public int goldAmount;

        [Tooltip("Experience points rewarded")]
        public int experienceAmount;

        [Tooltip("Recipe unlocked upon completion (optional)")]
        public Crafting.CraftingRecipe recipeUnlock;
    }

    /// <summary>
    /// ScriptableObject defining a quest with objectives, rewards, and prerequisites.
    /// Supports multi-objective quests and quest chains via prerequisites.
    /// </summary>
    [CreateAssetMenu(fileName = "NewQuest", menuName = "HavenwoodHollow/Quest Data")]
    public class QuestData : ScriptableObject
    {
        [Header("Identification")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea(2, 5)]
        [SerializeField] private string description;

        [Header("Category")]
        [SerializeField] private QuestCategory category;

        [Header("Quest Giver")]
        [Tooltip("NPC ID who gives this quest")]
        [SerializeField] private string questGiverId;

        [Header("Objectives")]
        [SerializeField] private QuestObjective[] objectives;

        [Header("Rewards")]
        [SerializeField] private QuestReward[] rewards;

        [Header("Prerequisites")]
        [Tooltip("Quests that must be completed before this one is available")]
        [SerializeField] private QuestData[] prerequisites;

        [Header("Availability")]
        [Tooltip("Minimum day number before this quest becomes available")]
        [SerializeField] private int minimumDay;

        [Tooltip("Season required (None = any season)")]
        [SerializeField] private Farming.Season requiredSeason = Farming.Season.None;

        #region Properties

        public string ID => id;
        public string DisplayName => displayName;
        public string Description => description;
        public QuestCategory Category => category;
        public string QuestGiverId => questGiverId;
        public QuestObjective[] Objectives => objectives;
        public QuestReward[] Rewards => rewards;
        public QuestData[] Prerequisites => prerequisites;
        public int MinimumDay => minimumDay;
        public Farming.Season RequiredSeason => requiredSeason;
        public int ObjectiveCount => objectives != null ? objectives.Length : 0;

        #endregion
    }
}
