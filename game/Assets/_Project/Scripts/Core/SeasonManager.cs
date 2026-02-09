using System;
using UnityEngine;
using HavenwoodHollow.Farming;

namespace HavenwoodHollow.Core
{
    /// <summary>
    /// Manages seasonal transitions and day tracking for Havenwood Hollow.
    /// Listens to GameManager.OnSleepTriggered to advance the day counter.
    /// Notifies CropManager when the season changes so out-of-season crops are removed.
    /// Reference: Plan Section 9 Phase 2 - The Simulation Loop.
    /// </summary>
    public class SeasonManager : MonoBehaviour
    {
        public static SeasonManager Instance { get; private set; }

        [Header("Season Settings")]
        [Tooltip("Number of days per season before transitioning")]
        [SerializeField] private int daysPerSeason = 28;

        [Tooltip("The current season")]
        [SerializeField] private Season currentSeason = Season.Spring;

        [Header("Day Tracking")]
        [Tooltip("Current day within the season (1-based)")]
        [SerializeField] private int currentDay = 1;

        [Tooltip("Total number of years elapsed")]
        [SerializeField] private int currentYear = 1;

        public Season CurrentSeason => currentSeason;
        public int CurrentDay => currentDay;
        public int DaysPerSeason => daysPerSeason;
        public int CurrentYear => currentYear;

        /// <summary>Fired when the season changes to a new value.</summary>
        public event Action<Season> OnSeasonChanged;

        /// <summary>Fired every day with the new day number (1-based).</summary>
        public event Action<int> OnDayChanged;

        /// <summary>Fired when the year rolls over.</summary>
        public event Action OnNewYear;

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

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSleepTriggered += AdvanceDay;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSleepTriggered -= AdvanceDay;
            }
        }

        /// <summary>
        /// Advances the calendar by one day. Triggers season and year
        /// transitions when the day count exceeds daysPerSeason.
        /// </summary>
        public void AdvanceDay()
        {
            currentDay++;

            if (currentDay > daysPerSeason)
            {
                currentDay = 1;
                AdvanceSeason();
            }

            OnDayChanged?.Invoke(currentDay);
        }

        /// <summary>
        /// Transitions to the next season in order: Spring → Summer → Fall → Winter → Spring.
        /// Notifies CropManager so crops that cannot survive the new season are removed.
        /// </summary>
        private void AdvanceSeason()
        {
            switch (currentSeason)
            {
                case Season.Spring:
                    currentSeason = Season.Summer;
                    break;
                case Season.Summer:
                    currentSeason = Season.Fall;
                    break;
                case Season.Fall:
                    currentSeason = Season.Winter;
                    break;
                case Season.Winter:
                    currentSeason = Season.Spring;
                    currentYear++;
                    OnNewYear?.Invoke();
                    break;
            }

            Debug.Log($"[SeasonManager] Season changed to {currentSeason} (Year {currentYear})");

            if (CropManager.Instance != null)
            {
                CropManager.Instance.SetSeason(currentSeason);
            }

            OnSeasonChanged?.Invoke(currentSeason);
        }

        /// <summary>
        /// Sets the season directly. Useful when loading a save file.
        /// </summary>
        public void SetSeason(Season season)
        {
            currentSeason = season;

            if (CropManager.Instance != null)
            {
                CropManager.Instance.SetSeason(currentSeason);
            }

            OnSeasonChanged?.Invoke(currentSeason);
        }

        /// <summary>
        /// Sets the day counter directly. Useful when loading a save file.
        /// </summary>
        public void SetDay(int day)
        {
            currentDay = Mathf.Clamp(day, 1, daysPerSeason);
            OnDayChanged?.Invoke(currentDay);
        }

        /// <summary>
        /// Sets the year counter directly. Useful when loading a save file.
        /// </summary>
        public void SetYear(int year)
        {
            currentYear = Mathf.Max(1, year);
        }
    }
}
