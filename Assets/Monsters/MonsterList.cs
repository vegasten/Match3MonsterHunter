
using System;
using UnityEngine;

public enum MonsterListEnum
{
    SlimeGreen = 0,
    SlimeBlue = 1,
    SlimeRed = 2,
    SlimeYellow = 3,
    SlimePurple = 4
};

public class MonsterList : MonoBehaviour
{
    const int NUMBER_OF_MONSTERS = 5;

    [SerializeField] private MonsterScriptableObject _slimeGreen;
    [SerializeField] private MonsterScriptableObject _slimeBlue;
    [SerializeField] private MonsterScriptableObject _slimeRed;
    [SerializeField] private MonsterScriptableObject _slimeYellow;
    [SerializeField] private MonsterScriptableObject _slimePurple;

    private MonsterScriptableObject GetMonsterData(MonsterListEnum monster)
    {
        MonsterScriptableObject targetMonster = null;

        switch (monster)
        {
            case MonsterListEnum.SlimeGreen:
                targetMonster = _slimeGreen;
                break;
            case MonsterListEnum.SlimeBlue:
                targetMonster = _slimeBlue;
                break;
            case MonsterListEnum.SlimeRed:
                targetMonster = _slimeRed;
                break;
            case MonsterListEnum.SlimeYellow:
                targetMonster = _slimeYellow;
                break;
            case MonsterListEnum.SlimePurple:
                targetMonster = _slimePurple;
                break;
            default:
                throw new ArgumentException($"Trying to access monster {monster}, which doesn't exist");
        }

        return targetMonster;
    }

    public MonsterStorageData GetStorageData(MonsterListEnum monster)
    {
        var allData = GetMonsterData(monster);
        var storageData = new MonsterStorageData()
        {
            StorageImage = allData.StorageImage
        };

        return storageData;
    }

    public GameObject GetBattleGameObject(MonsterListEnum monster)
    {
        return GetMonsterData(monster).BattleGameObject;
    }

    public MonsterBattleData GetRandomMonsterBattleDataUniform()
    {
        var randomMonster = GetRandomMonsterUniform();
        var allData = GetMonsterData(randomMonster);
        var battleData = new MonsterBattleData()
        {
            Health = allData.Health,
            AttackPower = allData.AttackPower,
            BattlePrefab = allData.BattleGameObject
        };

        return battleData;
    }

    private MonsterListEnum GetRandomMonsterUniform()
    {
        int randomNumber = UnityEngine.Random.Range(0, NUMBER_OF_MONSTERS);
        return (MonsterListEnum)randomNumber;
    }
}