using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int initial = 3;
    public int max = 3;
    public UnityEvent OnChange;
    public float coolDown = 1f;
    protected int m_currentHealth;

    public int current
    {
        get { return m_currentHealth; }
        protected set
        {
            var last = m_currentHealth;
            if (value != last)
            {
                m_currentHealth = Mathf.Clamp(value, 0, max);
                OnChange?.Invoke();
            }
        }
    }

    public virtual bool isEmpty => current == 0;

    protected float m_lastDamageTime;

    public virtual bool recovering => Time.time < m_lastDamageTime + coolDown;
    public UnityEvent OnDamage;

    public virtual void Damage(int amount)
    {
        if (!recovering)
        {
            current -= Mathf.Abs(amount);
            m_lastDamageTime = Time.time;
            OnDamage?.Invoke();
        }
    }

    public virtual void Increase(int amount) => current = Mathf.Clamp(current + amount, 0, max);

    public virtual void Reset()
    {
        current = initial;
    }

    protected virtual void Awake()
    {
        current = initial;
    }
}