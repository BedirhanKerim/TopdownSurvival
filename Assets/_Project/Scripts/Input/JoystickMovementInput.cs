using UnityEngine;

namespace TopdownSurvival.Input
{
    public sealed class JoystickMovementInput : MonoBehaviour, IMovementInput
    {
        [SerializeField] private Joystick m_Joystick;
        [SerializeField] private float m_DeadZone = 0.15f;

        public Vector2 Move
        {
            get
            {
                if (m_Joystick == null)
                {
                    return Vector2.zero;
                }

                Vector2 direction = m_Joystick.Direction;
                if (direction.sqrMagnitude < m_DeadZone * m_DeadZone)
                {
                    return Vector2.zero;
                }

                return direction;
            }
        }

        private void Awake()
        {
            if (m_Joystick == null)
            {
                Debug.LogError($"{nameof(JoystickMovementInput)} on '{name}' has no Joystick assigned.", this);
            }
        }
    }
}
