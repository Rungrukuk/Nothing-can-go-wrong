
using UnityEngine;

public class CrouchState : PlayerState
{
    public CrouchState(PlayerCharacterController controller, PlayerStateMachine stateMachine) : base(controller, stateMachine)
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
        ApplyCharacterVelocity(0, 0, 0);
    }
    public override void HandleStateChange()
    {
        base.HandleStateChange();
        if (!controller.inputHandler.GetCrouchInputHeld())
        {
            stateMachine.ChangeState(new IdleState(controller, stateMachine));
        }
        if (controller.IsGrounded && controller.inputHandler.GetMoveInput() != Vector3.zero)
        {
            stateMachine.ChangeState(new CrouchWalkState(controller, stateMachine));
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
