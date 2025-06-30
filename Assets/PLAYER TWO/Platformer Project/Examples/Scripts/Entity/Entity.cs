using System;
using UnityEditor.VersionControl;
using UnityEngine;


public abstract class Entity : MonoBehaviour
{
    public EntityEvents entityEvents;
    public bool isGrounded { get; set; } = true;
    public Vector3 velocity { get; set; }
    public float turningDragMultiplier { get; set; } = 1;
    public float topSpeedMultiplier { get; set; } = 1;

    public float acclerationMultiplier { get; set; } = 1;
    public float decelerationMultiplie { get; set; } = 1;
    public float gravityMultiplier { get; set; } = 1;

    public CharacterController controller { get; protected set; }

    public Vector3 lateralVelocity
    {
        get { return new Vector3(velocity.x, 0, velocity.z); }
        set { velocity = new Vector3(value.x, velocity.y, value.z); }
    }

    public Vector3 verticalVelocity
    {
        get { return new Vector3(0, velocity.y, 0); }
        set { velocity = new Vector3(velocity.x, value.y, velocity.z); }
    }

    public float height => controller.height;
    protected readonly float m_groundOffset = 0.1f;
    public float radius => controller.radius;
    public Vector3 center => controller.center;
    public Vector3 position => transform.position + center;
    public float lastGroundTime { get; protected set; }
    public Vector3 stepPosition => position - transform.up * (height * 0.5f - controller.stepOffset);
    public RaycastHit groundHit;
    public float originalHeight { get; protected set; }
    public Vector3 unsizePosition => position - transform.up * height * 0.5f + transform.up * originalHeight * 0.5f;

    protected Collider[] m_contactBuffer = new Collider[10];
    protected CapsuleCollider m_collider;

    public virtual bool IsPointUnderStep(Vector3 point) => stepPosition.y > point.y;

    public abstract void ApplyDamage(int amount, Vector3 origin);

    public float groundAngle { get; protected set; }
    public Vector3 groundNormal { get; protected set; }
    public Vector3 localSlopeDirection { get; protected set; }
}

public abstract class Entity<T> : Entity where T : Entity<T>
{
    public EntityStateManager<T> states { get; protected set; }

    protected virtual void HandleState() => states.Step();

    protected virtual void InitializeStateManager() => states = GetComponent<EntityStateManager<T>>();

    public virtual bool SphereCast(Vector3 direction, float distance, out RaycastHit hit,
        int layerMask = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        var castDistance = Mathf.Abs(distance - radius);
        return Physics.SphereCast(position, radius, direction, out hit, castDistance, layerMask,
            queryTriggerInteraction);
    }

    protected virtual void InitializeController()
    {
        controller = GetComponent<CharacterController>();
        if (!controller)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }

