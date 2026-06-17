using System;
using TopdownSurvival.Combat;
using UnityEngine;

namespace TopdownSurvival.Player
{
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float m_MaxHealth = 1f;
        [SerializeField] private PlayerController m_Controller;
        [SerializeField] private PlayerWeapon m_Weapon;

        private float m_Health;
        private bool m_IsAlive = true;

        public event Action Died;

        public bool IsAlive => m_IsAlive;
        public float Health => m_Health;

        private void Awake()
        {
            m_Health = m_MaxHealth;
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

            if (m_Controller != null)
            {
                m_Controller.enabled = false;
            }

            if (m_Weapon != null)
            {
                m_Weapon.enabled = false;
            }

            Died?.Invoke();
        }
    }
}
