using System;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Effects")]
    public AudioClip death;

    protected Enemy m_enemy;
    protected AudioSource m_audioSource;

    protected virtual void InitializeEnemy() => m_enemy = GetComponent<Enemy>();

    protected virtual void InitializeAudio()
    {
        if (!TryGetComponent(out m_audioSource))
        {
            m_audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected virtual void InitializeCallBacks()
    {
        m_enemy.enemyEvents.OnDie.AddListener(() => m_audioSource.PlayOneShot(death));
    }

    private void Start()
    {
        InitializeEnemy();
        InitializeAudio();
        InitializeCallBacks();
    }
}