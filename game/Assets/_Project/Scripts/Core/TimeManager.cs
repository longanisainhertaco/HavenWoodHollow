using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HavenwoodHollow.Core
{
    /// <summary>
    /// Manages the in-game day/night cycle and time progression.
    /// Drives the Global Light 2D color and intensity based on time of day.
    /// Reference: Plan Section 5.1.1 - URP 2D Lighting Components.
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [Header("Time Settings")]
        [SerializeField] private float dayLengthInSeconds = 600f;
        [SerializeField] private float startHour = 6f;
        [Tooltip("Current time expressed as hours (0-24)")]
        [SerializeField] private float currentTimeOfDay;

        [Header("Lighting Reference")]
        [SerializeField] private Light2D globalLight;

        [Header("Day Phase Colors (URP 2D)")]
        [Tooltip("Day (12:00): Pure White, intensity 1.0")]
        [SerializeField] private Color dayColor = Color.white;
        [SerializeField] private float dayIntensity = 1.0f;

        [Tooltip("Dusk (18:00): Orange/Purple tint, intensity 0.8")]
        [SerializeField] private Color duskColor = new Color(0.8f, 0.5f, 0.6f);
        [SerializeField] private float duskIntensity = 0.8f;

        [Tooltip("Night (00:00): Deep Blue/Black, intensity 0.3")]
        [SerializeField] private Color nightColor = new Color(0.1f, 0.1f, 0.25f);
        [SerializeField] private float nightIntensity = 0.3f;

        [Tooltip("Dawn (06:00): Warm sunrise")]
        [SerializeField] private Color dawnColor = new Color(0.9f, 0.7f, 0.5f);
        [SerializeField] private float dawnIntensity = 0.6f;

        public float CurrentHour => currentTimeOfDay;
        public bool IsNight => currentTimeOfDay >= 20f || currentTimeOfDay < 5f;
        public bool IsEvening => currentTimeOfDay >= 17f && currentTimeOfDay < 20f;
        public bool IsDaytime => currentTimeOfDay >= 5f && currentTimeOfDay < 17f;

        public event Action<float> OnTimeChanged;
        public event Action OnDawn;
        public event Action OnDusk;
        public event Action OnMidnight;

        private float hoursPerSecond;
        private float previousHour;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            currentTimeOfDay = startHour;
            hoursPerSecond = 24f / dayLengthInSeconds;
        }

        private void Update()
        {
            previousHour = currentTimeOfDay;
            currentTimeOfDay += hoursPerSecond * Time.deltaTime;

            if (currentTimeOfDay >= 24f)
            {
                currentTimeOfDay -= 24f;
            }

            CheckTimeEvents();
            UpdateLighting();
            OnTimeChanged?.Invoke(currentTimeOfDay);
        }

        private void CheckTimeEvents()
        {
            if (CrossedHour(6f))
                OnDawn?.Invoke();
            if (CrossedHour(18f))
                OnDusk?.Invoke();
            if (CrossedHour(0f))
                OnMidnight?.Invoke();
        }

        private bool CrossedHour(float hour)
        {
            if (hour == 0f)
            {
                return previousHour > 23f && currentTimeOfDay < 1f;
            }
            return previousHour < hour && currentTimeOfDay >= hour;
        }

        /// <summary>
        /// Interpolates the global light color and intensity across four key points:
        /// Dawn (06:00), Day (12:00), Dusk (18:00), Night (00:00).
        /// </summary>
        private void UpdateLighting()
        {
            if (globalLight == null) return;

            Color targetColor;
            float targetIntensity;

            if (currentTimeOfDay >= 6f && currentTimeOfDay < 12f)
            {
                // Dawn -> Day (06:00 - 12:00)
                float t = (currentTimeOfDay - 6f) / 6f;
                targetColor = Color.Lerp(dawnColor, dayColor, t);
                targetIntensity = Mathf.Lerp(dawnIntensity, dayIntensity, t);
            }
            else if (currentTimeOfDay >= 12f && currentTimeOfDay < 18f)
            {
                // Day -> Dusk (12:00 - 18:00)
                float t = (currentTimeOfDay - 12f) / 6f;
                targetColor = Color.Lerp(dayColor, duskColor, t);
                targetIntensity = Mathf.Lerp(dayIntensity, duskIntensity, t);
            }
            else if (currentTimeOfDay >= 18f && currentTimeOfDay < 24f)
            {
                // Dusk -> Night (18:00 - 00:00)
                float t = (currentTimeOfDay - 18f) / 6f;
                targetColor = Color.Lerp(duskColor, nightColor, t);
                targetIntensity = Mathf.Lerp(duskIntensity, nightIntensity, t);
            }
            else
            {
                // Night -> Dawn (00:00 - 06:00)
                float t = currentTimeOfDay / 6f;
                targetColor = Color.Lerp(nightColor, dawnColor, t);
                targetIntensity = Mathf.Lerp(nightIntensity, dawnIntensity, t);
            }

            globalLight.color = targetColor;
            globalLight.intensity = targetIntensity;
        }

        /// <summary>
        /// Sets the time of day directly (for sleep/warp mechanics).
        /// </summary>
        public void SetTime(float hour)
        {
            currentTimeOfDay = Mathf.Repeat(hour, 24f);
            UpdateLighting();
        }

        /// <summary>
        /// Advances time to the next morning (06:00). Used by the sleep mechanic.
        /// </summary>
        public void AdvanceToMorning()
        {
            SetTime(6f);
        }
    }
}
