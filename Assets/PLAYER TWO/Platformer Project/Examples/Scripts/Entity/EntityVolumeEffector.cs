using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityVolumeEffector : MonoBehaviour
{
    public float velocityConversion = 1f;
    public float accelerationMultiplier = 1f;
    public float topSpeedMultiplier = 1f;
    public float decelerationMultiplie = 1f;
    public float turningDragMultiplier = 1f;
    public float gravityMultiplier = 1f;

    protected Collider m_collider;

    protected virtual void Start()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity entity))
        {
            entity.velocity *= velocityConversion;
            entity.acclerationMultiplier = accelerationMultiplier;
            entity.topSpeedMultiplier = topSpeedMultiplier;
            entity.decelerationMultiplie = decelerationMultiplie;
            entity.turningDragMultiplier = turningDragMultiplier;
            entity.gravityMultiplier = gravityMultiplier;
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Entity entity))
        {
            entity.acclerationMultiplier = 1f;
            entity.topSpeedMultiplier = 1f;
            entity.decelerationMultiplie = 1f;
            entity.turningDragMultiplier = 1f;
            entity.gravityMultiplier = 1f;
        }
    }
}