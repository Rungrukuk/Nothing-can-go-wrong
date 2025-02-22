using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerCharacterController controller, PlayerStateMachine stateMachine)
        : base(controller, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        controller.TargetCharacterHeight = controller.characterConfig.capsuleHeightStanding;
        controller.IsSliding = false;
    }

    public override void Update()
    {
        base.Update();
        if (controller.IsGrounded && controller.inputHandler.GetMoveInput() == Vector3.zero)
        {
            ApplyCharacterVelocity(0, 0, 0);
        }
    }
    public override void HandleStateChange()
    {
        base.HandleStateChange();
        if (controller.IsGrounded && controller.inputHandler.GetMoveInput() != Vector3.zero)
        {
            stateMachine.ChangeState(new WalkState(controller, stateMachine));
        }
        if (controller.inputHandler.GetCrouchInputHeld() && controller.IsGrounded && controller.CharacterVelocity == Vector3.zero)
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
    }
}