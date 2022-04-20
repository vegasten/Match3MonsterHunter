using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattlePresenter _presenter;

    private PlayerData _player;
    private PlayerData _enemy;

    private void Start()
    {
        StartBattle();
    }

    public void StartBattle()
    {
        _player = new PlayerData(100);
        _enemy = new PlayerData(100);

        _presenter.SetHealthBar(1, Players.Player);
        _presenter.SetHealthBar(1, Players.Enemy);
    }

    public void RegisterComboResult(int numberOfCombos, int numberOfTiles)
    {
        int damage = CalculateDamage(numberOfCombos, numberOfTiles);
        _enemy.TakeDamage(damage);
        _presenter.SetHealthBar(_enemy.GetPercentLife(), Players.Enemy);
        Debug.Log($"Doing {damage} points of damage to the enemy");

        if (_enemy.Life <= 0)
        {
            Debug.Log("Enemy is dead!");
            _presenter.SetVictoryText();
        }
    }

    public void ClearComboText()
    {
        _presenter.ClearComboText();
    }

    public void DisplayOngoingCombo(int numberOfCombos)
    {
        _presenter.DisplayCombos(numberOfCombos);
    }

    private int CalculateDamage(int numberOfCombos, int numberOfTiles)
    {
        return numberOfCombos * numberOfTiles;
    }
}
