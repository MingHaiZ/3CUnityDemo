using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity<Player>
{
    public PlayerEvents playerEvents;

    public PlayerInputManager inputs { get; protected set; }
    public PlayerStatsManager stats { get; protected set; }
    public int jumpCounter { get; protected set; }
    public bool holding { get; protected set; }
    public Health health { get; protected set; }
    public bool onWater { get; protected set; }
    public int airSpinCount { get; protected set; }

    protected Vector3 m_respawnPosition;
    protected Quaternion m_respawnRotation;

    protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
    protected virtual void InitializeStats() => stats = GetComponent<PlayerStatsManager>();
    protected virtual void InitializeHealth() => health = GetComponent<Health>();

    protected virtual void InitializeRespawn()
    {
        m_respawnPosition = transform.position;
        m_respawnRotation = transform.rotation;
    }

    public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, stats.current.rotationSpeed);
    public virtual void Decelerate() => Decelerate(stats.current.deceleration);
    public virtual void Friction() => Decelerate(stats.current.friction);
    public virtual void InitializeTag() => tag = GameTag.Player;

    protected override void Awake()
    {
        base.Awake();
        InitializeInputs();
        InitializeStats();
        InitializeHealth();
        InitializeTag();
        InitializeRespawn();
    }

    public virtual void Accelerate(Vector3 direction)
    {
        var turningDrag = isGrounded && inputs.GetRun()
            ? stats.current.runningTurnningDrag
            : stats.current.turningDrag;
        var acceleration = isGrounded && inputs.GetRun()
            ? stats.current.runningAcceleration
            : stats.current.acceleration;
        var topSpeed = inputs.GetRun()
            ? stats.current.runningTopSpeed
            : stats.current.topSpeed;
        var finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;

        Accelerate(direction, turningDrag, finalAcceleration, topSpeed);
    }

    // public virtual void Backflip(float force)
    // {
    //     if (stats.current.canBackflip)
    //     {
    //         verticalVelocity = Vector3.up * stats.current.backflipJumpHeight;
    //         lateralVelocity = -transform.forward * force;
    //         states.Change<BackflipPlayerState>();
    //         playerEvents.OnBackflip.Invoke();
    //     }
    // }


    public virtual void BackflipAcceleration()
    {
        var direction = inputs.GetMovementCameraDirection();
        Accelerate(direction,
            stats.current.backflipTurningDrag,
            stats.current.backflipAirAcceleration,
            stats.current.backflipTopSpeed);
    }

    public virtual void Gravity()
    {
        if (!isGrounded && verticalVelocity.y > -stats.current.gravityTopSpeed)
        {
            var speed = verticalVelocity.y;
            var force = verticalVelocity.y > 0 ? stats.current.gravity : stats.current.fallGravity;
            speed -= force * gravityMultiplier * Time.deltaTime;
            speed = Mathf.Max(speed, -stats.current.gravityTopSpeed);
            verticalVelocity = new Vector3(0, speed, 0);
        }
    }

    public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);

    public virtual void Fall()
    {
        if (!isGrounded)
        {
            states.Change<FallPlayerState>();
        }
    }

    public virtual void Jump()
    {
        // 这里提到了土狼时间,之前玩APEX的了解过,有点意思
        var canMultiJump = (jumpCounter > 0) && (jumpCounter < stats.current.multiJumps);
        var canCoyoteJump = (jumpCounter == 0) && (Time.time < lastGroundTime + stats.current.coyoteJumpThreshold);

        if (isGrounded || canMultiJump || canCoyoteJump)
        {
            if (inputs.GetJumpDown())
            {
                Jump(stats.current.maxJumpHeight);
            }

            if (inputs.GetJumpUp() && (jumpCounter > 0) && verticalVelocity.y > stats.current.minJumpHeight)
            {
                verticalVelocity = Vector3.up * stats.current.minJumpHeight;
            }
        }
    }

    private void Jump(float height)
    {
        jumpCounter++;
        verticalVelocity = Vector3.up * height;
        states.Change<FallPlayerState>();
        playerEvents.OnJump?.Invoke();
    }

    public virtual void PushRigidbody(Collider other)
    {
        if (!IsPointUnderStep(other.bounds.max) && other.TryGetComponent(out Rigidbody rigidbody))
        {
            var force = lateralVelocity * stats.current.pushForce;
            rigidbody.velocity += force / rigidbody.mass * Time.deltaTime;
        }
    }

    public virtual void Throw()
    {
        if (holding)
        {
            var force = lateralVelocity.magnitude * stats.current.throwVelocityMultiplier;
            // TODO
        }
    }

    public virtual void Respawn()
    {
        health.Reset();
        transform.SetPositionAndRotation(m_respawnPosition, m_respawnRotation);
        states.Change<IdlePlayerState>();
    }

    public virtual void AccelerateToInputDirection()
    {
        var inputDirection = inputs.GetMovementCameraDirection();
        Accelerate(inputDirection);
    }

    public override void ApplyDamage(int amount, Vector3 origin)
    {
        if (!health.isEmpty && !health.recovering)
        {
            health.Damage(amount);
            var damageDir = origin - transform.position;
            damageDir.y = 0;
            damageDir = damageDir.normalized;
            FaceDirection(damageDir);
            lateralVelocity = -transform.forward * stats.current.hurtBackwardsForce;

            if (!onWater)
            {
                verticalVelocity = Vector3.up * stats.current.hurtUpwardsForce;
                states.Change<HurtPlayerState>();
            }

            playerEvents.OnHurt?.Invoke();

            if (health.isEmpty)
            {
                Throw();
                playerEvents.OnDie?.Invoke();
            }
        }
    }

    public virtual void Spin()
    {
        var canAirSpin = (isGrounded || stats.current.canAirSpin) && airSpinCount < stats.current.canAirSpinCount;
        if (stats.current.canSpin && canAirSpin && !holding && inputs.GetSpinDown())
        {
            if (!isGrounded)
            {
                airSpinCount++;
            }

            states.Change<SpinPlayerState>();
            playerEvents.OnSpin?.Invoke();
        }
    }


    public virtual void ResetAirSpinCount() => airSpinCount = 0;
}