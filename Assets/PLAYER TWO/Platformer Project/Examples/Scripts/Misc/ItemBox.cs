using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class ItemBox : MonoBehaviour, IEntityContact
{
    // 当前Box的碰撞盒
    protected BoxCollider m_collider;

    protected Vector3 m_initialScale;

    // 可收集物品
    public Collectable[] collectables;

    // 是否可以被获取
    public bool m_enabled = true;

    // 空盒子材质
    public Material emptyItemBoxMaterial;

    // 盒子渲染器
    public MeshRenderer itemBoxRender;

    [Space(15)]
    public UnityEvent onCollect;

    public UnityEvent onDisable;

    public int m_index;

    // 初始化可收集物品
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

    // 收集方法
    public virtual void Collect(Player player)
    {
        // 如果能被收集
        if (m_enabled)
        {
            if (m_index < collectables.Length)
            {
                // 如果收集物隐藏了,说明可以被收集
                if (collectables[m_index].hidden)
                {
                    collectables[m_index].Collect(player);
                }
                // 否则激活
                else
                {
                    collectables[m_index].gameObject.SetActive(true);
                }
            }

            // 将物品的index限制在0到length区间
            m_index = Mathf.Clamp(m_index + 1, 0, collectables.Length);
            // 发送事件
            onCollect?.Invoke();
        }

        // 如果index到达可收集物品的上限则将当前box转换为disable状态(这里不是指Editor里的Disable)
        // 此处的更改只会更改box的material
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
            // 让当前箱子的渲染材质编程emptyItemBoxMaterial
            itemBoxRender.sharedMaterial = emptyItemBoxMaterial;
            // 如果有事件则发送事件
            onDisable?.Invoke();
        }
    }

    protected void Start()
    {
        // 初始化collider
        m_collider = GetComponent<BoxCollider>();
        // 记录初始化大小
        m_initialScale = transform.localScale;
        InitializeCollectables();
    }

    public void OnEntityContact(Entity entity)
    {
        if (entity is Player player)
        {
            // velocity.y 大于 0的时候代表当前玩家在往上跳
            if (entity.velocity.y > 0 && player.position.y+ (entity.height * 0.5f)  <= m_collider.bounds.min.y)
            {
                Collect(player);
            }
        }
    }
}