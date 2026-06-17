using TopdownSurvival.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace TopdownSurvival.Enemies
{
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyHealth m_Health;
        [SerializeField] private NavMeshAgent m_Agent;
        [SerializeField] private float m_RepathInterval = 0.15f;
        [SerializeField] private float m_ContactDistance = 1.2f;
        [SerializeField] private float m_ContactDamage = 100f;

        private Transform m_Target;
        private IDamageable m_TargetDamageable;
        private float m_NextRepathTime;

        public void Activate(Vector3 position, Transform target)
        {
            if (m_Health != null)
            {
                m_Health.ResetForSpawn();
            }

            m_Target = target;
            m_TargetDamageable = target != null ? target.GetComponentInParent<IDamageable>() : null;

            transform.position = position;
            if (m_Agent != null)
            {
                m_Agent.enabled = true;
                m_Agent.Warp(position);
                if (m_Agent.isOnNavMesh)
                {
                    m_Agent.isStopped = false;
                }
            }

            m_NextRepathTime = 0f;
        }

        private void Update()
        {
            if (m_Health == null || !m_Health.IsAlive || m_Target == null)
            {
                return;
            }

            if (Time.time >= m_NextRepathTime && m_Agent != null && m_Agent.enabled && m_Agent.isOnNavMesh)
            {
                m_NextRepathTime = Time.time + m_RepathInterval;
                m_Agent.SetDestination(m_Target.position);
            }

            TryContactDamage();
        }

        private void TryContactDamage()
        {
            if (m_TargetDamageable == null || !m_TargetDamageable.IsAlive)
            {
                return;
            }

            Vector3 offset = m_Target.position - transform.position;
            offset.y = 0f;
            if (offset.sqrMagnitude <= m_ContactDistance * m_ContactDistance)
            {
                m_TargetDamageable.TakeDamage(m_ContactDamage);
            }
        }
    }
}
