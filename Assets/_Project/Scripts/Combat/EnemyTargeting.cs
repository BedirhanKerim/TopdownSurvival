using UnityEngine;

namespace TopdownSurvival.Combat
{
    public sealed class EnemyTargeting : MonoBehaviour, IAimTargetProvider
    {
        private const float k_MinAimSqrMagnitude = 0.0001f;

        [SerializeField] private LayerMask m_EnemyMask;
        [SerializeField] private float m_DetectRadius = 20f;
        [SerializeField] private float m_RefreshInterval = 0.1f;
        [SerializeField] private int m_MaxResults = 32;

        private Collider[] m_Hits;
        private Transform m_Current;
        private Vector3 m_AimDirection;
        private float m_NextSearchTime;

        public Transform Current => m_Current;
        public Vector3 AimDirection => m_AimDirection;

        public bool TryGetTarget(out Transform target)
        {
            target = m_Current;
            return m_Current != null;
        }

        private void Awake()
        {
            m_Hits = new Collider[Mathf.Max(1, m_MaxResults)];
        }

        private void Update()
        {
            if (Time.time >= m_NextSearchTime)
            {
                m_NextSearchTime = Time.time + m_RefreshInterval;
                SearchNearestEnemy();
            }

            RefreshAimDirection();
        }

        private void SearchNearestEnemy()
        {
            Vector3 origin = transform.position;
            int count = Physics.OverlapSphereNonAlloc(origin, m_DetectRadius, m_Hits, m_EnemyMask);

            Transform nearest = null;
            float nearestSqr = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider hit = m_Hits[i];
                if (hit == null)
                {
                    continue;
                }

                Vector3 offset = hit.transform.position - origin;
                offset.y = 0f;
                float sqr = offset.sqrMagnitude;
                if (sqr < nearestSqr)
                {
                    nearestSqr = sqr;
                    nearest = hit.transform;
                }
            }

            m_Current = nearest;
        }

        private void RefreshAimDirection()
        {
            if (m_Current == null)
            {
                m_AimDirection = Vector3.zero;
                return;
            }

            Vector3 direction = m_Current.position - transform.position;
            direction.y = 0f;
            m_AimDirection = direction.sqrMagnitude > k_MinAimSqrMagnitude
                ? direction.normalized
                : Vector3.zero;
        }
    }
}
