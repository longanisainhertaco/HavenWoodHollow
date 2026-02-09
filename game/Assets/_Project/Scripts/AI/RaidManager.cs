using System;
using System.Collections.Generic;
using UnityEngine;
using HavenwoodHollow.AI.GOAP;
using HavenwoodHollow.Core;

namespace HavenwoodHollow.AI
{
    /// <summary>
    /// Singleton that manages procedural raid events for town defense.
    /// Subscribes to the night cycle and spawns enemy waves at designated points.
    /// Reference: Plan Section 4.3 - Procedural Raid Generation.
    /// </summary>
    public class RaidManager : MonoBehaviour
    {
        #region Singleton

        public static RaidManager Instance { get; private set; }

        #endregion

        #region Serialized Fields

        [Header("Raid Settings")]
        [Tooltip("Probability of a raid occurring each night (0-1)")]
        [SerializeField] private float raidCheckChance = 0.3f;

        [Tooltip("Minimum number of enemies per raid")]
        [SerializeField] private int minEnemies = 3;

        [Tooltip("Maximum number of enemies per raid")]
        [SerializeField] private int maxEnemies = 8;

        [Header("Spawn Configuration")]
        [Tooltip("Points where enemies can spawn during a raid")]
        [SerializeField] private Transform[] spawnPoints;

        [Tooltip("Enemy prefabs to randomly select from when spawning")]
        [SerializeField] private GameObject[] enemyPrefabs;

        #endregion

        #region Private Fields

        private bool raidActive;
        private int currentWave;
        private List<GameObject> activeEnemies = new List<GameObject>();

        /// <summary>Reference to shared world state for setting RaidActive flag.</summary>
        private WorldState globalState;

        #endregion

        #region Events

        /// <summary>Fired when a raid begins.</summary>
        public event Action OnRaidStarted;

        /// <summary>Fired when a raid ends.</summary>
        public event Action OnRaidEnded;

        /// <summary>Fired when a new wave starts, with the wave number.</summary>
        public event Action<int> OnWaveStarted;

        #endregion

        #region Properties

        /// <summary>Whether a raid is currently in progress.</summary>
        public bool IsRaidActive => raidActive;

        /// <summary>Number of living enemies in the current raid.</summary>
        public int ActiveEnemyCount => activeEnemies.Count;

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
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnNightStarted += CheckForRaid;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnNightStarted -= CheckForRaid;
            }
        }

        private void Update()
        {
            if (!raidActive) return;

            // Clean up destroyed enemies
            activeEnemies.RemoveAll(e => e == null);

            if (activeEnemies.Count == 0)
            {
                EndRaid();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the shared world state reference so raids can toggle RaidActive.
        /// </summary>
        public void SetGlobalState(WorldState state)
        {
            globalState = state;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs a random roll to determine if a raid should start tonight.
        /// </summary>
        private void CheckForRaid()
        {
            if (raidActive) return;

            float roll = UnityEngine.Random.value;
            if (roll <= raidCheckChance)
            {
                StartRaid();
            }
        }

        /// <summary>
        /// Starts a raid by spawning enemies at designated spawn points.
        /// Sets the global world state RaidActive flag to true.
        /// </summary>
        private void StartRaid()
        {
            raidActive = true;
            currentWave = 1;

            if (globalState != null)
            {
                globalState.SetBool("RaidActive", true);
            }

            int enemyCount = UnityEngine.Random.Range(minEnemies, maxEnemies + 1);

            for (int i = 0; i < enemyCount; i++)
            {
                if (spawnPoints == null || spawnPoints.Length == 0) break;

                Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
                SpawnEnemy(spawnPoint);
            }

            Debug.Log($"[RaidManager] Raid started! Wave {currentWave}, {activeEnemies.Count} enemies spawned.");
            OnWaveStarted?.Invoke(currentWave);
            OnRaidStarted?.Invoke();
        }

        /// <summary>
        /// Ends the current raid, cleans up remaining enemies,
        /// and sets the global world state RaidActive flag to false.
        /// </summary>
        private void EndRaid()
        {
            raidActive = false;

            if (globalState != null)
            {
                globalState.SetBool("RaidActive", false);
            }

            // Clean up any remaining enemies
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
            activeEnemies.Clear();

            Debug.Log("[RaidManager] Raid ended.");
            OnRaidEnded?.Invoke();
        }

        /// <summary>
        /// Instantiates a random enemy prefab at the given spawn point.
        /// </summary>
        private void SpawnEnemy(Transform spawnPoint)
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

            GameObject prefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            activeEnemies.Add(enemy);
        }

        #endregion
    }
}
