using UnityEngine;

namespace HavenwoodHollow.Farming
{
    /// <summary>
    /// Defines valid growing seasons for crops.
    /// Uses enum flags so a crop can grow in multiple seasons.
    /// Reference: Plan Section 3.2.1, Table 1 - Seasons field.
    /// </summary>
    [System.Flags]
    public enum Season
    {
        None   = 0,
        Spring = 1 << 0,
        Summer = 1 << 1,
        Fall   = 1 << 2,
        Winter = 1 << 3
    }

    /// <summary>
    /// ScriptableObject defining static data for a crop type.
    /// Reference: Plan Section 3.2.1, Table 1 - Proposed Unity Data Structure for Crops.
    ///
    /// Growth Phases: Defined as an array of integers representing days.
    /// E.g., [2, 3, 2, 3, 3] means Phase 1 lasts 2 days, Phase 2 lasts 3 days, etc.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCrop", menuName = "HavenwoodHollow/Crop Data")]
    public class CropData : ScriptableObject
    {
        [Header("Identification")]
        [Tooltip("Unique identifier (e.g., 'crop_parsnip')")]
        [SerializeField] private string id;

        [Tooltip("Display name for the crop")]
        [SerializeField] private string displayName;

        [Header("Growth")]
        [Tooltip("Days required for each visual stage")]
        [SerializeField] private int[] phaseDurations;

        [Tooltip("True if crop persists after harvest")]
        [SerializeField] private bool regrows;

        [Tooltip("The phase index to revert to after harvest")]
        [SerializeField] private int regrowPhaseIndex;

        [Header("Seasons")]
        [Tooltip("Valid growing seasons for this crop")]
        [SerializeField] private Season seasons;

        [Header("Harvest")]
        [Tooltip("Item yielded on harvest")]
        [SerializeField] private ItemData harvestItem;

        [Tooltip("Minimum quantity harvested")]
        [SerializeField] private int minHarvestQuantity = 1;

        [Tooltip("Maximum quantity harvested (for random yield)")]
        [SerializeField] private int maxHarvestQuantity = 1;

        [Header("Visuals")]
        [Tooltip("Sprites for each growth phase (must match phaseDurations length + 1 for harvest)")]
        [SerializeField] private Sprite[] phaseSprites;

        #region Public Properties

        public string ID => id;
        public string DisplayName => displayName;
        public int[] PhaseDurations => phaseDurations;
        public int TotalGrowthDays
        {
            get
            {
                int total = 0;
                for (int i = 0; i < phaseDurations.Length; i++)
                    total += phaseDurations[i];
                return total;
            }
        }
        public bool Regrows => regrows;
        public int RegrowPhaseIndex => regrowPhaseIndex;
        public Season Seasons => seasons;
        public ItemData HarvestItem => harvestItem;
        public int MinHarvestQuantity => minHarvestQuantity;
        public int MaxHarvestQuantity => maxHarvestQuantity;
        public Sprite[] PhaseSprites => phaseSprites;
        public int PhaseCount => phaseDurations != null ? phaseDurations.Length : 0;

        #endregion

        /// <summary>
        /// Checks whether this crop can grow in the given season.
        /// </summary>
        public bool CanGrowInSeason(Season season)
        {
            return (seasons & season) != 0;
        }

        /// <summary>
        /// Calculates days saved by Speed-Gro fertilizer.
        /// Formula: DaysSaved = ceil(TotalDays * modifier).
        /// Reference: Plan Section 3.2.1 - Fertilizer Math.
        /// </summary>
        public int CalculateFertilizerDaysSaved(float modifier)
        {
            return Mathf.CeilToInt(TotalGrowthDays * modifier);
        }
    }
}
