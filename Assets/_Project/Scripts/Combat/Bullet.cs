using Lean.Pool;
using UnityEngine;

namespace TopdownSurvival.Combat
{
    public sealed class Bullet : MonoBehaviour
    {
        private const float k_MinDirectionSqrMagnitude = 0.0001f;

        [SerializeField] private float m_Speed = 30f;
        [SerializeField] private float m_Damage = 50f;
        [SerializeField] private float m_MaxLifetime = 3f;
        [SerializeField] private float m_Radius = 0.1f;
        [SerializeField] private LayerMask m_HitMask;
        [SerializeField] private ParticleSystem m_ImpactVfxPrefab;
        [SerializeField] private float m_ImpactVfxLifetime = 1f;

        private readonly RaycastHit[] m_HitBuffer = new RaycastHit[4];
        private Vector3 m_Direction;
        private float m_Age;

        public void Launch(Vector3 origin, Vector3 direction)
        {
            m_Direction = direction.sqrMagnitude > k_MinDirectionSqrMagnitude
                ? direction.normalized
                : transform.forward;
            m_Age = 0f;
            transform.SetPositionAndRotation(origin, Quaternion.LookRotation(m_Direction, Vector3.up));
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            m_Age += deltaTime;
            if (m_Age >= m_MaxLifetime)
            {
                Despawn();
                return;
            }

            float step = m_Speed * deltaTime;
            int count = Physics.SphereCastNonAlloc(transform.position, m_Radius, m_Direction, m_HitBuffer, step, m_HitMask, QueryTriggerInteraction.Ignore);
            if (count > 0)
            {
                RaycastHit nearest = m_HitBuffer[0];
                for (int i = 1; i < count; i++)
                {
                    if (m_HitBuffer[i].distance < nearest.distance)
                    {
                        nearest = m_HitBuffer[i];
                    }
                }

                if (nearest.collider.TryGetComponent(out IDamageable damageable) && damageable.IsAlive)
                {
                    damageable.TakeDamage(m_Damage);
                }

                transform.position = nearest.point;
                PlayImpact(nearest.point);
                Despawn();
                return;
            }

            transform.position += m_Direction * step;
        }

        private void PlayImpact(Vector3 point)
        {
            if (m_ImpactVfxPrefab == null)
            {
                return;
            }

            ParticleSystem vfx = LeanPool.Spawn(m_ImpactVfxPrefab, point, Quaternion.identity);
            if (vfx == null)
            {
                return;
            }

            vfx.transform.position = point;
            vfx.Clear();
            vfx.Play();
            LeanPool.Despawn(vfx, m_ImpactVfxLifetime);
        }

        private void Despawn()
        {
            LeanPool.Despawn(this);
        }
    }
}
