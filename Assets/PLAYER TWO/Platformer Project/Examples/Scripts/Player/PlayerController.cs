using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    protected Player m_player;

    protected Player player
    {
        get
        {
            if (!m_player)
            {
                m_player = FindObjectOfType<Player>();
            }

            return m_player;
        }
    }

    public void AddHealth() => AddHealth(1);
    public virtual void AddHealth(int amount) => player.health.Increase(amount);
}