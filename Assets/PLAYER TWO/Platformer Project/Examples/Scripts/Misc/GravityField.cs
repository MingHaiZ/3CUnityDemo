using System;
using UnityEngine;

public class GravityField : MonoBehaviour
{
    public float force = 75f;
    protected Collider m_coller;

    protected void Start()
    {
        m_coller = GetComponent<Collider>();
        m_coller.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTag.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                if (player.isGrounded)
                {
                    player.verticalVelocity = Vector3.zero;
                }

                player.velocity += transform.up * force * Time.deltaTime;
            }
        }
    }
}