using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Character Config")]
public class CharacterConfig : ScriptableObject
{
    [Header("Movement Settings")]
    public float maxWalkSpeedOnGround = 5f;
    public float maxSprintSpeed = 8f;
    public float maxSpeedInAir = 5f;
    public float accelerationModifier = 3f;
    public float decelarationModifier = 15f;

    [Header("Crouch Settings")]
    public float crouchingSharpness = 15f;
    public float capsuleHeightStanding = 1.8f;
    public float capsuleHeightCrouching = 1.8f;
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Slide Settings")]
    public float minSpeedToSlide = 1f;
    public float slideMultiplier = 3f;
    public float slidingDecreaseRate = 2f;
    public float maxAngleToSlide = 85f;
    public float minAngleToSlide = 95f;
    public float slideDelayTime = 2f;
    public float slideSpeedIncreaseRate = 2f;
    [Header("Camera Settings")]
    public float rotationMultiplier = 6f;
    public const float k_RotationSpeed = 200f;
    public const float k_CameraHeightRatio = 0.8f;


    public const float k_GroundCheckDistanceInAir = 0.07f;
    public const float k_GroundCheckDistance = 1f;
    public const float k_JumpGroundingPreventionTime = 0.2f;
    public const float GravityDownForce = 20f;
    // public float slopeSlideSpeed = 10f;
    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float accelerationRateInAir = 3.75f;
    public float slideJumpBoost = 1.5f;
    public float earlyJumpTime = 2f;
    public const float k_activateEalyJumpTimer = 0.0001f;


    // public float slideJumpBoost = 3f;
    // public float acceleration = 5f;
    // public float deceleration = 7f;
}
