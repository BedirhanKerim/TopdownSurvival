using Lean.Pool;
using UnityEngine;

namespace TopdownSurvival.Combat
{
    public sealed class PlayerWeapon : MonoBehaviour
    {
        private const float k_MinDirectionSqrMagnitude = 0.0001f;

        private static readonly int s_IsFiringHash = Animator.StringToHash("isFiring");

        [SerializeField] private MonoBehaviour m_AimProviderSource;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Transform m_Muzzle;
        [SerializeField] private Bullet m_BulletPrefab;
        [SerializeField] private float m_FireRate = 6f;
        [SerializeField] private ParticleSystem m_MuzzleVfx;
        [SerializeField] private AudioSource m_FireAudioSource;
        [SerializeField] private AudioClip m_FireClip;

        private IAimTargetProvider m_AimProvider;
        private float m_NextFireTime;
        private bool m_IsFiring;

        public bool HasTarget => m_AimProvider != null && m_AimProvider.TryGetTarget(out _);

        private void Awake()
        {
            m_AimProvider = m_AimProviderSource as IAimTargetProvider;
            if (m_AimProvider == null)
            {
                Debug.LogError($"{nameof(PlayerWeapon)} on '{name}' needs an aim provider implementing {nameof(IAimTargetProvider)}.", this);
            }

            if (m_BulletPrefab == null)
            {
                Debug.LogError($"{nameof(PlayerWeapon)} on '{name}' has no bullet prefab.", this);
            }
        }

        private void Update()
        {
            if (m_AimProvider == null)
            {
                return;
            }

            bool hasTarget = m_AimProvider.TryGetTarget(out _);
            if (hasTarget != m_IsFiring)
            {
                m_IsFiring = hasTarget;
                if (m_Animator != null)
                {
                    m_Animator.SetBool(s_IsFiringHash, hasTarget);
                }
            }

            if (!hasTarget)
            {
                return;
            }

            if (Time.time >= m_NextFireTime)
            {
                m_NextFireTime = Time.time + 1f / Mathf.Max(0.01f, m_FireRate);
                Fire();
            }
        }

        public void Fire()
        {
            if (m_BulletPrefab == null || m_Muzzle == null)
            {
                return;
            }

            Vector3 direction = m_AimProvider != null && m_AimProvider.AimDirection.sqrMagnitude > k_MinDirectionSqrMagnitude
                ? m_AimProvider.AimDirection
                : transform.forward;

            Bullet bullet = LeanPool.Spawn(m_BulletPrefab, m_Muzzle.position, Quaternion.LookRotation(direction, Vector3.up));
            if (bullet == null)
            {
                return;
            }

            bullet.Launch(m_Muzzle.position, direction);

            if (m_MuzzleVfx != null)
            {
                m_MuzzleVfx.Play();
            }

            if (m_FireAudioSource != null && m_FireClip != null)
            {
                m_FireAudioSource.PlayOneShot(m_FireClip);
            }
        }
    }
}
