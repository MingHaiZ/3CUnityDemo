using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int initial = 3;
    public int max = 3;
    public UnityEvent OnChange;
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

    public virtual void Reset()
    {
        current = initial;
    }
    
    protected virtual void Awake()
    {
        current = initial;
    }
}