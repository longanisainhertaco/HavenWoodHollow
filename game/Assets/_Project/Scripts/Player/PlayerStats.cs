using System;
using UnityEngine;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.Player
{
    /// <summary>
    /// Manages player health, stamina, and energy.
    /// Provides events for UI binding and gameplay systems.
    /// Reference: Plan Section 4.2.2 Table 2 - HealthLow, Stamina &lt; 20.
    /// </summary>
    public class PlayerStats : MonoBehaviour, IDamageable
    {
        #region Serialized Fields

        [Header("Health Settings")]
        [Tooltip("Maximum health value")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [Header("Stamina Settings")]
        [Tooltip("Maximum stamina value")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float currentStamina;

        [Header("Regeneration")]
        [Tooltip("Passive stamina regeneration per second when idle")]
        [SerializeField] private float staminaRegenRate = 0.5f;

        #endregion

        #region Events

        /// <summary>Fired when health changes. Parameters: current, max.</summary>
        public event Action<float, float> OnHealthChanged;

        /// <summary>Fired when stamina changes. Parameters: current, max.</summary>
        public event Action<float, float> OnStaminaChanged;

        /// <summary>Fired when the player dies.</summary>
        public event Action OnDied;

        #endregion

        #region Private Fields

        private Vector2 lastPosition;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public float CurrentHealth => currentHealth;

        /// <inheritdoc/>
        public float MaxHealth => maxHealth;

        /// <summary>Current stamina value.</summary>
        public float CurrentStamina => currentStamina;

        /// <summary>Maximum stamina value.</summary>
        public float MaxStamina => maxStamina;

        /// <inheritdoc/>
        public bool IsAlive => currentHealth > 0f;

        /// <summary>Stamina as a 0-1 percentage.</summary>
        public float StaminaPercentage => maxStamina > 0f ? currentStamina / maxStamina : 0f;

        /// <summary>Health as a 0-1 percentage.</summary>
        public float HealthPercentage => maxHealth > 0f ? currentHealth / maxHealth : 0f;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
        }

        /// <summary>
        /// Passive stamina regeneration when the player is not moving.
        /// </summary>
        private void Update()
        {
            if (!IsAlive) return;

            Vector2 currentPosition = (Vector2)transform.position;
            bool isMoving = (currentPosition - lastPosition).sqrMagnitude > 0.0001f;
            lastPosition = currentPosition;

            if (!isMoving && currentStamina < maxStamina)
            {
                RestoreStamina(staminaRegenRate * Time.deltaTime);
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public void TakeDamage(float damage)
        {
            if (!IsAlive || damage <= 0f) return;

            currentHealth = Mathf.Max(currentHealth - damage, 0f);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (!IsAlive)
            {
                OnDied?.Invoke();
            }
        }

        /// <inheritdoc/>
        public void OnDestroyed()
        {
            OnDied?.Invoke();
        }

        /// <summary>
        /// Heals the player by the specified amount, clamped to max health.
        /// </summary>
        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f) return;

            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Attempts to consume stamina. Returns false if insufficient.
        /// </summary>
        public bool UseStamina(float cost)
        {
            if (cost <= 0f) return true;
            if (!HasEnoughStamina(cost)) return false;

            currentStamina = Mathf.Max(currentStamina - cost, 0f);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            return true;
        }

        /// <summary>
        /// Checks whether the player has enough stamina for a given cost.
        /// </summary>
        public bool HasEnoughStamina(float cost)
        {
            return currentStamina >= cost;
        }

        /// <summary>
        /// Restores stamina by the specified amount, clamped to max.
        /// </summary>
        public void RestoreStamina(float amount)
        {
            if (amount <= 0f) return;

            currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        /// <summary>
        /// Sets health to a specific value, clamped between 0 and max.
        /// Used by SaveSystem to restore saved health.
        /// </summary>
        public void SetHealth(float value)
        {
            currentHealth = Mathf.Clamp(value, 0f, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            if (!IsAlive) OnDied?.Invoke();
        }

        /// <summary>
        /// Sets stamina to a specific value, clamped between 0 and max.
        /// Used by SaveSystem to restore saved stamina.
        /// </summary>
        public void SetStamina(float value)
        {
            currentStamina = Mathf.Clamp(value, 0f, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        /// <summary>
        /// Fully restores health and stamina to maximum values.
        /// </summary>
        public void RestoreAllStats()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        #endregion
    }
}
