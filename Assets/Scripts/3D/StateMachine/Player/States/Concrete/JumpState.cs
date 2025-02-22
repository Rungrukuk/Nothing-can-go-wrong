
using UnityEngine;
using UnityEngine.TextCore.Text;

public class JumpState : PlayerState
{
    public JumpState(PlayerCharacterController controller, PlayerStateMachine stateMachine) : base(controller, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        controller.CharacterVelocity = new Vector3(controller.CharacterVelocity.x, 0f, controller.CharacterVelocity.z);
        controller.CharacterVelocity += controller.GroundNormal * controller.characterConfig.jumpForce;
        controller.LastTimeJumped = Time.time;
        controller.IsGrounded = false;
        controller.IsSliding = false;
    }

    public override void Update()
    {
        base.Update();
        ApplyCharacterVelocity(controller.characterConfig.accelerationRateInAir, 0, controller.characterConfig.accelerationRateInAir);
    }
    public override void HandleStateChange()
    {
        if (controller.IsGrounded)
        {
            if (controller.CharacterVelocity.magnitude >= controller.characterConfig.minSpeedToSlide && controller.inputHandler.GetCrouchInputHeld())
            {
                Debug.Log("I'm in");
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
            if (controller.inputHandler.GetCrouchInputHeld() && controller.inputHandler.GetMoveInput().Equals(Vector3.zero) && controller.CharacterVelocity.magnitude < controller.characterConfig.minSpeedToSlide)
            {
                stateMachine.ChangeState(new CrouchState(controller, stateMachine));
            }
            if (controller.inputHandler.GetCrouchInputHeld() && !controller.inputHandler.GetMoveInput().Equals(Vector3.zero) && controller.CharacterVelocity.magnitude < controller.characterConfig.minSpeedToSlide)
            {
                stateMachine.ChangeState(new CrouchWalkState(controller, stateMachine));
            }
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void Exit()
    {
        base.Exit();
    }
}
