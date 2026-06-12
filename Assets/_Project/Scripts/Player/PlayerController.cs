using TopdownSurvival.Combat;
using TopdownSurvival.Input;
using UnityEngine;

namespace TopdownSurvival.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        private enum NoTargetFacingMode
        {
            FaceMoveDirection,
            KeepLast
        }

        private const float k_MinDirectionSqrMagnitude = 0.0001f;
        private const float k_GroundedVerticalVelocity = -2f;

        [Header("References")]
        [SerializeField] private CharacterController m_Controller;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private MonoBehaviour m_MovementInputSource;
        [SerializeField] private MonoBehaviour m_AimProviderSource;

        [Header("Movement")]
        [SerializeField] private float m_MaxSpeed = 3.5f;
        [SerializeField] private float m_MoveDeadzone = 0.15f;
        [SerializeField] private float m_Gravity = -20f;

        [Header("Facing")]
        [SerializeField] private float m_TurnSpeedDegPerSec = 720f;
        [SerializeField] private NoTargetFacingMode m_NoTargetFacing = NoTargetFacingMode.FaceMoveDirection;

        [Header("Animation")]
        [SerializeField] private float m_LocomotionDamp = 0.1f;

        private static readonly int s_MoveXHash = Animator.StringToHash("moveX");
        private static readonly int s_MoveZHash = Animator.StringToHash("moveZ");
        private static readonly int s_SpeedHash = Animator.StringToHash("speed");

        private IMovementInput m_MovementInput;
        private IAimTargetProvider m_AimProvider;
        private float m_VerticalVelocity;

        private void Awake()
        {
            if (m_Controller == null)
            {
                m_Controller = GetComponent<CharacterController>();
            }

            m_MovementInput = m_MovementInputSource as IMovementInput;
            m_AimProvider = m_AimProviderSource as IAimTargetProvider;

            if (m_MovementInput == null)
            {
                Debug.LogError($"{nameof(PlayerController)} on '{name}' needs a movement input source implementing {nameof(IMovementInput)}.", this);
            }

            if (m_AimProvider == null)
            {
                Debug.LogError($"{nameof(PlayerController)} on '{name}' needs an aim provider source implementing {nameof(IAimTargetProvider)}.", this);
            }

            if (m_Animator == null)
            {
                Debug.LogError($"{nameof(PlayerController)} on '{name}' has no Animator assigned.", this);
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            Vector3 worldMove = ReadWorldMove();
            Move(worldMove, deltaTime);
            UpdateFacing(worldMove, deltaTime);
            UpdateAnimator(worldMove, deltaTime);
        }

        private Vector3 ReadWorldMove()
        {
            if (m_MovementInput == null)
            {
                return Vector3.zero;
            }

            Vector2 input = m_MovementInput.Move;
            if (input.magnitude < m_MoveDeadzone)
            {
                return Vector3.zero;
            }

            return new Vector3(input.x, 0f, input.y).normalized;
        }

        private void Move(Vector3 worldMove, float deltaTime)
        {
            if (m_Controller == null)
            {
                return;
            }

            if (m_Controller.isGrounded && m_VerticalVelocity < 0f)
            {
                m_VerticalVelocity = k_GroundedVerticalVelocity;
            }
            else
            {
                m_VerticalVelocity += m_Gravity * deltaTime;
            }

            Vector3 velocity = worldMove * m_MaxSpeed;
            velocity.y = m_VerticalVelocity;
            m_Controller.Move(velocity * deltaTime);
        }

        private void UpdateFacing(Vector3 worldMove, float deltaTime)
        {
            Vector3 desiredDirection = Vector3.zero;

            if (m_AimProvider != null && m_AimProvider.TryGetTarget(out _))
            {
                desiredDirection = m_AimProvider.AimDirection;
            }
            else if (m_NoTargetFacing == NoTargetFacingMode.FaceMoveDirection)
            {
                desiredDirection = worldMove;
            }

            desiredDirection.y = 0f;
            if (desiredDirection.sqrMagnitude < k_MinDirectionSqrMagnitude)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_TurnSpeedDegPerSec * deltaTime);
        }

        private void UpdateAnimator(Vector3 worldMove, float deltaTime)
        {
            if (m_Animator == null)
            {
                return;
            }

            Vector3 localMove = transform.InverseTransformDirection(worldMove);
            m_Animator.SetFloat(s_MoveXHash, localMove.x, m_LocomotionDamp, deltaTime);
            m_Animator.SetFloat(s_MoveZHash, localMove.z, m_LocomotionDamp, deltaTime);
            m_Animator.SetFloat(s_SpeedHash, worldMove.magnitude, m_LocomotionDamp, deltaTime);
        }
    }
}