        controller.skinWidth = 0.005f;
        controller.minMoveDistance = 0;
        originalHeight = controller.height;
    }

    protected virtual void Awake()
    {
        InitializeController();
        InitializeStateManager();
        InitializeCollider();
    }

    protected void FixedUpdate()
    {
        if (controller.enabled || m_collider != null)
        {
            HandleGround();
            HandleContacts();
        }
    }

    protected virtual void Update()
    {
        if (controller.enabled)
        {
            HandleState();
            HandleController();
            OnUpdate();
        }
    }

    protected virtual void OnUpdate()
    {
    }

    protected virtual void InitializeCollider()
    {
        m_collider = gameObject.AddComponent<CapsuleCollider>();
        m_collider.height = controller.height;
        m_collider.radius = controller.radius;
        m_collider.center = controller.center;
        m_collider.isTrigger = true;
        m_collider.enabled = false;
    }

    protected virtual void HandleController()
    {
        if (controller.enabled)
        {
            controller.Move(velocity * Time.deltaTime);
            return;
        }

        transform.position = velocity * Time.deltaTime;
    }

    public virtual void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed)
    {
        if (direction.sqrMagnitude > 0)
        {
            // 代表“我当前的速度（lateralVelocity），有多‘顺着’我想去的新方向（direction）”。
            // 如果数字很大很正：非常顺路。
            // 如果数字是0：正好垂直，不顺路也不逆路。
            // 如果数字是负数：完全是反方向，在“开倒车”。
            var speed = Vector3.Dot(direction, lateralVelocity);
            // direction 是一个只有方向没有大小的“路标”，speed 是一个只有大小没有方向的“油门大小”。两者一乘，就得到了一个既有正确方向，又有合适大小的速度向量。这就是我们分解出的“好速度”。
            var velocity = direction * speed;

            var turningVelocity = lateralVelocity - velocity;
            var turiningDelta = turningDrag * turningDragMultiplier * Time.deltaTime;

            var targetTopSpeed = topSpeed * topSpeedMultiplier;
            if (lateralVelocity.magnitude < targetTopSpeed || speed < 0)
            {
                speed += acceleration * acclerationMultiplier * Time.deltaTime;
                speed = Mathf.Clamp(speed, -targetTopSpeed, targetTopSpeed);
            }

            velocity = direction * speed;
            turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turiningDelta);
            lateralVelocity = velocity + turningVelocity;
        }
    }

    public virtual void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0)
        {
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = rotation;
        }
    }

    public virtual void FaceDirection(Vector3 direction, float degressPerSpeed)
    {
        if (direction != Vector3.zero)
        {
            var rotation = transform.rotation;
            var rotationDelta = degressPerSpeed * Time.deltaTime;
            var target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(rotation, target, rotationDelta);
        }
    }

    public virtual void Decelerate(float deceleration)
    {
        var delta = deceleration * decelerationMultiplie * Time.deltaTime;
        lateralVelocity = Vector3.MoveTowards(lateralVelocity, Vector3.zero, delta);
    }

    public virtual void HandleGround()
    {
        var distance = (height * 0.5f) + m_groundOffset;
        if (SphereCast(Vector3.down, distance, out var hit) && verticalVelocity.y <= 0)
        {
            if (!isGrounded)
            {
                if (EvaluateLanding(hit))
                {
                    EnterGround(hit);
                } else
                {
                    HandleHighLedge(hit);
                }
            } else if (IsPointUnderStep(hit.point))
            {
                UpdateGround(hit);
                if (Vector3.Angle(hit.normal, Vector3.up) >= controller.slopeLimit)
                {
                }
            } else
            {
                HandleHighLedge(hit);
            }
        } else
        {
            ExitGround();
        }
    }

    private void UpdateGround(RaycastHit hit)
    {
        if (isGrounded)
        {
            groundHit = hit;
            groundNormal = groundHit.normal;
            groundAngle = Vector3.Angle(Vector3.up, groundHit.normal);
            localSlopeDirection = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
            transform.parent = hit.collider.CompareTag(GameTag.Platform) ? hit.transform : null;
        }
    }

    protected virtual void HandleHighLedge(RaycastHit hit)
    {
    }

    protected virtual void EnterGround(RaycastHit hit)
    {
        if (!isGrounded)
        {
            groundHit = hit;
            isGrounded = true;
            entityEvents.OnGroundEnter?.Invoke();
        }
    }

    protected virtual void ExitGround()
    {
        if (isGrounded)
        {
            isGrounded = false;
            transform.parent = null;
            lastGroundTime = Time.time;
            verticalVelocity = Vector3.Max(verticalVelocity, Vector3.zero);
            entityEvents.OnGroundExit?.Invoke();
        }
    }

    // 处理碰撞
    protected virtual void HandleContacts()
    {
        int overlaps = OverlapEntity(m_contactBuffer);

        for (int i = 0; i < overlaps; i++)
        {
            if (!m_contactBuffer[i].isTrigger && m_contactBuffer[i].transform != transform)
            {
                OnContact(m_contactBuffer[i]);

                var listeners = m_contactBuffer[i].GetComponents<IEntityContact>();
                foreach (var contact in listeners)
                {
                    contact.OnEntityContact((T)this);
                }

                if (m_contactBuffer[i].bounds.min.y > controller.bounds.max.y)
                {
                    verticalVelocity = Vector3.Min(verticalVelocity, Vector3.zero);
                }
            }
        }
    }

    /**
     * 获取检测范围内的gameobject个数
     */
    public virtual int OverlapEntity(Collider[] result, float skinOffset = 0)
    {
        var contactOffset = skinOffset + controller.skinWidth + Physics.defaultContactOffset;
        var overlapsRadius = radius + contactOffset;
        var offset = (height + contactOffset) * 0.5f - overlapsRadius;
        var top = position + Vector3.up * offset;
        var bottom = position + Vector3.down * offset;
        return Physics.OverlapCapsuleNonAlloc(top, bottom, overlapsRadius, result);
    }

    protected virtual void OnContact(Collider other)
    {
        if (other)
        {
            states.OnContact(other);
        }
    }

    protected virtual bool EvaluateLanding(RaycastHit hit)
    {
        return IsPointUnderStep(hit.point) && Vector3.Angle(hit.normal, Vector3.up) < controller.slopeLimit;
    }

    public virtual void SnapToGround(float force)
    {
        if (isGrounded && (verticalVelocity.y <= 0))
        {
            verticalVelocity = Vector3.down * force;
        }
    }

    public virtual bool CapsuleCast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        return CapsuleCast(direction, distance, out _, layer, queryTriggerInteraction);
    }

    public virtual bool CapsuleCast(Vector3 direction, float distance, out RaycastHit hit,
        int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        var origin = position - direction * radius + center;
        var offset = transform.up * (height * 0.5f - radius);
        var top = origin + offset;
        var bottom = origin - offset;
        return Physics.CapsuleCast(top, bottom, radius, direction, out hit, distance + radius, layer,
            queryTriggerInteraction);
    }

    public virtual void Gravity(float gravity)
    {
        if (!isGrounded)
        {
            verticalVelocity += Vector3.down * gravity * gravityMultiplier * Time.deltaTime;
        }
    }
}