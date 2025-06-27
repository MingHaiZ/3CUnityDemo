using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Pickable : MonoBehaviour, IEntityContact
{
    [Header("Attack Settings")]
    public bool attackEnemies = true;

    public int damage = 1;
    public float minDamageSpeed = 5f;

    [Header("Respawn Settings")]
    public bool autoRespawn;

    public bool respawnOnHitHazards;
    public float respawnHeightLimit = -100;

    protected Collider m_collider;

    protected Rigidbody m_rigidbody;
    protected Vector3 m_initialPosition;
    protected Quaternion m_initialRotation;
    protected Transform m_initialParent;

    public bool beingHold { get; protected set; }

    protected virtual void Start()
    {
        m_collider = GetComponent<Collider>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_initialPosition = transform.localPosition;
        m_initialRotation = transform.rotation;
        m_initialParent = transform.parent;
    }

    public void OnEntityContact(Entity entity)
    {
        if (attackEnemies && entity is Enemy && m_rigidbody.velocity.magnitude > minDamageSpeed)
        {
            entity.ApplyDamage(damage, transform.position);
        }
    }

    public virtual void Respawn()
    {
        m_rigidbody.velocity = Vector3.zero;
        transform.parent = m_initialParent;
        transform.SetPositionAndRotation(m_initialPosition, m_initialRotation);
        m_rigidbody.isKinematic = m_collider.isTrigger = beingHold = false;
    }

    protected virtual void EvaluateHazardRespawn(Collider other)
    {
        if (autoRespawn && respawnOnHitHazards && other.CompareTag(GameTag.Hazard))
        {
            Respawn();
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        EvaluateHazardRespawn(other);
    }

    protected void OnCollisionEnter(Collision other)
    {
        EvaluateHazardRespawn(other.collider);
    }
}