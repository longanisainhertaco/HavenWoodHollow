using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HavenwoodHollow.Stealth
{
    /// <summary>
    /// Calculates the aggregate light intensity at the player's position
    /// and determines stealth visibility.
    /// Reference: Plan Section 5.2 - Stealth Mechanics: The Invisible Enemy.
    ///
    /// If Intensity &lt; StealthThreshold, the Player enters "Stealth Mode,"
    /// becoming invisible to enemies (reduced aggro radius).
    /// Forces players to destroy light sources or stick to shadows.
    /// </summary>
    public class StealthSystem : MonoBehaviour
    {
        public static StealthSystem Instance { get; private set; }

        [Header("Stealth Settings")]
        [Tooltip("Light intensity below which player enters stealth mode")]
        [SerializeField] private float stealthThreshold = 0.3f;

        [Tooltip("Multiplier applied to enemy aggro radius when in stealth")]
        [SerializeField] private float stealthAggroMultiplier = 0.2f;

        [Tooltip("Radius to search for nearby Light2D sources")]
        [SerializeField] private float lightDetectionRadius = 10f;

        [Header("Current State")]
        [SerializeField] private bool isInStealth;
        [SerializeField] private float currentLightIntensity;

        private Transform playerTransform;

        /// <summary>Cached Light2D references to avoid per-frame FindObjectsByType calls.</summary>
        private Light2D[] cachedLights;
        private float lightCacheRefreshInterval = 2f;
        private float lightCacheTimer;

        public bool IsInStealth => isInStealth;
        public float CurrentLightIntensity => currentLightIntensity;
        public float StealthAggroMultiplier => isInStealth ? stealthAggroMultiplier : 1f;

        public event System.Action<bool> OnStealthChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            RefreshLightCache();
        }

        private void Update()
        {
            if (playerTransform == null) return;

            // Periodically refresh the light cache
            lightCacheTimer += Time.deltaTime;
            if (lightCacheTimer >= lightCacheRefreshInterval)
            {
                RefreshLightCache();
                lightCacheTimer = 0f;
            }

            currentLightIntensity = CalculateLightAtPosition(playerTransform.position);
            bool wasStealth = isInStealth;
            isInStealth = currentLightIntensity < stealthThreshold;

            if (isInStealth != wasStealth)
            {
                OnStealthChanged?.Invoke(isInStealth);
            }
        }

        /// <summary>
        /// Refreshes the cached array of Light2D references in the scene.
        /// Called periodically rather than every frame for performance.
        /// </summary>
        public void RefreshLightCache()
        {
            cachedLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        }

        /// <summary>
        /// Calculates the aggregate light intensity at a given world position
        /// by sampling nearby Light2D sources.
        /// </summary>
        private float CalculateLightAtPosition(Vector3 position)
        {
            if (cachedLights == null) return 1f;

            float totalIntensity = 0f;

            foreach (var light in cachedLights)
            {
                if (!light.enabled) continue;

                if (light.lightType == Light2D.LightType.Global)
                {
                    // Global light contributes directly
                    totalIntensity += light.intensity;
                }
                else
                {
                    // Point/Spot lights: intensity falls off with distance
                    float distance = Vector2.Distance(position, light.transform.position);
                    float outerRadius = light.pointLightOuterRadius;

                    if (distance < outerRadius)
                    {
                        // Linear falloff based on distance within the light's range
                        float falloff = 1f - (distance / outerRadius);
                        totalIntensity += light.intensity * falloff;
                    }
                }
            }

            return totalIntensity;
        }

        /// <summary>
        /// Sets the player transform reference (for runtime assignment).
        /// </summary>
        public void SetPlayer(Transform player)
        {
            playerTransform = player;
        }
    }
}
