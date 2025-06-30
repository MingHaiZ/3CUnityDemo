using System;
using UnityEngine;
using UnityEngine.Events;

public class CheckPoint : MonoBehaviour
{
    public AudioClip clip;
    public UnityEvent OnActive;
    public Transform respawn;
    protected Collider m_collider;
    protected AudioSource m_audio;
    public bool activated { get; protected set; }

    public virtual void Activate(Player player)
    {
        if (!activated)
        {
            activated = true;
            m_audio.PlayOneShot(clip);
            player.SetRespawn(respawn.position, respawn.rotation);
            OnActive?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activated && other.CompareTag(GameTag.Player))
        {
            if (other.TryGetComponent(out Player player))
            {
                Activate(player);
            }
        }
    }

    protected void Awake()
    {
        if (!TryGetComponent(out m_audio))
        {
            m_audio = gameObject.AddComponent<AudioSource>();
        }

        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }
}