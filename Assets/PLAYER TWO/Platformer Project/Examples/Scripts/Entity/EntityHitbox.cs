using System;
using UnityEngine;

public class EntityHitbox : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 1;

    public bool breakObjects = true;

    [Header("Rebound Settings")]
    public bool rebound;

    public float reboundMinForce = 10f;
    public float reboundMaxForce = 25f;

    [Header("Push Back Settings")]
    public bool pushBack;

    public float pushBackMinMagnitute = 10f;
    public float pushBackMaxMagnitute = 25f;

    protected Entity m_entity;
    protected Collider m_collider;

    protected virtual void InitiallizeEntity()
    {
        if (!m_entity)
        {
            m_entity = GetComponent<Entity>();
        }
    }

    protected virtual void InitializeCollider()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }

    protected virtual void Start()
    {
        InitiallizeEntity();
    }

    protected virtual void HandleEntityAttack(Entity other)
    {
        other.ApplyDamage(damage, transform.position);
    }

    protected virtual void HandleRebound()
    {
        if (rebound)
        {
            var force = -m_entity.velocity.y;
            force = Mathf.Clamp(force, reboundMinForce, reboundMaxForce);
            m_entity.verticalVelocity = Vector3.up * force;
        }
    }

    protected virtual void HandlePushBack()
    {
        if (pushBack)
        {
            var force = -m_entity.lateralVelocity.magnitude;
            force = Mathf.Clamp(force, pushBackMinMagnitute, pushBackMaxMagnitute);
            m_entity.lateralVelocity = -transform.forward * force;
        }
    }

    protected virtual void HandleBreakableObject(Breakable breakable)
    {
        if (breakObjects)
        {
            breakable.Break();
        }
    }

    protected virtual void HandleCollision(Collider other)
    {
        if (other != m_entity.controller)
        {
            if (other.TryGetComponent(out Entity target))
            {
                HandleEntityAttack(target);
                HandleRebound();
                HandlePushBack();
            } else if (other.TryGetComponent(out Breakable breakable))
            {
                HandleBreakableObject(breakable);
            }
        }
    }

    protected virtual void HandleCustomCollision(Collider other)
    {
        
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }
}