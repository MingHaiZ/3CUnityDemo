using UnityEngine;

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

    [Header("Running stats")]
    public float runningAcceleration = 16f;

    public float runningTopSpeed = 7.5f;
    public float runningTurnningDrag = 14f;

    [Header("Backflip Stats")]
    public bool canBackflip = true;

    public float backflipJumpHeight = 23f;
    public float backflipGravity = 35f;
    public float backflipTurningDrag = 2.5f;
    public float backflipAirAcceleration = 12f;
    public float backflipTopSpeed = 7.5f;
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
}