using System.Collections.Generic;

public class StateSaver
{
    private static StateSaver _instance;

    public static StateSaver Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new StateSaver();
            }
            return _instance;
        }
    }

    private StateSaver() { }

    public void SaveMonsters(List<MonsterData> monsters)
    {

    }

    public List<MonsterData> LoadMonsters()
    {
        var monsters = new List<MonsterData>();

        // TODO Just for testing
        var slimeYellow = new MonsterData
        {
            MonsterType = MonsterListEnum.SlimeYellow,
            Health = 100,
            AttackPower = 1,
            Level = 2,
            Active = false
        };

        var slimeRed = new MonsterData
        {
            MonsterType = MonsterListEnum.SlimeRed,
            Health = 100,
            AttackPower = 1,
            Level = 3,
            Active = true
        };

        monsters.Add(slimeYellow);
        monsters.Add(slimeRed);

        return monsters;
    }
}
