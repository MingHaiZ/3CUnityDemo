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

    public Vector3 velocity { get; set; }

    public Vector3 lateralVelocity
    {
        get { return new Vector3(velocity.x, 0, velocity.z); }
        set { velocity = new Vector3(value.x, velocity.y, value.z); }
    }


    private void Awake()
    {
        InitializeStateManager();
    }

    protected void Update()
    {
        HandleState();
    }
}