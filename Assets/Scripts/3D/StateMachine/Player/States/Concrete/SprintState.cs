using UnityEngine;

public class SprintState : PlayerState
{
    float speedMultiplier;
    public SprintState(PlayerCharacterController controller, PlayerStateMachine stateMachine)
        : base(controller, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        speedMultiplier = controller.characterConfig.maxSprintSpeed / controller.characterConfig.maxWalkSpeedOnGround;
        controller.IsSliding = false;
    }

    public override void Update()
    {
        base.Update();
        ApplyCharacterVelocity(speedMultiplier, 0, speedMultiplier);
    }
    public override void HandleStateChange()
    {
        base.HandleStateChange();
        if (controller.inputHandler.GetCrouchInputHeld() && controller.IsGrounded
        && controller.CharacterVelocity.magnitude >= controller.characterConfig.minSpeedToSlide
        && Vector3.Angle(controller.GroundNormal, controller.transform.forward) <= controller.characterConfig.minAngleToSlide)
        {
            stateMachine.ChangeState(new SpeedSlideState(controller, stateMachine));
        }
        if (!controller.inputHandler.GetSprintInputHeld() && controller.IsGrounded)
        {
            stateMachine.ChangeState(new IdleState(controller, stateMachine));
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
