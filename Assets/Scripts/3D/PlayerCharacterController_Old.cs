using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(AudioSource))]
public class PlayerCharacterController_Old : MonoBehaviour
{
    [Header("References")]
    public Camera PlayerCamera;
    public AudioSource AudioSource;

    [Header("General")]
    public float GravityDownForce = 20f;
    public LayerMask GroundCheckLayers = -1;
    public float GroundCheckDistance = 0.05f;

    [Header("Movement")]
    public float MaxSpeedOnGround = 10f;
    public float MovementSharpnessOnGround = 15f;
    [Range(0, 1)] public float MaxSpeedCrouchedRatio = 0.5f;
    public float MaxSpeedInAir = 10f;
    public float AccelerationSpeedInAir = 25f;
    public float SprintSpeedModifier = 2f;
    [Range(1, 2)] public float SlideSpeedModifier = 1.5f;
    [Range(1, 3)] public float SlideJumpSpeedModifier = 1.5f;

    [Header("Rotation")]
    public float RotationSpeed = 200f;
    [Range(0.1f, 1f)] public float AimingRotationMultiplier = 0.4f;

    [Header("Jump")]
    public float JumpForce = 9f;

    [Header("Stance")]
    public float CameraHeightRatio = 0.9f;
    public float CapsuleHeightStanding = 1.8f;
    public float CapsuleHeightCrouching = 0.9f;
    public float CrouchingSharpness = 10f;

    [Header("Audio")]
    public float FootstepSfxFrequency = 1f;
    public float FootstepSfxFrequencyWhileSprinting = 1f;
    public AudioClip FootstepSfx;
    public AudioClip JumpSfx;
    public AudioClip LandSfx;

    [Header("Slide Settings")]
    [SerializeField] private float SlideBoostMultiplier = 1.5f;
    [SerializeField] private float SlideDecayRate = 2f;

    [SerializeField] private TMP_Text debugText;

    [Header("Animation Settings")]

    public UnityAction<bool> OnStanceChanged;

    public Vector3 CharacterVelocity { get; set; }
    public bool IsGrounded { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsSlidingFromSlope { get; private set; }
    public bool IsSliding { get; private set; }
    public bool IsSlideJumping { get; private set; }
    public Vector3 SlideDirection { get; private set; }

    public float RotationMultiplier => 1f; // Placeholder for aiming logic

    private PlayerInputHandler m_InputHandler;
    private CharacterController m_Controller;
    private Vector3 m_GroundNormal;
    private float m_LastTimeJumped;
    private float m_CameraVerticalAngle;
    private float m_FootstepDistanceCounter;
    private float m_TargetCharacterHeight;
    private float m_CurrentSlideSpeed;

    private const float k_JumpGroundingPreventionTime = 0.2f;
    private const float k_GroundCheckDistanceInAir = 0.07f;

    void Start()
    {
        IsSlideJumping = false;
        m_Controller = GetComponent<CharacterController>();
        m_InputHandler = GetComponent<PlayerInputHandler>();
        m_Controller.enableOverlapRecovery = true;

        UpdateCharacterHeight(true);
        IsCrouching = false;
    }

    void Update()
    {
        debugText.text = "Velocity: " + CharacterVelocity.magnitude;
        bool wasGrounded = IsGrounded;
        GroundCheck();

        if (IsGrounded && !wasGrounded)
        {
            AudioSource.PlayOneShot(LandSfx);
        }
        Debug.Log("Target_Height:" + m_TargetCharacterHeight);
        Debug.Log("Controller Height:" + m_Controller.height);

        HandleCrouching();
        UpdateCharacterHeight(false);
        HandleCharacterMovement();
    }

    void GroundCheck()
    {
        float chosenGroundCheckDistance = IsGrounded ? (m_Controller.skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;
        IsGrounded = false;
        m_GroundNormal = Vector3.up;

        if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
        {
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height),
                m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
                QueryTriggerInteraction.Ignore))
            {
                m_GroundNormal = hit.normal;

                if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(m_GroundNormal))
                {
                    IsGrounded = true;
                    if (hit.distance > m_Controller.skinWidth)
                    {
                        m_Controller.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    void HandleCharacterMovement()
    {
        // Horizontal rotation
        transform.Rotate(new Vector3(0f, m_InputHandler.GetLookInputsHorizontal() * RotationSpeed * RotationMultiplier, 0f), Space.Self);

        // Vertical camera rotation
        m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * RotationSpeed * RotationMultiplier;
        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);
        PlayerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);

        // Movement handling
        bool isSprinting = m_InputHandler.GetSprintInputHeld();

        float speedModifier = isSprinting ? SprintSpeedModifier : 1f;
        Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

        if (IsGrounded)
        {
            Vector3 targetVelocity = worldspaceMoveInput * MaxSpeedOnGround * speedModifier;
            targetVelocity = HandleSliding(targetVelocity);

            if (IsCrouching && !IsSliding && !IsSlidingFromSlope)
            {
                targetVelocity *= MaxSpeedCrouchedRatio;
            }

            targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) * targetVelocity.magnitude;
            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, MovementSharpnessOnGround * Time.deltaTime);

            if (m_InputHandler.GetJumpInputDown())
            {
                if (IsSlidingFromSlope)
                {
                    CharacterVelocity = new Vector3(CharacterVelocity.x * SlideJumpSpeedModifier, 0f, CharacterVelocity.z * SlideJumpSpeedModifier);
                    IsSlideJumping = true;
                }
                else
                {
                    CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
                    IsSlideJumping = false;
                }

                CharacterVelocity += m_GroundNormal * JumpForce;
                AudioSource.PlayOneShot(JumpSfx);
                m_LastTimeJumped = Time.time;
                IsGrounded = false;
                m_GroundNormal = Vector3.up;
            }

            float chosenFootstepSfxFrequency = isSprinting ? FootstepSfxFrequencyWhileSprinting : FootstepSfxFrequency;
            if (m_FootstepDistanceCounter >= 1f / chosenFootstepSfxFrequency)
            {
                m_FootstepDistanceCounter = 0f;
                AudioSource.PlayOneShot(FootstepSfx);
            }

            m_FootstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
        }
        else
        {
            IsSlidingFromSlope = false;
            IsSliding = false;
            CharacterVelocity += worldspaceMoveInput * AccelerationSpeedInAir * Time.deltaTime;

            float verticalVelocity = CharacterVelocity.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);

