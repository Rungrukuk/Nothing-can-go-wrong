
using UnityEngine;

public class CrouchWalkState : PlayerState
{
    public CrouchWalkState(PlayerCharacterController controller, PlayerStateMachine stateMachine) : base(controller, stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        controller.TargetCharacterHeight = controller.characterConfig.capsuleHeightCrouching;
        controller.IsSliding = false;
    }

    public override void Update()
    {
        base.Update();
        ApplyCharacterVelocity(controller.characterConfig.crouchSpeedMultiplier, 0, controller.characterConfig.crouchSpeedMultiplier);
    }
    public override void HandleStateChange()
    {
        base.HandleStateChange();
        if (!controller.inputHandler.GetCrouchInputHeld())
        {
            stateMachine.ChangeState(new IdleState(controller, stateMachine));
        }
        if (controller.IsGrounded && controller.inputHandler.GetMoveInput() == Vector3.zero)
        {
            stateMachine.ChangeState(new CrouchState(controller, stateMachine));
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        controller.TargetCharacterHeight = controller.characterConfig.capsuleHeightStanding;
    }
}
