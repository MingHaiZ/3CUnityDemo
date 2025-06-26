using System;
using UnityEngine;
using UnityEngine.Events;

public class Panel : MonoBehaviour, IEntityContact
{
    public bool autoToggle;

    protected Collider m_collider;
    protected Collider m_entityActivator;
    protected Collider m_otherActivator;

    protected AudioSource m_audioSource;
    public bool requirePlayer;
    public bool requireStomp;
    public AudioClip activateClip;
    public AudioClip deactivateClip;

    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    public bool activated { get; protected set; }

    private void Start()
    {
        gameObject.tag = GameTag.Panel;
        m_collider = GetComponent<Collider>();
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (m_entityActivator || m_otherActivator)
        {
            var center = m_collider.bounds.center;
            var contactOffset = Physics.defaultContactOffset + 0.1f;
            var size = m_collider.bounds.size + Vector3.up * contactOffset;
            var bounds = new Bounds(center, size);


            var intersectsEntity = m_entityActivator && bounds.Intersects(m_entityActivator.bounds);
            var intersectsOther = m_otherActivator && bounds.Intersects(m_otherActivator.bounds);
            if (intersectsEntity || intersectsOther)
            {
                Activate();
            } else
            {
                m_entityActivator = intersectsEntity ? m_entityActivator : null;
                m_otherActivator = intersectsEntity ? m_otherActivator : null;

                if (autoToggle)
                {
                    Deactivate();
                }
            }
        }
    }


    public virtual void Activate()
    {
        if (!activated)
        {
            if (activateClip)
            {
                m_audioSource.PlayOneShot(activateClip);
            }

            activated = true;
            OnActivate?.Invoke();
        }
    }

    public virtual void Deactivate()
    {
        if (activated)
        {
            if (deactivateClip)
            {
                m_audioSource.PlayOneShot(deactivateClip);
            }

            activated = false;
            OnDeactivate?.Invoke();
        }
    }

    public void OnEntityContact(Entity entity)
    {
        if (entity.velocity.y <= 0 && entity.IsPointUnderStep(m_collider.bounds.max))
        {
            if ((!requirePlayer || entity is Player)
                && (!requireStomp || (entity as Player).states.IsCurrentOfType(typeof(StompPlayerState))))
            {
                m_entityActivator = entity.controller;
            }
        }
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (!(requirePlayer || requireStomp) && !collision.collider.CompareTag(GameTag.Player))
        {
            m_otherActivator = collision.collider;
        }
    }
}