
using UnityEngine;

public class SlopeSlideState : PlayerState
{
    private float currentSlideMultiplier;

    public SlopeSlideState(PlayerCharacterController controller, PlayerStateMachine stateMachine)
    : base(controller, stateMachine)
    {
    }
    private Vector3 slideDirection;
    public override void Enter()
    {
        base.Enter();
        currentSlideMultiplier = controller.characterConfig.slideMultiplier;
        slideDirection = controller.transform.forward.normalized;
        controller.IsSliding = true;
        ApplyCharacterVelocity(0, 0, currentSlideMultiplier, slideDirection);
        controller.TargetCharacterHeight = controller.characterConfig.capsuleHeightCrouching;
    }

    public override void Update()
    {
        base.Update();
        if (Vector3.Angle(controller.GroundNormal, controller.transform.forward) >= controller.characterConfig.maxAngleToSlide)
        {
            currentSlideMultiplier = Mathf.Lerp(
                currentSlideMultiplier,
                controller.characterConfig.crouchSpeedMultiplier,
                Time.deltaTime * controller.characterConfig.slidingDecreaseRate
            );
        }
        else
        {
            currentSlideMultiplier += controller.characterConfig.slideSpeedIncreaseRate * Time.deltaTime;
        }
        ApplyCharacterVelocity(0, 0, currentSlideMultiplier, slideDirection);

    }
    public override void HandleStateChange()
    {
        base.HandleStateChange();
        if (!controller.inputHandler.GetCrouchInputHeld()
        // && controller.CharacterVelocity.magnitude <= controller.characterConfig.maxWalkSpeedOnGround * controller.characterConfig.crouchSpeedMultiplier + speeedError
        )
        {
            stateMachine.ChangeState(new IdleState(controller, stateMachine));
        }
        if (controller.inputHandler.GetJumpInputDown() && controller.IsSliding)
        {
            stateMachine.ChangeState(new SlideJumpState(controller, stateMachine));
        }
        if (Vector3.Angle(controller.GroundNormal, controller.transform.forward) >= controller.characterConfig.maxAngleToSlide
        && controller.CharacterVelocity.magnitude < controller.characterConfig.minSpeedToSlide)
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
