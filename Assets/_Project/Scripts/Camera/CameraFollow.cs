using UnityEngine;

namespace TopdownSurvival.CameraRig
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform m_Target;
        [SerializeField] private Vector3 m_Offset;
        [SerializeField] private float m_SmoothTime = 0.15f;

        private Vector3 m_Velocity;

        private void Start()
        {
            if (m_Target != null && m_Offset == Vector3.zero)
            {
                m_Offset = transform.position - m_Target.position;
            }
        }

        private void LateUpdate()
        {
            if (m_Target == null)
            {
                return;
            }

            Vector3 desired = m_Target.position + m_Offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref m_Velocity, m_SmoothTime);
        }
    }
}
