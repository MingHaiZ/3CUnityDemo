using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class Game : Singleton<Game>
{
    public UnityEvent<int> OnRetriesSet;
    public int initialRetires = 3;

    public int m_dataIndex;
    public DateTime m_createdAt;
    public DateTime m_updateAt;

    public int m_retries;

    public int retries
    {
        get { return m_retries; }
        set
        {
            m_retries = value;
            OnRetriesSet?.Invoke(m_retries);
        }
    }

    public UnityEvent OnSavingRequested;

    public List<GameLevel> levels;

    public virtual void LoadState(int index, GameData data)
    {
        m_dataIndex = index;
        m_retries = data.retries;
        m_createdAt = DateTime.Parse(data.createdAt);
        m_updateAt = DateTime.Parse(data.updatedAt);

        for (int i = 0; i < data.levels.Length; i++)
        {
            levels[i].LoadState(data.levels[i]);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public static void LockCursor(bool value = true)
    {
#if UNITY_STANDALONE
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
#endif
    }

    public virtual GameLevel GetCurrentLevel()
    {
        var scene = GameLoader.instance.currentScene;
        return levels.Find((level) => level.scene == scene);
    }

    public virtual int GetCurrentLevelIndex()
    {
        var scene = GameLoader.instance.currentScene;
        return levels.FindIndex((level) => level.scene == scene);
    }

    public virtual void UnlockNextLevel()
    {
        var index = GetCurrentLevelIndex() + 1;
        if (index >= 0 && index < levels.Count)
        {
            levels[index].locked = false;
        }
    }

    public virtual LevelData[] LevelData()
    {
        return levels.Select(level => level.ToData()).ToArray();
    }

    public virtual GameData ToData()
    {
        return new GameData()
        {
            retries = m_retries,
            levels = LevelData(),
            createdAt = m_createdAt.ToString(),
            updatedAt = m_updateAt.ToString()
        };
    }

    public virtual void RequestSaving()
    {
        GameSaver.instance.Save(ToData(), m_dataIndex);
        OnSavingRequested?.Invoke();
    }
}