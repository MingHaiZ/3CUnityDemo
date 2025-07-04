using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStats : EntityStats<PlayerStats>
{
    [Header("General Stats")]
    public float rotationSpeed = 970f;

    public float friction = 16f;
    public float gravityTopSpeed = 50f;
    public float gravity = 38f;
    public float fallGravity = 65f;
    public float pushForce = 4f;
    public float snapForce = 15f;

    [Header("Spin Stats")]
    public bool canSpin = true;

    public bool canAirSpin = true;
    public int canAirSpinCount = 1;
    public float airSignUpwardForce = 10f;
    public float spinDuration = 0.5f;

    [Header("Motion stats")]
    public float brakeThreshold = -0.8f;

    public float turningDrag = 28f;
    public float acceleration = 13f;
    public float topSpeed = 6f;
    public float airAcceleration = 32f;
    public float deceleration = 28f;
    public float slopeUpwardForce = 25f;
    public float slopeDownwardForce = 28f;

    [Header("Running stats")]
    public float runningAcceleration = 16f;

    public float runningTopSpeed = 7.5f;
    public float runningTurnningDrag = 14f;

    [Header("Backflip Stats")]
    public bool canBackflip = true;

    public bool backflipLockMovement = true;
    public float backflipJumpHeight = 23f;
    public float backflipGravity = 35f;
    public float backflipTurningDrag = 2.5f;
    public float backflipAirAcceleration = 12f;
    public float backflipTopSpeed = 7.5f;
    public float backflipBackwarForce = 4f;
    public float backflipBackwardTurnForce = 8f;

    [Header("Jump Stats")]
    public int multiJumps = 1;

    public float coyoteJumpThreshold = 0.15f;
    public float maxJumpHeight = 17f;
    public float minJumpHeight = 10f;

    [Header("Stomp Attack Stats")]
    public float stompAirTime = 0.8f;

    public bool canStompAttack = true;
    public float stompDownWardForce = 20f;
    public float stompGroundTime = 0.5f;
    public float stompGroundLeapHeight = 10f;

    [Header("Hurt Stats")]
    public float hurtBackwardsForce = 5f;

    public float hurtUpwardsForce = 10f;

    [Header("Pick'n Throw Stats")]
    public bool canPickUp = true;

    public bool canPickUpOnAir = false;
    public float throwVelocityMultiplier = 1.5f;
    public float pickUpDistance = 0.5f;

    [Header("Ledge Hanging Stats")]
    public bool canLedgeHang = true;

    public LayerMask ledgeHangingLayers;
    public Vector3 ledgeHangingSkinOffset;
    public float ledgeMaxForwardDistance = 0.25f;
    public float ledgeMaxDownwardDistance = 0.25f;
    public float ledgeSideMaxDistance = 0.5f;
    public float ledgeSideHeightOffset = 0.15f;
    public float ledgeSideCollisionRadius = 0.25f;
    public float ledgeMovementSpeed = 1.5f;

    [Header("Ledge Climbing Stats")]
    public bool canClimbLedge = true;

    public LayerMask ledgeClimbingLayers;
    public Vector3 ledgeClimbingSkinOffset;
    public float leggeClimbingDuration = 1f;

    [Header("Rail Grinding Stats")]
    public bool useCustomCollision = true;

    public float grindRadiusOffset = 0.26f;
    public float minGrindInitialSpeed = 10f;
    public float minGrindSpeed = 5f;
    public float maxGrindSpeed = 25f;

    [Header("Rail Grinding Brake")]
    public bool canGrindBrake = true;

    public float grindBrakeDeceleration = 10f;

    [Header("Rail Grinding Dash Stats")]
    public bool canGrindDash = true;

    public bool applyGrindingSlopeFactor = true;

    public float grindDashCoolDown = 0.5f;
    public float grindDashForce = 25f;

    [Header("Swimming Stats")]
    public float waterConversion = 0.35f;

    public float waterRotationSpeed = 360f;
    public float waterUpwardForce = 8f;
    public float waterJumpForce = 15;
    public float waterTurningDrag = 2.5f;
    public float swimAcceleration = 4f;
    public float swimDeceleration = 3f;
    public float swimTopSpeed = 4f;
    public float swimDiveForce = 15f;

    [Header("Portal Stats")]
    public float teleportalCooldown = 1f;

    [Header("Crouch Stats")]
    public float crouchHeight = 1f;

    public float crouchFriction = 10f;

    [Header("Crawling Stats")]
    public float crawlingAcceleration = 8f;

    public float crawlingFriction = 32f;
    public float crawlingTopSpeed = 2.5f;
    public float crawlingTurningSpeed = 3f;

    [Header("Air Dive Stats")]
    public bool canAireDive = true;

    public bool applyDiveSlopeFactor = true;
    public float airDiveForwardForce = 16f;
    public float airDiveFriction = 32f;
    public float airDiveSlopeFriction = 12f;
    public float airDiveGroundLeapHeight = 10f;
    public float airDiveRotationSpeed = 45f;

    [Header("Gliding Stats")]
    public bool canGlide = true;

    public float glidingGravity = 10f;
    public float glidingMaxFallSpeed = 2f;
    public float glidingTurningDrag = 8f;

    [Header("Dash Stats")]
    public bool canAirDash = true;

    public bool canGroundDash = true;
    public float dashForce = 25f;
    public float dashDuration = 0.3f;
    public float groundDashCoolDown = 0.5f;
    public int allowedAirDashes = 1;
}