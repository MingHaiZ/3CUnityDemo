using System;
using UnityEngine;
using UnityEngine.Events;

public class Volume : MonoBehaviour
{
    protected Collider m_collider;
    protected AudioSource m_audio;

    public UnityEvent onEnter;
    public UnityEvent onExit;

    public AudioClip enterClip;
    public AudioClip exitClip;

    protected virtual void Start()
    {
        InitializeCollider();
        InitializeAudioSource();
    }

    private void InitializeAudioSource()
    {
        if (!TryGetComponent(out m_audio))
        {
            m_audio = gameObject.AddComponent<AudioSource>();
        }

        m_audio.spatialBlend = 0.5f;
    }

    private void InitializeCollider()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_collider.bounds.Contains(other.bounds.max) && !m_collider.bounds.Contains(other.bounds.min))
        {
            m_audio.PlayOneShot(enterClip);
            onEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!m_collider.bounds.Contains(other.transform.position))
        {
            m_audio.PlayOneShot(exitClip);
            onExit?.Invoke();
        }
    }
}