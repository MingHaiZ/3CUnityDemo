using System;
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
    public Collider water { get; protected set; }
    public int airSpinCount { get; protected set; }
    public Pickable pickable { get; protected set; }

    public Transform pickableSlot;
    public Transform skin;

    public Vector3 m_skinInitialPosition;
    public Quaternion m_skinInitialRotation;

    protected Vector3 m_respawnPosition;
    protected Quaternion m_respawnRotation;

    protected const float k_waterExitOffset = 0.25f;
    protected readonly float m_slopingGroundAngle = 20f;

    public Vector3 lastWallNormal { get; protected set; }
    public int airDashCounter { get; protected set; }
    public float lastDashTime { get; protected set; }

    public virtual bool canStandUp => !SphereCast(Vector3.up, originalHeight);

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
    public virtual void ResetJumps() => jumpCounter = 0;
    public virtual void ResetAirDashCounter() => airDashCounter = 0;
    public virtual void InitializeTag() => tag = GameTag.Player;
    public virtual bool IsAlive() => !health.isEmpty;
    public virtual void StartGrind() => states.Change<RailGrindPlayerState>();

    protected override void Awake()
    {
        base.Awake();
        InitializeInputs();
        InitializeStats();
        InitializeHealth();
        InitializeTag();
        InitializeRespawn();
        InitializeSkin();

        entityEvents.OnGroundEnter.AddListener(() =>
        {
            ResetJumps();
            ResetAirSpinCount();
            ResetAirDashCounter();
        });

        entityEvents.OnRailsEnter.AddListener(() =>
        {
            ResetJumps();
            ResetAirSpinCount();
            StartGrind();
        });
    }

    protected void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTag.VolumeWater))
        {
            if (!onWater && other.bounds.Contains(unsizePosition))
            {
                EnterWater(other);
            } else if (onWater)
            {
                var exitPoint = position + Vector3.down * k_waterExitOffset;

                if (!other.bounds.Contains(exitPoint))
                {
                    ExitWater();
                }
            }
        }
    }

    /**
     * 计算速度
     */
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

        if (!states.IsCurrentOfType(typeof(DashPlayerState)))
        {
            if (!inputs.GetRun())
            {
                lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed);
            }
        }
    }

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

    public virtual void SetJumps(int amount) => jumpCounter = amount;

    public virtual void Jump()
    {
        // 这里提到了土狼时间,之前玩APEX的了解过,有点意思
        var canMultiJump = (jumpCounter > 0) && (jumpCounter < stats.current.multiJumps);
        var canCoyoteJump = (jumpCounter == 0) && (Time.time < lastGroundTime + stats.current.coyoteJumpThreshold);
        var holdJump = !holding;

        if ((isGrounded || onRails || canMultiJump || canCoyoteJump) && holdJump)
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

    public void Jump(float height)
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
            pickable.Release(transform.forward, force);

            pickable = null;
            holding = false;

            playerEvents.OnThrow?.Invoke();
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

    public virtual void PickAndThrow()
    {
        if (stats.current.canPickUp && inputs.GetPickAndDropDown())
        {
            if (!holding)
            {
                if (CapsuleCast(transform.forward, stats.current.pickUpDistance, out var hit))
                {
                    if (hit.transform.TryGetComponent(out Pickable pickable))
                    {
                        PickUp(pickable);
                    }
                }
            } else
            {
                Throw();
            }
        }
    }

    public virtual void PickUp(Pickable pickable)
    {
        if (!holding && (isGrounded || stats.current.canPickUpOnAir))
        {
            holding = true;
            this.pickable = pickable;
            pickable.PickUp(pickableSlot);
            pickable.onRespawn.AddListener(RemovePickable);
            playerEvents.OnPickUp?.Invoke();
        }
    }

    public virtual void RemovePickable()
    {
        if (holding)
        {
            pickable = null;
            holding = false;
        }
    }

    protected override bool EvaluateLanding(RaycastHit hit)
    {
        return base.EvaluateLanding(hit) && !hit.collider.CompareTag(GameTag.Spring);
    }

    public void StompAttack()
    {
        if (inputs.GetStompDown())
        {
            states.Change<StompPlayerState>();
        }
    }

    public virtual void SetRespawn(Vector3 position, Quaternion rotation)
    {
        m_respawnPosition = position;
        m_respawnRotation = rotation;
    }

    public virtual void LedgeGrab()
    {
        if (stats.current.canLedgeHang && velocity.y < 0 && !holding &&
            states.ContainsStateOfType(typeof(LedgeHangingPlayerState)) && DetectingLedge(
                stats.current.ledgeMaxForwardDistance, stats.current.ledgeMaxDownwardDistance, out var hit))
        {
            if (!(hit.collider is CapsuleCollider) && !(hit.collider is SphereCollider))
            {
                var ledgeDistance = radius + stats.current.ledgeMaxForwardDistance;
                var lateralOffset = transform.forward * ledgeDistance;
                var verticalOffset = Vector3.down * height * 0.5f - center;

                velocity = Vector3.zero;

                transform.parent = hit.collider.CompareTag(GameTag.Platform) ? hit.transform.parent : null;
                transform.position = hit.point - lateralOffset + verticalOffset;

                states.Change<LedgeHangingPlayerState>();

                playerEvents.OnLedgeGrabbed?.Invoke();
            }
        }
    }

    protected virtual bool DetectingLedge(float forwardDistance, float downwardDistance, out RaycastHit ledgeHit)
    {
        var contactOffset = Physics.defaultContactOffset + positionDelta;
        var ledgeMaxDistance = radius + forwardDistance;
        var ledgeHeightOffset = height * 0.5f + contactOffset;
        var upwardOffset = transform.up * ledgeHeightOffset;
        var forwardOffset = transform.forward * ledgeMaxDistance;

        if (Physics.Raycast(position + upwardOffset, transform.forward, ledgeMaxDistance, Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore)
            || Physics.Raycast(position + forwardOffset * 0.01f, transform.up, ledgeHeightOffset,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
        {
            ledgeHit = new RaycastHit();
            return false;
        }

        var origin = position + upwardOffset + forwardOffset;
        var distance = downwardDistance + contactOffset;

        return Physics.Raycast(origin, Vector3.down, out ledgeHit, distance, stats.current.ledgeHangingLayers,
            QueryTriggerInteraction.Ignore);
    }

    public virtual void SetSkinParent(Transform parent)
    {
        if (skin)
        {
            skin.parent = parent;
        }
    }

    public virtual void InitializeSkin()
    {
        if (skin)
        {
            m_skinInitialPosition = skin.localPosition;
            m_skinInitialRotation = skin.localRotation;
        }
    }

    public virtual void ResetSkinParent()
    {
        if (skin)
        {
            skin.parent = transform;
            skin.localPosition = Vector3.zero;
            skin.localRotation = Quaternion.identity;
        }
    }

    public virtual void WaterAcceleration(Vector3 direction)
    {
        Accelerate(direction, stats.current.waterTurningDrag, stats.current.swimAcceleration,
            stats.current.swimTopSpeed);
    }

    public virtual void WaterFaceDirection(Vector3 direction)
    {
        FaceDirection(direction, stats.current.waterRotationSpeed);
    }

    public virtual void EnterWater(Collider water)
    {
        if (!onWater && !health.isEmpty)
        {
            Throw();
            onWater = true;
            this.water = water;
            states.Change<SwimPlayerState>();
        }
    }

    public virtual void ExitWater()
    {
        if (onWater)
        {
            onWater = false;
        }
    }

    public virtual void Backflip(float force)
    {
        if (stats.current.canBackflip && !holding)
        {
            verticalVelocity = Vector3.up * stats.current.backflipJumpHeight;
            lateralVelocity = -transform.forward * force;
            states.Change<BackflipPlayerState>();
            playerEvents.OnBackflip?.Invoke();
        }
    }

    public virtual void ResizeCollider(float height)
    {
        var delta = height - this.height;
        controller.height = height;
        controller.center += Vector3.up * delta * 0.5f;
    }

    public virtual void Crouch()
    {
        if (inputs.GetCrouchAndCrawl())
        {
            states.Change<CrouchPlayerState>();
        }
    }

    public virtual void CrawlingAccelerate(Vector3 direction)
    {
        Accelerate(direction, stats.current.crawlingTurningSpeed, stats.current.crawlingAcceleration,
            stats.current.crawlingTopSpeed);
    }

    public virtual void AirDive()
    {
        if (inputs.GetAirDiveDown() && stats.current.canAireDive && !holding && !isGrounded)
        {
            states.Change<AirDivePlayerState>();
            playerEvents.OnAirDive?.Invoke();
        }
    }

    public virtual void SlopeFactor(float upwardForce, float downWardForce)
    {
        if (!isGrounded || !OnSlopingGround())
        {
            return;
        }

        var factor = Vector3.Dot(Vector3.up, groundNormal);
        var downwards = Vector3.Dot(localSlopeDirection, lateralVelocity) > 0;
        var multiplier = downwards ? downWardForce : upwardForce;
        var delta = factor * multiplier * Time.deltaTime;
        lateralVelocity += localSlopeDirection * delta;
    }

    public virtual bool OnSlopingGround()
    {
        if (isGrounded && groundAngle > m_slopingGroundAngle)
        {
            if (Physics.Raycast(transform.position, -transform.up, out var hit, height * 2f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                return Vector3.Angle(hit.normal, Vector3.up) > m_slopingGroundAngle;
            } else
            {
                return true;
            }
        }

        return false;
    }

    public virtual void Glide()
    {
        if (inputs.GetGlide() && !isGrounded && !holding && verticalVelocity.y <= 0 && stats.current.canGlide)
        {
            states.Change<GlidingPlayerState>();
        }
    }

    public virtual void Dash()
    {
        var canAirDash = stats.current.canAirDash && !isGrounded && airDashCounter < stats.current.allowedAirDashes;
        var canGroundDash = stats.current.canGroundDash && isGrounded &&
                            Time.time > lastDashTime + stats.current.groundDashCoolDown;
        if (inputs.GetDashDown() && (canAirDash || canGroundDash))
        {
            if (!isGrounded)
            {
                airDashCounter++;
            }

            lastDashTime = Time.time;
            states.Change<DashPlayerState>();
        }
    }

    public virtual void RegularSlopeFactor()
    {
        if (stats.current.applySlopeFactor)
        {
            SlopeFactor(stats.current.slopeUpwardForce, stats.current.slopeDownwardForce);
        }
    }

    public virtual void DirectionalJump(Vector3 direction, float height, float distance)
    {
        jumpCounter++;
        verticalVelocity = Vector3.up * height;
        lateralVelocity = direction * distance;
        playerEvents.OnJump.Invoke();
    }

    public virtual void WallDrag(Collider other)
    {
        if (stats.current.canWallDrag && velocity.y <= 0 && !holding && !other.TryGetComponent(out Rigidbody _))
        {
            if (CapsuleCast(transform.forward, 0.25f, out var hit, stats.current.wallDragLayers) &&
                !DetectingLedge(0.25f, height, out _))
            {
                if (hit.collider.CompareTag(GameTag.Platform))
                {
                    transform.parent = hit.transform;
                }

                lastWallNormal = hit.normal;
                states.Change<WallDragPlayerState>();
            }
        }
    }
}