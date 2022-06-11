using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Battle spots")]
    [SerializeField] private Transform _friendlyBattleSpot;
    [SerializeField] private Transform _enemyBattleSpot;

    [Header("Battle Presenter")]
    [SerializeField] private BattlePresenter _presenter;

    [Header("Board")]
    [SerializeField] private GameBoardManager _boardManager;

    private MonsterBattleState _friendlyMonsterData;
    private MonsterBattleState _enemyMonsterData;
    private BattleMonster _friendlyBattleMonster;
    private BattleMonster _enemyBattleMonster;

    private PersistantState _persistantState;

    private Players _attacker;
    private bool _batteHasEnded = false;

    private void Start()
    {
        _persistantState = GameObject.FindObjectOfType<PersistantState>();

        StartBattle();
    }

    public void StartBattle()
    {
        _attacker = Players.Friendly;

        _friendlyMonsterData = new MonsterBattleState(_persistantState.PlayerMonster);
        _enemyMonsterData = new MonsterBattleState(_persistantState.EnemyMonster);

        var friendlyMonsterPrefab = _persistantState.PlayerMonster.BattlePrefab;
        var enemyMonsterPrefab = _persistantState.EnemyMonster.BattlePrefab;

        _friendlyBattleMonster = Instantiate(friendlyMonsterPrefab, _friendlyBattleSpot).GetComponent<BattleMonster>();
        _enemyBattleMonster = Instantiate(enemyMonsterPrefab, _enemyBattleSpot).GetComponent<BattleMonster>();

        _presenter.SetHealthBar(1, Players.Friendly);
        _presenter.SetHealthBar(1, Players.Enemy);

        _presenter.DisplayActiveAttacer(_attacker);
        _boardManager.StartTurn(_attacker);
    }

    public void RegisterComboResult(int numberOfCombos, int numberOfTiles)
    {
        int damage = CalculateDamage(numberOfCombos, numberOfTiles);

        if (_attacker == Players.Friendly)
        {
            _enemyMonsterData.TakeDamage(damage);
            if (numberOfCombos <= 2)
                StartCoroutine(AnimateFriendlyAttack());
            else
                StartCoroutine(AnimateFriendlyBigAttack());

            if (_enemyMonsterData.Life <= 0)
            {
                _presenter.EndBattleWithVictory();
                _batteHasEnded = true;
                _enemyBattleMonster.TriggerDeath();
            }
        }
        else
        {
            _friendlyMonsterData.TakeDamage(damage);
            if (numberOfCombos <= 2)
                StartCoroutine(AnimateEnemyAttack());
            else
                StartCoroutine(AnimateEnemyBigAttack());

            if (_friendlyMonsterData.Life <= 0)
            {
                _presenter.EndBattleWithDefeat();
                _batteHasEnded = true;
                _friendlyBattleMonster.TriggerDeath();
            }
        }

        if (!_batteHasEnded)
        {
            StartCoroutine(StartNextTurnAfterDelay(2.0f));
        }
    }

    private IEnumerator StartNextTurnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextTurn();
    }

    private void StartNextTurn()
    {
        _attacker = _attacker == Players.Friendly ? Players.Enemy : Players.Friendly;
        _presenter.DisplayActiveAttacer(_attacker);
        _boardManager.StartTurn(_attacker);
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
        _presenter.SetHealthBar(_friendlyMonsterData.GetPercentLife(), Players.Friendly);
    }

    private IEnumerator AnimateEnemyBigAttack()
    {
        _enemyBattleMonster.TriggerBigAttack();
        yield return new WaitForSeconds(1.5f);
        _friendlyBattleMonster.TriggerHit();
        yield return new WaitForSeconds(0.5f);
        _presenter.SetHealthBar(_friendlyMonsterData.GetPercentLife(), Players.Friendly);
    }
}
