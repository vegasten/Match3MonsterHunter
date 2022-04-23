using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Temporary monster")]
    [SerializeField] private BattleMonster _friendlyBattleMonster;
    [SerializeField] private BattleMonster _enemyBattleMonster;

    [Header("Battle Presenter")]
    [SerializeField] private BattlePresenter _presenter;

    private PlayerData _friendlyMonsterData;
    private PlayerData _enemyMonsterData;

    private void Start()
    {
        StartBattle();
    }

    public void StartBattle()
    {
        _friendlyMonsterData = new PlayerData(100);
        _enemyMonsterData = new PlayerData(100);

        _presenter.SetHealthBar(1, Players.Player);
        _presenter.SetHealthBar(1, Players.Enemy);
    }

    public void RegisterComboResult(int numberOfCombos, int numberOfTiles)
    {
        int damage = CalculateDamage(numberOfCombos, numberOfTiles);
        _enemyMonsterData.TakeDamage(damage);

        if (numberOfCombos <= 2)
        {
            StartCoroutine(AnimateFriendlyAttack());
        }
        else
        {
            StartCoroutine(AnimateFriendlyBigAttack());
        }

        Debug.Log($"Doing {damage} points of damage to the enemy");

        if (_enemyMonsterData.Life <= 0)
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

    private IEnumerator AnimateFriendlyAttack()
    {
        _friendlyBattleMonster.TriggerAttack();
        yield return new WaitForSeconds(0.2f);
        _enemyBattleMonster.TriggerHit();
        yield return new WaitForSeconds(0.5f);
        _presenter.SetHealthBar(_enemyMonsterData.GetPercentLife(), Players.Enemy);
    }

    private IEnumerator AnimateFriendlyBigAttack()
    {
        _friendlyBattleMonster.TriggerBigAttack();
        yield return new WaitForSeconds(1.5f);
        _enemyBattleMonster.TriggerHit();
        yield return new WaitForSeconds(0.5f);
        _presenter.SetHealthBar(_enemyMonsterData.GetPercentLife(), Players.Enemy);
    }

    private IEnumerator AnimateEnemyAttack()
    {
        _enemyBattleMonster.TriggerAttack();
        yield return new WaitForSeconds(0.2f);
        _friendlyBattleMonster.TriggerHit();
        yield return new WaitForSeconds(0.5f);
        _presenter.SetHealthBar(_friendlyMonsterData.GetPercentLife(), Players.Enemy);
    }

    private IEnumerator AnimateEnemyBigAttack()
    {
        _enemyBattleMonster.TriggerBigAttack();
        yield return new WaitForSeconds(1.5f);
        _friendlyBattleMonster.TriggerHit();
        yield return new WaitForSeconds(0.5f);
        _presenter.SetHealthBar(_friendlyMonsterData.GetPercentLife(), Players.Enemy);
    }
}
