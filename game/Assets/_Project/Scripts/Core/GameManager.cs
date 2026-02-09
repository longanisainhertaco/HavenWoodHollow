using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HavenwoodHollow.Core
{
    /// <summary>
    /// Main game manager singleton. Persists across scenes.
    /// Coordinates all game systems for Havenwood Hollow.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private bool isPaused;

        public bool IsPaused => isPaused;

        public event Action<bool> OnPauseChanged;
        public event Action OnGameStarted;
        public event Action OnGameEnded;
        public event Action OnDayStarted;
        public event Action OnNightStarted;
        public event Action OnSleepTriggered;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }

        private void Initialize()
        {
            Debug.Log("[GameManager] Havenwood Hollow initialized");
        }

        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;
            OnPauseChanged?.Invoke(true);
        }

        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;
            OnPauseChanged?.Invoke(false);
        }

        /// <summary>
        /// Triggers the nightly sleep event, advancing the day.
        /// This drives crop growth, NPC schedules, and raid checks.
        /// </summary>
        public void TriggerSleep()
        {
            OnSleepTriggered?.Invoke();
        }

        public void StartDay()
        {
            OnDayStarted?.Invoke();
        }

        public void StartNight()
        {
            OnNightStarted?.Invoke();
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void QuitGame()
        {
            OnGameEnded?.Invoke();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
