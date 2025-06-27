using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider), typeof(AudioSource))]
public class Breakable : MonoBehaviour
{
    public GameObject display;
    public AudioClip clip;
    
    protected Collider m_collider;
    protected AudioSource m_audioSource;
    protected Rigidbody m_rigidbody;
    
    public UnityEvent onBreak;

    public bool broken { get; protected set; }

    protected virtual void Break()
    {
        if (!broken)
        {
            if (m_rigidbody)
            {
                m_rigidbody.isKinematic = true;
            }

            broken = true;
            display.SetActive(false);
            m_collider.enabled = false;
            m_audioSource.PlayOneShot(clip);
            onBreak?.Invoke();
        }
    }

    protected virtual void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_collider = GetComponent<Collider>();
        TryGetComponent(out m_rigidbody);
    }
}