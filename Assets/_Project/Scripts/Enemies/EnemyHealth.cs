using System;
using Lean.Pool;
using TopdownSurvival.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace TopdownSurvival.Enemies
{
    public sealed class EnemyHealth : MonoBehaviour, IDamageable
    {
        private static readonly int s_DieHash = Animator.StringToHash("Die");

        [SerializeField] private float m_MaxHealth = 100f;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private NavMeshAgent m_Agent;
        [SerializeField] private Collider m_Collider;
        [SerializeField] private float m_DeathReturnDelay = 2f;

        private float m_Health;
        private bool m_IsAlive = true;

        public event Action<EnemyHealth> Died;

        public bool IsAlive => m_IsAlive;

        public void ResetForSpawn()
        {
            m_Health = m_MaxHealth;
            m_IsAlive = true;

            if (m_Collider != null)
            {
                m_Collider.enabled = true;
            }

            if (m_Animator != null)
            {
                m_Animator.Rebind();
                m_Animator.Update(0f);
            }
        }

        public void TakeDamage(float amount)
        {
            if (!m_IsAlive)
            {
                return;
            }

            m_Health -= amount;
            if (m_Health > 0f)
            {
                return;
            }

            Die();
        }

        private void Die()
        {
            m_IsAlive = false;

            if (m_Agent != null && m_Agent.enabled)
            {
                if (m_Agent.isOnNavMesh)
                {
                    m_Agent.isStopped = true;
                    m_Agent.ResetPath();
                }

                m_Agent.enabled = false;
            }

            if (m_Collider != null)
            {
                m_Collider.enabled = false;
            }

            if (m_Animator != null)
            {
                m_Animator.SetTrigger(s_DieHash);
            }

            Died?.Invoke(this);
            LeanPool.Despawn(gameObject, m_DeathReturnDelay);
        }
    }
}
