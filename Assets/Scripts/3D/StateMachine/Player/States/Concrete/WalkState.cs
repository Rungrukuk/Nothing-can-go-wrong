using UnityEngine;

public class WalkState : PlayerState
{
    public WalkState(PlayerCharacterController controller, PlayerStateMachine stateMachine)
        : base(controller, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        controller.IsSliding = false;
    }

    public override void Update()
    {
        base.Update();
        ApplyCharacterVelocity();
    }
    public override void HandleStateChange()
    {
        base.HandleStateChange();
        if (controller.inputHandler.GetSprintInputHeld())
        {
            stateMachine.ChangeState(new SprintState(controller, stateMachine));
        }
        if (controller.inputHandler.GetCrouchInputHeld()
        && controller.CharacterVelocity.magnitude < controller.characterConfig.minSpeedToSlide
        && Vector3.Angle(controller.GroundNormal, controller.transform.forward) >= controller.characterConfig.maxAngleToSlide)
        {
            stateMachine.ChangeState(new CrouchWalkState(controller, stateMachine));
        }
        if (controller.inputHandler.GetCrouchInputHeld() && Vector3.Angle(controller.GroundNormal, controller.transform.forward) < controller.characterConfig.maxAngleToSlide)
        {
            stateMachine.ChangeState(new SlopeSlideState(controller, stateMachine));
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