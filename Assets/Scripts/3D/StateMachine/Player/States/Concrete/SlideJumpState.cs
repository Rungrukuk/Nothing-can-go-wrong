
using UnityEngine;

public class SlideJumpState : PlayerState
{
    private float jumpEarlyTimer;
    private bool shouldNotEarlyJump;
    private float activateEalyJumpTimer;
    public SlideJumpState(PlayerCharacterController controller, PlayerStateMachine stateMachine) : base(controller, stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        controller.IsSliding = false;
        controller.CharacterVelocity = new Vector3(controller.CharacterVelocity.x, 0f, controller.CharacterVelocity.z);
        controller.CharacterVelocity += controller.GroundNormal * controller.characterConfig.jumpForce;
        controller.LastTimeJumped = Time.time;
        controller.IsGrounded = false;
        shouldNotEarlyJump = false;
        controller.EarlyJumpActivated = false;
        activateEalyJumpTimer = CharacterConfig.k_activateEalyJumpTimer;
    }

    public override void Update()
    {
        base.Update();
        ApplyCharacterVelocity(controller.characterConfig.accelerationRateInAir, 0, controller.characterConfig.accelerationRateInAir * controller.characterConfig.slideJumpBoost);
        if (controller.inputHandler.GetJumpInputDown() && !shouldNotEarlyJump && activateEalyJumpTimer <= 0)
        {
            jumpEarlyTimer = controller.characterConfig.earlyJumpTime;
            controller.EarlyJumpActivated = true;
        }
        if (jumpEarlyTimer <= 0 && activateEalyJumpTimer <= 0 && controller.EarlyJumpActivated)
        {
            shouldNotEarlyJump = true;
            controller.EarlyJumpActivated = false;
        }
        activateEalyJumpTimer -= Time.deltaTime;
        jumpEarlyTimer -= Time.deltaTime;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void HandleStateChange()
    {
        if (controller.IsGrounded)
        {
            if (controller.CharacterVelocity.magnitude >= controller.characterConfig.minSpeedToSlide && controller.inputHandler.GetCrouchInputHeld())
            {
                stateMachine.ChangeState(new SpeedSlideState(controller, stateMachine));
            }
            if (!controller.inputHandler.GetMoveInput().Equals(Vector3.zero) && controller.inputHandler.GetSprintInputHeld() && !controller.inputHandler.GetCrouchInputHeld())
            {
                stateMachine.ChangeState(new SprintState(controller, stateMachine));
            }
            if (!controller.inputHandler.GetMoveInput().Equals(Vector3.zero) && !controller.inputHandler.GetSprintInputHeld() && !controller.inputHandler.GetCrouchInputHeld())
            {
                stateMachine.ChangeState(new WalkState(controller, stateMachine));
            }
            if (controller.inputHandler.GetMoveInput().Equals(Vector3.zero) && !controller.inputHandler.GetSprintInputHeld() && !controller.inputHandler.GetCrouchInputHeld())
            {
                stateMachine.ChangeState(new IdleState(controller, stateMachine));
            }
        }
    }
    public override void Exit()
    {
        base.Exit();
        controller.TargetCharacterHeight = controller.characterConfig.capsuleHeightStanding;
    }
}
