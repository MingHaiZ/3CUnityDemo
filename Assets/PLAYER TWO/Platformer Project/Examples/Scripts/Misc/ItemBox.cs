using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class ItemBox : MonoBehaviour, IEntityContact
{
    protected BoxCollider m_collider;
    protected Vector3 m_initialScale;
    public Collectable[] collectables;
    public bool m_enabled = true;
    public Material emptyItemBoxMaterial;
    public MeshRenderer itemBoxRender;

    [Space(15)]
    public UnityEvent onCollect;

    public UnityEvent onDisable;

    public int m_index;

    protected virtual void InitializeCollectables()
    {
        foreach (var collectable in collectables)
        {
            if (!collectable.hidden)
            {
                collectable.gameObject.SetActive(false);
            } else
            {
                collectable.collectOnContact = false;
            }
        }
    }

    public virtual void Collect(Player player)
    {
        if (m_enabled)
        {
            if (m_index < collectables.Length)
            {
                if (collectables[m_index].hidden)
                {
                    collectables[m_index].Collect(player);
                } else
                {
                    collectables[m_index].gameObject.SetActive(true);
                }
            }

            m_index = Mathf.Clamp(m_index + 1, 0, collectables.Length);
            onCollect?.Invoke();
        }

        if (m_index == collectables.Length)
        {
            Disable();
        }
    }

    public virtual void Disable()
    {
        if (m_enabled)
        {
            m_enabled = false;
            itemBoxRender.sharedMaterial = emptyItemBoxMaterial;
            onDisable?.Invoke();
        }
    }

    protected void Start()
    {
        m_collider = GetComponent<BoxCollider>();
        m_initialScale = transform.localScale;
        InitializeCollectables();
    }

    public void OnEntityContact(Entity entity)
    {
        if (entity is Player player)
        {
            if (entity.velocity.y > 0 && player.position.y < m_collider.bounds.min.y)
            {
                Collect(player);
            }
        }
    }
}