            if (IsSlideJumping)
            {
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir * SlideJumpSpeedModifier * speedModifier);
            }
            else
            {
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir * speedModifier);
            }

            CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
            CharacterVelocity += Vector3.down * GravityDownForce * Time.deltaTime;
        }

        m_Controller.Move(CharacterVelocity * Time.deltaTime);
    }

    bool IsNormalUnderSlopeLimit(Vector3 normal) => Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;

    Vector3 GetCapsuleBottomHemisphere() => transform.position + (transform.up * m_Controller.radius);

    Vector3 GetCapsuleTopHemisphere(float atHeight) => transform.position + (transform.up * (atHeight - m_Controller.radius));

    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }

    void UpdateCharacterHeight(bool force)
    {
        if (force)
        {
            m_Controller.height = m_TargetCharacterHeight;
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            PlayerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * CameraHeightRatio;
        }
        else if (m_Controller.height != m_TargetCharacterHeight)
        {
            m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight, CrouchingSharpness * Time.deltaTime);
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            PlayerCamera.transform.localPosition = Vector3.Lerp(PlayerCamera.transform.localPosition,
                Vector3.up * m_TargetCharacterHeight * CameraHeightRatio, CrouchingSharpness * Time.deltaTime);
        }
    }

    void HandleCrouching()
    {
        if (IsGrounded && m_InputHandler.GetCrouchInputHeld())
        {
            IsCrouching = true;
            m_TargetCharacterHeight = CapsuleHeightCrouching;
        }
        if (!m_InputHandler.GetCrouchInputHeld())
        {
            IsCrouching = false;
            m_TargetCharacterHeight = CapsuleHeightStanding;
        }
    }

    Vector3 HandleSliding(Vector3 targetVelocity)
    {
        const float slopeAngleThreshold = 85f;
        float crouchSpeed = MaxSpeedOnGround * MaxSpeedCrouchedRatio;

        if (IsCrouching && Vector3.Angle(m_GroundNormal, transform.forward) < slopeAngleThreshold)
        {
            if (!IsSlidingFromSlope)
            {
                SlideDirection = transform.forward;
                IsSlidingFromSlope = true;
                m_CurrentSlideSpeed = Mathf.Max(CharacterVelocity.magnitude, crouchSpeed) * SlideBoostMultiplier;
            }
            targetVelocity = m_CurrentSlideSpeed * SlideDirection;
        }
        else if (IsCrouching && CharacterVelocity.magnitude > 15f && Vector3.Angle(m_GroundNormal, transform.forward) < slopeAngleThreshold + 8)
        {
            if (!IsSliding)
            {
                SlideDirection = transform.forward.normalized;
                IsSliding = true;
                m_CurrentSlideSpeed = Mathf.Max(CharacterVelocity.magnitude * SlideBoostMultiplier, crouchSpeed);
            }
            m_CurrentSlideSpeed = Mathf.Lerp(m_CurrentSlideSpeed, crouchSpeed, SlideDecayRate * Time.deltaTime);
            targetVelocity = m_CurrentSlideSpeed * SlideDirection;
        }
        else
        {
            IsSliding = false;
            IsSlidingFromSlope = false;
        }

        return targetVelocity;
    }

    private void OnDrawGizmos()
    {
        if (m_Controller == null) return;

        float chosenGroundCheckDistance = IsGrounded
            ? (m_Controller.skinWidth + CharacterConfig.k_GroundCheckDistance)
            : CharacterConfig.k_GroundCheckDistance;

        Vector3 bottom = GetCapsuleBottomHemisphere();
        Vector3 top = GetCapsuleTopHemisphere(m_Controller.height);
        Vector3 direction = Vector3.down;

        Gizmos.color = Color.red;

        if (Physics.CapsuleCast(bottom, top, m_Controller.radius, direction,
            out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers, QueryTriggerInteraction.Ignore))
        {
            Gizmos.color = Color.green;

            Gizmos.DrawSphere(hit.point, 0.1f);
        }

        Gizmos.DrawWireSphere(bottom, m_Controller.radius);
        Gizmos.DrawWireSphere(top, m_Controller.radius);
        Gizmos.DrawLine(bottom, top);

        Gizmos.DrawRay((bottom + top) / 2, direction * chosenGroundCheckDistance);
    }
}