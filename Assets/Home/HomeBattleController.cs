using UnityEngine;
using UnityEngine.SceneManagement;

namespace Home
{
    public class HomeBattleController : MonoBehaviour
    {
        [SerializeField] private MonsterList _monsterList;

        [Header("Controllers")]
        public StorageController _storageController;

        PersistantState _persistantState;

        private void Start()
        {
            _persistantState = GameObject.FindObjectOfType<PersistantState>();
        }

        public void StartBattle()
        {
            SetPlayerMonster();
            SetRandomEnemyMonster();
            LoadBattleScene();
        }

        private void SetPlayerMonster()
        {
            var monsterData = _storageController.GetActiveMonster();
            var monsterBattleGameObject = _monsterList.GetBattleGameObject(monsterData.MonsterType);

            var monsterBattleData = new MonsterBattleData()
            {
                Health = monsterData.Health,
                AttackPower = monsterData.AttackPower,
                BattlePrefab = monsterBattleGameObject
            };

            _persistantState.SetPlayerMonster(monsterBattleData);
        }

        private void SetRandomEnemyMonster()
        {
            var randomMonster = _monsterList.GetRandomMonsterBattleDataUniform();
            _persistantState.SetEnemyMonster(randomMonster);
        }

        private void LoadBattleScene()
        {
            SceneManager.LoadScene("BattleScene");
        }
    }
}