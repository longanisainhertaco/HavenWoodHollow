using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HavenwoodHollow.Core;
using HavenwoodHollow.Farming;
using HavenwoodHollow.Player;

namespace HavenwoodHollow.UI
{
    /// <summary>
    /// Singleton manager for the main HUD overlay.
    /// Displays health, stamina, time, day/season info, hotbar, and interaction prompts.
    /// Reference: Plan Section 9 Phase 2 - UI (using standard Unity UI for compatibility).
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager Instance { get; private set; }

        #region Serialized Fields

        [Header("Health & Stamina")]
        [Tooltip("Fill image representing the player's current health")]
        [SerializeField] private Image healthBar;
        [Tooltip("Fill image representing the player's current stamina")]
        [SerializeField] private Image staminaBar;

        [Header("Time Display")]
        [Tooltip("Text element showing the current time of day")]
        [SerializeField] private TextMeshProUGUI timeText;
        [Tooltip("Text element showing the current day number")]
        [SerializeField] private TextMeshProUGUI dayText;
        [Tooltip("Text element showing the current season")]
        [SerializeField] private TextMeshProUGUI seasonText;

        [Header("Hotbar")]
        [Tooltip("Parent transform containing hotbar slot UI elements")]
        [SerializeField] private Transform hotbarContainer;

        [Header("Interaction")]
        [Tooltip("Text element showing the current interaction prompt")]
        [SerializeField] private TextMeshProUGUI interactionPromptText;
        [Tooltip("Panel that wraps the interaction prompt (toggled on/off)")]
        [SerializeField] private GameObject interactionPromptPanel;

        #endregion

        #region Private Fields

        private PlayerStats playerStats;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            playerStats = FindFirstObjectByType<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.OnHealthChanged += UpdateHealthBar;
                playerStats.OnStaminaChanged += UpdateStaminaBar;
            }

            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnTimeChanged += UpdateTimeDisplay;
            }

            if (SeasonManager.Instance != null)
            {
                SeasonManager.Instance.OnDayChanged += UpdateDayDisplay;
                SeasonManager.Instance.OnSeasonChanged += UpdateSeasonDisplay;
            }
        }

        private void OnDisable()
        {
            if (playerStats != null)
            {
                playerStats.OnHealthChanged -= UpdateHealthBar;
                playerStats.OnStaminaChanged -= UpdateStaminaBar;
            }

            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnTimeChanged -= UpdateTimeDisplay;
            }

            if (SeasonManager.Instance != null)
            {
                SeasonManager.Instance.OnDayChanged -= UpdateDayDisplay;
                SeasonManager.Instance.OnSeasonChanged -= UpdateSeasonDisplay;
            }
        }

        private void Start()
        {
            HideInteractionPrompt();

            // Initialize displays with current values
            if (playerStats != null)
            {
                UpdateHealthBar(playerStats.CurrentHealth, playerStats.MaxHealth);
                UpdateStaminaBar(playerStats.CurrentStamina, playerStats.MaxStamina);
            }

            if (TimeManager.Instance != null)
            {
                UpdateTimeDisplay(TimeManager.Instance.CurrentHour);
            }

            if (SeasonManager.Instance != null)
            {
                UpdateDayDisplay(SeasonManager.Instance.CurrentDay);
                UpdateSeasonDisplay(SeasonManager.Instance.CurrentSeason);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the health bar fill amount based on current and max values.
        /// </summary>
        public void UpdateHealthBar(float current, float max)
        {
            if (healthBar != null && max > 0f)
            {
                healthBar.fillAmount = current / max;
            }
        }

        /// <summary>
        /// Updates the stamina bar fill amount based on current and max values.
        /// </summary>
        public void UpdateStaminaBar(float current, float max)
        {
            if (staminaBar != null && max > 0f)
            {
                staminaBar.fillAmount = current / max;
            }
        }

        /// <summary>
        /// Updates the time display text. Formats the hour as "HH:MM AM/PM".
        /// </summary>
        public void UpdateTimeDisplay(float hour)
        {
            if (timeText == null) return;

            int totalMinutes = Mathf.FloorToInt(hour * 60f);
            int displayHour = totalMinutes / 60;
            int displayMinute = totalMinutes % 60;

            string period = displayHour >= 12 ? "PM" : "AM";
            int hour12 = displayHour % 12;
            if (hour12 == 0) hour12 = 12;

            timeText.text = $"{hour12:D2}:{displayMinute:D2} {period}";
        }

        /// <summary>
        /// Updates the day counter text as "Day X".
        /// </summary>
        public void UpdateDayDisplay(int day)
        {
            if (dayText != null)
            {
                dayText.text = $"Day {day}";
            }
        }

        /// <summary>
        /// Updates the season display text with the season name.
        /// </summary>
        public void UpdateSeasonDisplay(Season season)
        {
            if (seasonText != null)
            {
                seasonText.text = season.ToString();
            }
        }

        /// <summary>
        /// Shows the interaction prompt panel with the given text.
        /// </summary>
        public void ShowInteractionPrompt(string text)
        {
            if (interactionPromptText != null)
            {
                interactionPromptText.text = text;
            }

            if (interactionPromptPanel != null)
            {
                interactionPromptPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Hides the interaction prompt panel.
        /// </summary>
        public void HideInteractionPrompt()
        {
            if (interactionPromptPanel != null)
            {
                interactionPromptPanel.SetActive(false);
            }
        }

        #endregion
    }
}
