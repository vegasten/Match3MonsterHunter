using UnityEngine;

public class PersistantState : MonoBehaviour
{
    public MonsterBattleData PlayerMonster { get; private set; }
    public MonsterBattleData EnemyMonster { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetPlayerMonster(MonsterBattleData monster)
    {
        PlayerMonster = monster;
    }

    public void SetEnemyMonster(MonsterBattleData monster)
    {
        EnemyMonster = monster;
    }
}
