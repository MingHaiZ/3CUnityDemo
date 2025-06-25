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
}