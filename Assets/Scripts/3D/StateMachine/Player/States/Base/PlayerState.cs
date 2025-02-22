using UnityEngine;

public abstract class PlayerState
{
    protected PlayerCharacterController controller;
    protected PlayerStateMachine stateMachine;
    protected Vector3 targetVelocity;
    protected const float speeedError = 1f;

    public PlayerState(PlayerCharacterController controller, PlayerStateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {

    }
    public virtual void Update()
    {
        if (!controller.IsGrounded)
        {
            controller.CharacterVelocity += Vector3.down * CharacterConfig.GravityDownForce * Time.deltaTime;
        }
    }
    public virtual void HandleStateChange()
    {
        if (controller.inputHandler.GetJumpInputDown() && controller.IsGrounded && !controller.IsSliding)
        {
            stateMachine.ChangeState(new JumpState(controller, stateMachine));
        }
    }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    protected void ApplyCharacterVelocity(float speedMultiplierX = 1f, float speedMultiplierY = 0f, float speedMultiplierZ = 1f, Vector3 direction = default)
    {
        Vector3 localMoveInput = controller.inputHandler.GetMoveInput();
        localMoveInput.x *= speedMultiplierX;
        localMoveInput.z *= speedMultiplierZ;

        Vector3 worldspaceMoveInput = controller.transform.TransformVector(localMoveInput);
        if (controller.IsGrounded)
        {
            if (!controller.IsSliding)
            {
                targetVelocity = controller.characterConfig.maxWalkSpeedOnGround * worldspaceMoveInput;
                targetVelocity.y *= speedMultiplierY;
            }
            else if (controller.IsSliding)
            {
                targetVelocity = direction * controller.characterConfig.maxWalkSpeedOnGround * speedMultiplierZ;
                targetVelocity.y *= speedMultiplierY;
            }
            if (targetVelocity.magnitude > controller.CharacterVelocity.magnitude)
                controller.CharacterVelocity = Vector3.Lerp(controller.CharacterVelocity, targetVelocity, Time.deltaTime * controller.characterConfig.accelerationModifier);
            else
                controller.CharacterVelocity = Vector3.Lerp(controller.CharacterVelocity, targetVelocity, Time.deltaTime * controller.characterConfig.decelarationModifier);
        }
        else
        {
            controller.CharacterVelocity += worldspaceMoveInput * controller.characterConfig.accelerationModifier * Time.deltaTime;
            float verticalVelocity = controller.CharacterVelocity.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(controller.CharacterVelocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, controller.characterConfig.maxSpeedInAir * speedMultiplierZ);
            controller.CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
        }
    }
}
