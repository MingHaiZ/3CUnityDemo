using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameData
{
    public int retries;
    public LevelData[] levels;
    public string createdAt;
    public string updatedAt;

    public static GameData FormJson(string json)
    {
        return JsonUtility.FromJson<GameData>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static GameData Create()
    {
        return new GameData()
        {
            retries = Game.instance.initialRetires,
            createdAt = DateTime.UtcNow.ToString(),
            updatedAt = DateTime.UtcNow.ToString(),
            levels = Game.instance.levels.Select((level) =>
            {
                return new LevelData()
                {
                    locked = level.locked
                };
            }).ToArray()
        };
    }
}