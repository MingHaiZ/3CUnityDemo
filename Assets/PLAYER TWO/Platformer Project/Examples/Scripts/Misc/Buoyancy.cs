using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buoyancy : MonoBehaviour
{
    public float force;
    protected Rigidbody m_rigidbody;

    protected virtual void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameTag.VolumeWater))
        {
            if (transform.position.y < other.bounds.max.y)
            {
                var multiplier = Mathf.Clamp01(other.bounds.max.y - transform.position.y);
                var buoyancy = Vector3.up * force * multiplier;
                m_rigidbody.AddForce(buoyancy);
            }
        }
    }
}