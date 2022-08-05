using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleManager : MonoBehaviour
    {
        public static bool IsBoardInputEnabled;

        [Header("Battle spots")]
        [SerializeField] private Transform _friendlyBattleSpot;
        [SerializeField] private Transform _enemyBattleSpot;

        [Header("Battle Presenter")]
        [SerializeField] private BattlePresenter _presenter;

        [Header("Board")]
        [SerializeField] private GameBoardManager _boardManager;

        [Header("Cutscene")]
        [SerializeField] private StartBattleCutscene _startBattleCutscene;
        [SerializeField] private GameObject _battleUI;

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

            _battleUI.SetActive(false);
            IsBoardInputEnabled = false;

            _startBattleCutscene.StartCutScene();
            _startBattleCutscene.OnCutSceneComplete += OnStartCutsceneEnabled;
        }

        private void OnStartCutsceneEnabled()
        {
            IsBoardInputEnabled = true;
            _battleUI.SetActive(true);
        }

        public void StartBattle()
        {
            _attacker = Players.Friendly;

            // DEBUG
            _persistantState.EnemyMonster.Health = 1;

            _friendlyMonsterData = new MonsterBattleState(_persistantState.PlayerMonster);
            _enemyMonsterData = new MonsterBattleState(_persistantState.EnemyMonster);

            var friendlyMonsterPrefab = _persistantState.PlayerMonster.BattlePrefab;
            var enemyMonsterPrefab = _persistantState.EnemyMonster.BattlePrefab;

            _friendlyBattleMonster = Instantiate(friendlyMonsterPrefab, _friendlyBattleSpot).GetComponent<BattleMonster>();
            _enemyBattleMonster = Instantiate(enemyMonsterPrefab, _enemyBattleSpot).GetComponent<BattleMonster>();

            _presenter.SetHealthBar(1, Players.Friendly); // 1 as in percent
            _presenter.SetHealthBar(1, Players.Enemy); // 1 as in percent

            _presenter.DisplayActiveAttacer(_attacker);
            _boardManager.StartTurn(_attacker);
        }

        public void RegisterComboResult(int numberOfCombos, int numberOfTiles)
        {
            float secondsToWaitBeforeNextRound = 2.0f;
            int damage = CalculateDamage(numberOfCombos, numberOfTiles);

            if (_attacker == Players.Friendly)
            {
                _enemyMonsterData.TakeDamage(damage);
                if (numberOfCombos <= 2)
                {
                    StartCoroutine(AnimateFriendlyAttack());
                    secondsToWaitBeforeNextRound = 2.0f;
                }
                else
                {
                    StartCoroutine(AnimateFriendlyBigAttack());
                    secondsToWaitBeforeNextRound = 4.0f;
                }

                if (_enemyMonsterData.Life <= 0)
                {
                    EndBattle(true, secondsToWaitBeforeNextRound);
                }
            }
            else
            {
                _friendlyMonsterData.TakeDamage(damage);
                if (numberOfCombos <= 2)
                {
                    StartCoroutine(AnimateEnemyAttack());
                    secondsToWaitBeforeNextRound = 2.0f;
                }
                else
                {
                    StartCoroutine(AnimateEnemyBigAttack());
                    secondsToWaitBeforeNextRound = 4.0f;
                }

                if (_friendlyMonsterData.Life <= 0)
                {
                    EndBattle(false, secondsToWaitBeforeNextRound);
                }
            }

            if (!_batteHasEnded)
            {
                StartCoroutine(StartNextTurnAfterDelay(secondsToWaitBeforeNextRound));
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

        private void EndBattle(bool isVictory, float attackDelay)
        {
            _batteHasEnded = true;
            StartCoroutine(EndBattleDelay(isVictory, attackDelay));
        }

        private IEnumerator EndBattleDelay(bool isVictory, float attackDelay)
        {

            if (isVictory)
            {
                yield return new WaitForSeconds(attackDelay);
                _enemyBattleMonster.TriggerDeath();
                yield return new WaitForSeconds(2.0f);
                BattleWon();

            }
            else
            {
                yield return new WaitForSeconds(attackDelay);
                _friendlyBattleMonster.TriggerDeath();
                yield return new WaitForSeconds(2.0f);
                BattleLost();
            }
        }

        private void BattleWon()
        {
            var loot = LootMaster.GetLootForVictory();
            SaveLoot(loot);
            var lootString = GetLootString(loot);

            _presenter.EndBattleWithVictory(lootString);
        }

        private void SaveLoot(List<ILoot> loot)
        {
            var currentCurrency = StateSaver.Instance.LoadCurrency();
            int numberOfNewScrolls = 0;

            foreach (var item in loot)
            {
                if (item is BasicScroll)
                {
                    numberOfNewScrolls++;
                }
            }

            currentCurrency.BasicScrolls += numberOfNewScrolls;
            StateSaver.Instance.SaveCurrency(currentCurrency);
        }

        private string GetLootString(List<ILoot> loot)
        {
            int numberOfScrolls = 0;

            foreach (var item in loot)
            {
                if (item is BasicScroll)
                {
                    numberOfScrolls++;
                }
            }

            if (numberOfScrolls > 0)
            {
                if (numberOfScrolls == 1)
                {
                    return "1 scroll";
                }
                else
                {
                    return $"{numberOfScrolls} scrolls";

                }
            }

            return "";
        }

        private void BattleLost()
        {
            _presenter.EndBattleWithDefeat();
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
}