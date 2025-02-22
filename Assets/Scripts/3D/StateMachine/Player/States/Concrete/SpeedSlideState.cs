
using UnityEngine;

public class SpeedSlideState : PlayerState
{
    private float currentSlideMultiplier;
    private Vector3 slideDirection;

    private float slideDelayTimer;
    public SpeedSlideState(PlayerCharacterController controller, PlayerStateMachine stateMachine)
    : base(controller, stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        currentSlideMultiplier = controller.characterConfig.slideMultiplier;
        slideDirection = controller.transform.forward.normalized;
        controller.IsSliding = true;
        ApplyCharacterVelocity(0, 0, currentSlideMultiplier, slideDirection);
        controller.TargetCharacterHeight = controller.characterConfig.capsuleHeightCrouching;
        slideDelayTimer = controller.characterConfig.slideDelayTime;
    }

    public override void Update()
    {
        base.Update();
        if (slideDelayTimer > 0)
        {
            slideDelayTimer -= Time.deltaTime;
            return;
        }
        currentSlideMultiplier = Mathf.Lerp(
            currentSlideMultiplier,
            controller.characterConfig.crouchSpeedMultiplier,
            Time.deltaTime * controller.characterConfig.slidingDecreaseRate
        );
        ApplyCharacterVelocity(0, 0, currentSlideMultiplier, slideDirection);
    }

    public override void HandleStateChange()
    {
        base.HandleStateChange();
        if (controller.inputHandler.GetJumpInputDown() || controller.EarlyJumpActivated && controller.IsSliding)
        {
            if (controller.EarlyJumpActivated)
            {
                Debug.Log("I'm in");
            }
            stateMachine.ChangeState(new SlideJumpState(controller, stateMachine));
        }
        if (!controller.inputHandler.GetCrouchInputHeld()
        )
        {
            stateMachine.ChangeState(new IdleState(controller, stateMachine));
        }
        if (controller.CharacterVelocity.magnitude <= controller.characterConfig.maxWalkSpeedOnGround * controller.characterConfig.crouchSpeedMultiplier + speeedError
        && Vector3.Angle(controller.GroundNormal, controller.transform.forward) >= controller.characterConfig.maxAngleToSlide
        || Vector3.Angle(controller.GroundNormal, controller.transform.forward) > controller.characterConfig.minAngleToSlide)
        {
            stateMachine.ChangeState(new CrouchWalkState(controller, stateMachine));
        }
        if (Vector3.Angle(controller.GroundNormal, controller.transform.forward) < controller.characterConfig.maxAngleToSlide)
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
        controller.EarlyJumpActivated = false;
        controller.IsSliding = false;
    }

    // private void ApplySlideVelocity()
    // {



    //     if (slideVelocity.magnitude > controller.CharacterVelocity.magnitude)
    //         controller.CharacterVelocity = Vector3.Lerp(controller.CharacterVelocity, slideVelocity, Time.deltaTime * controller.characterConfig.accelerationModifier);
    //     else
    //         controller.CharacterVelocity = Vector3.Lerp(controller.CharacterVelocity, slideVelocity, Time.deltaTime * controller.characterConfig.decelarationModifier);
    // }

}
