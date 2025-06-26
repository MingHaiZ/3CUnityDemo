using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Hazard : MonoBehaviour, IEntityContact
{
    protected Collider m_collider;
    public bool isSolid;
    public bool damageOnlyFromAbove;
    public int damage = 1;


    private void Awake()
    {
        tag = GameTag.Hazard;
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = !isSolid;
    }

    protected virtual void TryToApplyDamageTo(Player player)
    {
        if (damageOnlyFromAbove || player.velocity.y <= 0 && player.IsPointUnderStep(m_collider.bounds.max))
        {
            player.ApplyDamage(damage, transform.position);
        }
    }

    public void OnEntityContact(Entity entity)
    {
        if (entity is Player player)
        {
            TryToApplyDamageTo(player);
        }
    }

    protected void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTag.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                TryToApplyDamageTo(player);
            }
        }
    }
}