
public class PlayerData
{
    public int Life { get; private set; }
    public int MaxLife { get; }

    public PlayerData(int life)
    {
        Life = life;
        MaxLife = life;
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
