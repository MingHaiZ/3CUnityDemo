public class GameSaver : Singleton<GameSaver>
{
    protected static readonly int TotalSlots = 5;

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
}