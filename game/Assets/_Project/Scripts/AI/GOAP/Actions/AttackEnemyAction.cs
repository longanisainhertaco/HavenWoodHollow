using UnityEngine;
using HavenwoodHollow.Inventory;

namespace HavenwoodHollow.AI.GOAP.Actions
{
    /// <summary>
    /// GOAP action: move to the nearest enemy and attack it.
    /// Uses Physics2D.OverlapCircle to find IDamageable targets tagged "Enemy".
    /// Reference: Plan Section 4.2.2 - Goals and Actions.
    /// </summary>
    public class AttackEnemyAction : GOAPAction
    {
        #region Serialized Fields

        [Header("Attack Settings")]
        [Tooltip("Range at which the NPC can deal damage")]
        [SerializeField] private float attackRange = 1.5f;

        [Tooltip("Damage dealt per attack")]
        [SerializeField] private float attackDamage = 10f;

        [Header("Detection Settings")]
        [Tooltip("Radius to scan for enemies")]
        [SerializeField] private float detectionRadius = 10f;

        [Tooltip("Layer mask to filter enemy colliders")]
        [SerializeField] private LayerMask enemyLayerMask = ~0;

        [Tooltip("Time between consecutive attacks")]
        [SerializeField] private float attackCooldown = 0.5f;

        #endregion

        #region Private Fields

        private NPCController npcController;
        private Transform currentTarget;
        private float attackTimer;

        #endregion

        #region GOAPAction Implementation

        /// <summary>
        /// Preconditions: the NPC must be armed and an enemy must be visible.
        /// </summary>
        public override WorldState GetPreconditions()
        {
            var preconditions = new WorldState();
            preconditions.SetBool("HasWeapon", true);
            preconditions.SetBool("EnemyVisible", true);
            return preconditions;
        }

        /// <summary>
        /// Effects: an enemy is killed.
        /// </summary>
        public override WorldState GetEffects()
        {
            var effects = new WorldState();
            effects.SetBool("EnemyDead", true);
            return effects;
        }

        /// <summary>
        /// Begins the attack sequence by locating the nearest enemy.
        /// </summary>
        public override void OnActionStart(GOAPAgent agent)
        {
            npcController = agent.GetComponent<NPCController>();
            attackTimer = 0f;
            FindNearestEnemy(agent.transform.position);
        }

        /// <summary>
        /// Moves towards the target enemy and attacks when in range.
        /// Returns true when the target is destroyed or lost.
        /// </summary>
        public override bool OnActionUpdate(GOAPAgent agent)
        {
            if (currentTarget == null)
            {
                // Target lost, try to find another
                FindNearestEnemy(agent.transform.position);
                return currentTarget == null;
            }

            float distance = Vector2.Distance(agent.transform.position, currentTarget.position);

            if (distance > attackRange)
            {
                // Move towards enemy
                npcController.MoveTo(currentTarget.position);
                return false;
            }

            // In range - attack on cooldown
            npcController.Stop();
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                attackTimer = 0f;
                PerformAttack();
            }

            return false;
        }

        /// <summary>
        /// Stops the NPC when the action ends.
        /// </summary>
        public override void OnActionEnd(GOAPAgent agent)
        {
            currentTarget = null;

            if (npcController != null)
            {
                npcController.Stop();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Scans for the nearest GameObject tagged "Enemy" using Physics2D.OverlapCircle.
        /// </summary>
        private void FindNearestEnemy(Vector3 position)
        {
            currentTarget = null;
            float closestDistance = float.MaxValue;

            Collider2D[] hits = Physics2D.OverlapCircleAll(position, detectionRadius, enemyLayerMask);

            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;

                float distance = Vector2.Distance(position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentTarget = hit.transform;
                }
            }
        }

        /// <summary>
        /// Deals damage to the current target if it implements IDamageable.
        /// </summary>
        private void PerformAttack()
        {
            if (currentTarget == null) return;

            IDamageable damageable = currentTarget.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);

                if (!damageable.IsAlive)
                {
                    currentTarget = null;
                }
            }
        }

        #endregion
    }
}
