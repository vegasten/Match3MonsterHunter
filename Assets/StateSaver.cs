using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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
        string fileDestination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(fileDestination))
        {
            file = File.OpenWrite(fileDestination);
        }
        else
        {
            file = File.Create(fileDestination);
        }

        var serializableList = new List<MonsterDataSerializeable>();

        foreach (var monster in monsters)
        {
            serializableList.Add(GetSerializableData(monster));
        }

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, serializableList.ToArray());
        file.Close();
    }

    public List<MonsterData> LoadMonsters()
    {
        string fileDestination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        var monstersData = new List<MonsterData>();

        if (File.Exists(fileDestination))
        {
            file = File.OpenRead(fileDestination);
        }
        else
        {
            Debug.LogWarning("Could not find monsters save file");
            return monstersData;
        }

        BinaryFormatter bf = new BinaryFormatter();
        var monstersSerializableData = (MonsterDataSerializeable[])bf.Deserialize(file);
        file.Close();

        foreach (var data in monstersSerializableData)
        {
            monstersData.Add(GetMonsterData(data));
        }

        return monstersData;
    }

    private MonsterDataSerializeable GetSerializableData(MonsterData data)
    {
        return new MonsterDataSerializeable()
        {
            MonsterIdentifier = data.MonsterType.ToString(),
            Health = data.Health,
            AttackPower = data.AttackPower,
            Level = data.Level,
            Active = data.Active
        };
    }

    private MonsterData GetMonsterData(MonsterDataSerializeable data)
    {
        return new MonsterData()
        {
            MonsterType = (MonsterListEnum)System.Enum.Parse(typeof(MonsterListEnum), data.MonsterIdentifier),
            Health = data.Health,
            AttackPower = data.AttackPower,
            Level = data.Level,
            Active = data.Active

        };
    }
}
