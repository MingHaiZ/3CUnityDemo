using System;
using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour, IEntityContact
{
    public float fallDelay = 2f;
    public bool autoReset = true;
    public float resetDelay = 5f;
    public float fallGravity = 40f;
    public bool falling { get; protected set; }

    [Header("Shake Settings")]
    public bool canShake = true;

    public float speed = 45;
    public float height = 0.1f;

    protected Collider m_collider;
    protected Vector3 m_initialPosition;
    protected Collider[] m_colliders = new Collider[32];
    public bool activated { get; protected set; }

    public void Start()
    {
        tag = GameTag.Spring;
        m_collider = GetComponent<Collider>();
        m_initialPosition = transform.position;
    }

    public void OnEntityContact(Entity entity)
    {
        if (entity.IsPointUnderStep(m_collider.bounds.max) && entity is Player player && player.IsAlive())
        {
            if (!activated)
            {
                activated = true;
                StartCoroutine(Routine());
            }
        }
    }

    protected IEnumerator Routine()
    {
        var timer = fallDelay;
        while (timer >= 0)
        {
            if (canShake && (timer <= fallDelay / 2f))
            {
                var shake = Mathf.Sin(Time.time * speed) * height;
                transform.position = m_initialPosition + Vector3.up * shake;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        Fall();
        if (autoReset)
        {
            yield return new WaitForSeconds(resetDelay);
            ReStart();
        }
    }

    
    public void Fall()
    {
        falling = true;
        m_collider.isTrigger = true;
    }

    public virtual void ReStart()
    {
        activated = falling = false;
        transform.position = m_initialPosition;
        m_collider.isTrigger = false;
        OffsetPlayer();
    }

    protected virtual void OffsetPlayer()
    {
        var center = m_collider.bounds.center;
        var extents = m_collider.bounds.extents;
        var maxY = m_collider.bounds.max.y;
        var overlaps = Physics.OverlapBoxNonAlloc(center, extents, m_colliders);

        for (int i = 0; i < overlaps; i++)
        {
            if (!m_colliders[i].CompareTag(GameTag.Player))
            {
                continue;
            }

            var distance = maxY - m_colliders[i].transform.position.y;
            var height = m_colliders[i].GetComponent<Player>().height;
            var offset = Vector3.up * (distance + height * 0.5f);

            m_colliders[i].transform.position += offset;
        }
    }

    private void Update()
    {
        if (falling)
        {
            transform.position += Vector3.down * fallGravity * Time.deltaTime;
        }
    }
}