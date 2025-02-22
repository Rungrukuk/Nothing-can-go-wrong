using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler))]
public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] private LayerMask groundCheckLayers;
    [SerializeField] private Camera playerCamera;
    [SerializeField] public CharacterConfig characterConfig;

    [SerializeField] private TMP_Text debugText;

    private CharacterController characterController;


    public PlayerInputHandler inputHandler { get; private set; }
    public bool IsGrounded { get; set; }
    public Vector3 CharacterVelocity { get; set; }
    public bool IsSliding { get; set; }
    public bool EarlyJumpActivated { get; set; }
    public Vector3 GroundNormal { get; private set; }
    public float LastTimeJumped { get; set; }
    public PlayerStateMachine stateMachine { get; private set; }
    public float TargetCharacterHeight { get; set; }


    private float cameraVerticalAngle;


    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
        stateMachine.Initialize(new IdleState(this, stateMachine));
        UpdateCharacterHeight(true);
    }

    private void Update()
    {
        HandleCameraMovement();
        GroundCheck();
        stateMachine.CurrentState.HandleStateChange();
        stateMachine.CurrentState.Update();
        ApplyCharacterVelocity();
        debugText.text = "Velocity: " + CharacterVelocity.magnitude + "\n" + "Current State: " + stateMachine.CurrentState;
        UpdateCharacterHeight(false);
        Debug.Log(EarlyJumpActivated);
    }

    private void FixedUpdate()
    {
        stateMachine.CurrentState.FixedUpdate();
    }


    #region Helper Methods
    void GroundCheck()
    {
        float chosenGroundCheckDistance = IsGrounded ? (characterController.skinWidth + CharacterConfig.k_GroundCheckDistance) : CharacterConfig.k_GroundCheckDistanceInAir;
        IsGrounded = false;
        GroundNormal = Vector3.up;

        if (Time.time >= LastTimeJumped + CharacterConfig.k_JumpGroundingPreventionTime)
        {
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(characterController.height),
                characterController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers,
                QueryTriggerInteraction.Ignore))
            {

                GroundNormal = hit.normal;

                if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(GroundNormal))
                {
                    IsGrounded = true;
                    if (hit.distance > characterController.skinWidth)
                    {
                        characterController.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }
    private void HandleCameraMovement()
    {
        // Horizontal rotation
        transform.Rotate(new Vector3(0f, inputHandler.GetLookInputsHorizontal() * CharacterConfig.k_RotationSpeed * characterConfig.rotationMultiplier, 0f), Space.Self);

        // Vertical camera rotation
        cameraVerticalAngle += inputHandler.GetLookInputsVertical() * CharacterConfig.k_RotationSpeed * characterConfig.rotationMultiplier;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);
        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
    }

    private void ApplyCharacterVelocity()
    {
        characterController.Move(CharacterVelocity * Time.deltaTime);
    }


    bool IsNormalUnderSlopeLimit(Vector3 normal) => Vector3.Angle(transform.up, normal) <= characterController.slopeLimit;

    Vector3 GetCapsuleBottomHemisphere() => transform.position + (transform.up * characterController.radius);

    Vector3 GetCapsuleTopHemisphere(float atHeight) => transform.position + (transform.up * (atHeight - characterController.radius));


    public void UpdateCharacterHeight(bool force)
    {
        if (force)
        {
            characterController.height = TargetCharacterHeight;
            characterController.center = Vector3.up * characterController.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.up * TargetCharacterHeight * CharacterConfig.k_CameraHeightRatio;
        }
        else if (characterController.height != TargetCharacterHeight)
        {
            characterController.height = Mathf.Lerp(characterController.height, TargetCharacterHeight, characterConfig.crouchingSharpness * Time.deltaTime);
            characterController.center = Vector3.up * characterController.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition,
                Vector3.up * TargetCharacterHeight * CharacterConfig.k_CameraHeightRatio, characterConfig.crouchingSharpness * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (characterController == null) return;

        float chosenGroundCheckDistance = IsGrounded
            ? (characterController.skinWidth + CharacterConfig.k_GroundCheckDistance)
            : CharacterConfig.k_GroundCheckDistance;

        Vector3 bottom = GetCapsuleBottomHemisphere();
        Vector3 top = GetCapsuleTopHemisphere(characterController.height);
        Vector3 direction = Vector3.down;

        Gizmos.color = Color.red;

        if (Physics.CapsuleCast(bottom, top, characterController.radius, direction,
            out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore))
        {
            Gizmos.color = Color.green;

            Gizmos.DrawSphere(hit.point, 0.1f);
        }

        Gizmos.DrawWireSphere(bottom, characterController.radius);
        Gizmos.DrawWireSphere(top, characterController.radius);
        Gizmos.DrawLine(bottom, top);

        Gizmos.DrawRay((bottom + top) / 2, direction * chosenGroundCheckDistance);
    }

    #endregion
}
