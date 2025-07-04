using System;
using UnityEngine;
using UnityEngine.Events;

public class LevelScore : Singleton<LevelScore>
{
    protected int m_coins;
    protected bool[] m_stars = new bool[GameLevel.StarsPerLevel];

    public bool[] stars => (bool[])m_stars.Clone();
    public float time { get; protected set; }

    protected Game m_game;
    protected GameLevel m_level;

    public int coins
    {
        get { return m_coins; }
        set
        {
            m_coins = value;
            OnCoinsSet?.Invoke(m_coins);
        }
    }

    public UnityEvent<int> OnCoinsSet;
    public UnityEvent<bool[]> OnStarsSet;
    public UnityEvent OnScoreLoaded;

    public bool stopTime { get; set; } = true;

    protected virtual void Start()
    {
        m_game = Game.instance;
        m_level = m_game?.GetCurrentLevel();

        if (m_level != null)
        {
            m_stars = (bool[])m_level.stars.Clone();
        }

        OnScoreLoaded?.Invoke();
    }

    private void Update()
    {
        if (!stopTime)
        {
            time += Time.deltaTime;
        }
    }

    public virtual void Consolidata()
    {
        if (m_level != null)
        {
            if (m_level.time == 0 || time < m_level.time)
            {
                m_level.time = time;
            }

            if (coins > m_level.coins)
            {
                m_level.coins = coins;
            }

            m_level.stars = (bool[])stars.Clone();
            m_game.RequestSaving();
        }
    }

    public virtual void CollectStar(int index)
    {
        if (index >= m_stars.Length)
        {
            return;
        }

        m_stars[index] = true;
        OnStarsSet.Invoke(m_stars);
    }
}