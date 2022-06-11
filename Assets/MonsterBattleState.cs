
public class MonsterBattleState
{
    public int Life { get; private set; }
    public int MaxLife { get; }
    public int StartingAttackPower { get; }
    
    private int _attackPower;



    public MonsterBattleState(MonsterBattleData data)
    {
        Life = data.Health;
        MaxLife = data.Health;
        StartingAttackPower = data.AttackPower;
        _attackPower = data.AttackPower;
    }

    public void TakeDamage(int damage)
    {
        Life -= damage;

        if (Life < 0)
            Life = 0;
    }

    public float GetPercentLife()
    {
        return (float)Life / (float)MaxLife;
    }
}
