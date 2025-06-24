using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Palmmedia.ReportGenerator.Core.Common;
using UnityEngine;

public class GameSaver : Singleton<GameSaver>
{
    protected static readonly int TotalSlots = 5;

    public enum Mode
    {
        Binary,
        JSON,
        PlayerPrefabs
    }

    public Mode mode = Mode.Binary;
    public string binaryFileExtension = "data";
    public string fileName = "save";

    public virtual GameData[] LoadList()
    {
        var list = new GameData[TotalSlots];

        for (int i = 0; i < TotalSlots; i++)
        {
            var data = Load(i);

            if (data != null)
            {
                list[i] = data;
            }
        }

        return list;
    }

    public virtual GameData Load(int index)
    {
        switch (mode)
        {
            default:
            case Mode.Binary:
                return LoadBinary(index);
            case Mode.JSON:
                return LoadJSON(index);
            case Mode.PlayerPrefabs:
                return LoadPlayerPrefabs(index);
        }
    }

    protected virtual void SaveBinary(GameData data, int index)
    {
        var path = GetFilePath(index);
        var formatter = new BinaryFormatter();
        var stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    protected virtual void SaveJson(GameData data, int index)
    {
        var path = GetFilePath(index);
        var json = data.ToJson();
        File.WriteAllText(path, json);
    }

    protected virtual void SavePlayerPrefabs(GameData data, int index)
    {
        var json = data.ToJson();
        var path = GetFilePath(index);
        File.WriteAllText(path, json);
    }

    protected virtual GameData LoadPlayerPrefabs(int index)
    {
        var key = index.ToString();
        if (PlayerPrefs.HasKey(key))
        {
            var json = PlayerPrefs.GetString(key);
            return GameData.FormJson(json);
        }

        return null;
    }

    protected virtual GameData LoadJSON(int index)
    {
        var patch = GetFilePath(index);
        if (File.Exists(patch))
        {
            var json = File.ReadAllText(patch);
            return GameData.FormJson(json);
        }

        return null;
    }

    protected virtual GameData LoadBinary(int index)
    {
        var patch = GetFilePath(index);
        if (File.Exists(patch))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(patch, FileMode.Open);
            var data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }

        return null;
    }

    protected virtual string GetFilePath(int index)
    {
        var extension = mode == Mode.Binary ? "Json" : binaryFileExtension;
        return Application.persistentDataPath + $"/{fileName}_{index}.{extension}";
    }

    public virtual void Save(GameData data, int index)
    {
        switch (mode)
        {
            default:
            case Mode.Binary:
                SaveBinary(data, index);
                break;
            case Mode.JSON:
                SaveJson(data, index);
                break;
            case Mode.PlayerPrefabs:
                SavePlayerPrefabs(data, index);
                break;
        }
    }
}