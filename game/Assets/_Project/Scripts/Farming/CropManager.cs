using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HavenwoodHollow.Farming
{
    /// <summary>
    /// Manages all crop tiles using a Dictionary-based approach.
    /// Avoids creating a GameObject per plant for performance.
    /// Growth simulation runs only on the "Sleep" event.
    /// Reference: Plan Section 3.2.2 - Unity 6 Implementation Strategy.
    /// </summary>
    public class CropManager : MonoBehaviour
    {
        public static CropManager Instance { get; private set; }

        [Header("Tilemap References")]
        [SerializeField] private Tilemap cropTilemap;

        [Header("Current Season")]
        [SerializeField] private Season currentSeason = Season.Spring;

        /// <summary>
        /// Dictionary storing dynamic crop data per grid coordinate.
        /// </summary>
        private Dictionary<Vector3Int, CropState> cropTiles = new Dictionary<Vector3Int, CropState>();

        public IReadOnlyDictionary<Vector3Int, CropState> CropTiles => cropTiles;

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
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnSleepTriggered += OnSleep;
            }
        }

        private void OnDisable()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnSleepTriggered -= OnSleep;
            }
        }

        /// <summary>
        /// Plants a crop at the given tile position.
        /// </summary>
        public bool PlantCrop(Vector3Int position, CropData cropData)
        {
            if (cropTiles.ContainsKey(position))
            {
                Debug.LogWarning($"[CropManager] Tile {position} already has a crop.");
                return false;
            }

            if (!cropData.CanGrowInSeason(currentSeason))
            {
                Debug.LogWarning($"[CropManager] {cropData.DisplayName} cannot grow in {currentSeason}.");
                return false;
            }

            var state = new CropState
            {
                CropData = cropData,
                CurrentPhase = 0,
                DaysInCurrentPhase = 0,
                IsWatered = false,
                IsHarvestable = false,
                FertilizerModifier = 0f,
                TotalDaysGrown = 0
            };

            cropTiles[position] = state;
            UpdateTileVisual(position, state);
            return true;
        }

        /// <summary>
        /// Waters a crop tile.
        /// </summary>
        public void WaterCrop(Vector3Int position)
        {
            if (!cropTiles.TryGetValue(position, out var state)) return;

            state.IsWatered = true;
            cropTiles[position] = state;
        }

        /// <summary>
        /// Applies fertilizer to a crop tile.
        /// </summary>
        public void ApplyFertilizer(Vector3Int position, float modifier)
        {
            if (!cropTiles.TryGetValue(position, out var state)) return;

            state.FertilizerModifier = modifier;
            cropTiles[position] = state;
        }

        /// <summary>
        /// Harvests a crop at the given position if it is ready.
        /// Returns the crop data for reward processing, or null.
        /// </summary>
        public CropData HarvestCrop(Vector3Int position)
        {
            if (!cropTiles.TryGetValue(position, out var state)) return null;
            if (!state.IsHarvestable) return null;

            CropData data = state.CropData;

            if (data.Regrows)
            {
                // Revert to regrow phase
                state.CurrentPhase = data.RegrowPhaseIndex;
                state.DaysInCurrentPhase = 0;
                state.IsHarvestable = false;
                cropTiles[position] = state;
                UpdateTileVisual(position, state);
            }
            else
            {
                // Remove crop entirely
                cropTiles.Remove(position);
                if (cropTilemap != null)
                {
                    cropTilemap.SetTile(position, null);
                }
            }

            return data;
        }

        /// <summary>
        /// Removes a crop without harvesting (e.g., destroyed by season change).
        /// </summary>
        public void RemoveCrop(Vector3Int position)
        {
            cropTiles.Remove(position);
            if (cropTilemap != null)
            {
                cropTilemap.SetTile(position, null);
            }
        }

        /// <summary>
        /// Sets the current season. Kills crops that cannot survive.
        /// </summary>
        public void SetSeason(Season season)
        {
            currentSeason = season;

            var toRemove = new List<Vector3Int>();
            foreach (var kvp in cropTiles)
            {
                if (!kvp.Value.CropData.CanGrowInSeason(season))
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var pos in toRemove)
            {
                RemoveCrop(pos);
            }
        }

        /// <summary>
        /// Nightly growth simulation. Called when the player sleeps.
        /// Growth is strictly conditional on watering.
        /// Reference: Plan Section 3.2.1 - Watering Dependency.
        /// </summary>
        private void OnSleep()
        {
            var positions = new List<Vector3Int>(cropTiles.Keys);

            foreach (var pos in positions)
            {
                var state = cropTiles[pos];

                // Growth requires watering
                if (!state.IsWatered)
                {
                    // Reset watered flag for next day (already false)
                    continue;
                }

                if (state.IsHarvestable)
                {
                    // Already fully grown, just reset watered
                    state.IsWatered = false;
                    cropTiles[pos] = state;
                    continue;
                }

                // Advance growth
                state.DaysInCurrentPhase++;
                state.TotalDaysGrown++;

                int originalPhaseDuration = state.CropData.PhaseDurations[state.CurrentPhase];
                int phaseDuration = originalPhaseDuration;

                // Apply fertilizer: reduce effective phase duration
                if (state.FertilizerModifier > 0f)
                {
                    int daysSaved = state.CropData.CalculateFertilizerDaysSaved(state.FertilizerModifier);
                    // Distribute saved days proportionally using original duration
                    float ratio = (float)originalPhaseDuration / state.CropData.TotalGrowthDays;
                    int phaseSaved = Mathf.Max(0, Mathf.FloorToInt(daysSaved * ratio));
                    phaseDuration = Mathf.Max(1, originalPhaseDuration - phaseSaved);
                }

                if (state.DaysInCurrentPhase >= phaseDuration)
                {
                    state.CurrentPhase++;
                    state.DaysInCurrentPhase = 0;

                    if (state.CurrentPhase >= state.CropData.PhaseCount)
                    {
                        // Crop is fully grown
                        state.CurrentPhase = state.CropData.PhaseCount - 1;
                        state.IsHarvestable = true;
                    }
                }

                // Reset watered for next day
                state.IsWatered = false;
                cropTiles[pos] = state;
                UpdateTileVisual(pos, state);
            }
        }

        /// <summary>
        /// Cache of Tile instances keyed by sprite to avoid repeated allocations.
        /// ScriptableObject.CreateInstance persists until destroyed, so we reuse tiles.
        /// </summary>
        private Dictionary<Sprite, UnityEngine.Tilemaps.Tile> tileCache = new Dictionary<Sprite, UnityEngine.Tilemaps.Tile>();

        /// <summary>
        /// Updates the visual tile sprite based on current growth phase.
        /// Uses Tilemap.SetTile for efficient rendering.
        /// </summary>
        private void UpdateTileVisual(Vector3Int position, CropState state)
        {
            if (cropTilemap == null) return;
            if (state.CropData.PhaseSprites == null || state.CropData.PhaseSprites.Length == 0) return;

            int spriteIndex = Mathf.Clamp(state.CurrentPhase, 0, state.CropData.PhaseSprites.Length - 1);
            Sprite sprite = state.CropData.PhaseSprites[spriteIndex];

            if (sprite != null)
            {
                if (!tileCache.TryGetValue(sprite, out var tile))
                {
                    tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
                    tile.sprite = sprite;
                    tileCache[sprite] = tile;
                }
                cropTilemap.SetTile(position, tile);
            }
        }
    }
}
