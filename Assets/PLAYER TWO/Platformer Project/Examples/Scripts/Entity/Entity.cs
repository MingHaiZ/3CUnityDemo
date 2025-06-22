using System;
using UnityEditor.VersionControl;
using UnityEngine;


public abstract class Entity : MonoBehaviour
{
}

public abstract class Entity<T> : Entity where T : Entity<T>
{
    public EntityStateManager<T> states { get; protected set; }

    protected virtual void HandleState() => states.Step();

    protected virtual void InitializeStateManager() => states = GetComponent<EntityStateManager<T>>();
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


    protected virtual void InitializeController()
    {
        controller = GetComponent<CharacterController>();
        if (!controller)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }

        controller.skinWidth = 0.005f;
        controller.minMoveDistance = 0;
    }

    protected virtual void Awake()
    {
        InitializeController();
        InitializeStateManager();
    }

    protected void Update()
    {
        HandleState();
        HandleController();
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

    public virtual void Gravity(float gravity)
    {
        if (!isGrounded)
        {
            verticalVelocity += Vector3.down * gravity * gravityMultiplier * Time.deltaTime;
        }
    }
}