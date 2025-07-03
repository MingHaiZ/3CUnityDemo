using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool useFlash = true;
    public Portal exit;
    public float exitOffset = 1f;
    public AudioClip teleportClip;

    protected Collider m_collider;
    protected AudioSource m_audio;
    protected PlayerCamera m_camera;

    public Vector3 position => transform.position;
    public Vector3 forward => transform.forward;

    protected void Start()
    {
        m_collider = GetComponent<Collider>();
        m_audio = GetComponent<AudioSource>();
        m_camera = FindObjectOfType<PlayerCamera>();
        m_collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (exit && other.TryGetComponent(out Player player) &&
            Time.time > player.lastTeleportalTime + player.stats.current.teleportalCooldown)
        {
            var yOffset = player.unsizePosition.y - transform.position.y;
            player.transform.position = exit.position + Vector3.up * yOffset;
            player.FaceDirectionSmooth(exit.forward);
            m_camera.Reset();
            player.lastTeleportalTime = Time.time;

            var inputDirection = player.inputs.GetMovementCameraDirection();

            if (Vector3.Dot(inputDirection, exit.forward) < 0)
            {
                player.FaceDirectionSmooth(-exit.forward);
            }

            player.transform.position += player.transform.forward * exit.exitOffset;
            player.lateralVelocity = player.transform.forward * player.lateralVelocity.magnitude;

            if (useFlash)
            {
                Flash.instance?.Trigger();
            }

            m_audio.PlayOneShot(teleportClip);
        }
    }
}