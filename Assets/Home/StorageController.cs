using System;
using System.Collections.Generic;
using UnityEngine;

namespace Home
{
    public class StorageController : MonoBehaviour
    {
        [SerializeField] private StoragePresenter _storagePresenter;
        [SerializeField] private MonsterList _monsterList;
        [SerializeField] private HomeUIController _homeUIController;

        [Header("Scene")]
        [SerializeField] private Transform _sceneMonsterSpot;

        private List<MonsterData> _monsters;
        private int _activeMonsterIndex;

        private int _deleteMonsterIndex;

        private void Start()
        {
            // DEBUG
            //ResetToYellowSlime();
            // DEBUG

            _monsters = StateSaver.Instance.LoadMonsters();
            if (_monsters.Count == 0)
                Debug.LogWarning("Loaded zero monsters");

            for (int i = 0; i < _monsters.Count; i++)
            {
                if (_monsters[i].Active)
                {
                    _activeMonsterIndex = i;
                    break;
                }
            }

            _storagePresenter.SetStorageMonsters(GetStorageData());
            _storagePresenter.OnNewActiveMonsterSet += SetNewActiveMonster;
            _storagePresenter.OnDeleteMonsterRequested += AskForMonsterDeletionConfirmation;
            _storagePresenter.OnConfirmDeletion += ConfirmMonsterDeletion;
            _storagePresenter.OnCancelDeletion += CancelDeletion;

            _homeUIController.OnBack += OnGoingBack;

            SetSceneMonster(_monsters[_activeMonsterIndex].MonsterType);
        }

        private void OnGoingBack()
        {
            _storagePresenter.HideDeletionModal();
        }

        private void AskForMonsterDeletionConfirmation(int index)
        {
            if (_monsters.Count <= 1)
            {
                return; // TODO Feedback to user about not being able to delete her last monster
            }

            _deleteMonsterIndex = index;
            _storagePresenter.ShowDeletionModal();
        }

        private void ConfirmMonsterDeletion()
        {
            _monsters.RemoveAt(_deleteMonsterIndex);

            if (_activeMonsterIndex == _deleteMonsterIndex)
            {
                _activeMonsterIndex = 0;
                _monsters[0].Active = true;
            }

            SaveMonstersToFile();
            _storagePresenter.SetStorageMonsters(GetStorageData());
            _storagePresenter.HideDeletionModal();
        }

        private void CancelDeletion()
        {
            _storagePresenter.HideDeletionModal();
        }

        // DEBUG DEBUG DEBUG
        // Run this to reset storage to a single yellow slime
        public void ResetToYellowSlime()
        {
            _monsters = new List<MonsterData>();
            _monsters.Add(new MonsterData()
            {
                MonsterType = MonsterListEnum.SlimeYellow,
                Health = 100,
                AttackPower = 1,
                Active = true
            });

            SaveMonstersToFile();
        }

        public void AddMonsterFromSummoning(MonsterSummoningData monsterSummoningData)
        {
            var monsterData = _monsterList.GetDefaultMonsterDataFromSummoningData(monsterSummoningData);
            _monsters.Add(monsterData);
            SaveMonstersToFile();
            _storagePresenter.SetStorageMonsters(GetStorageData());
        }

        public MonsterData GetActiveMonster()
        {
            return _monsters[_activeMonsterIndex];
        }

        private void SaveMonstersToFile()
        {
            StateSaver.Instance.SaveMonsters(_monsters);
        }

        private void SetNewActiveMonster(int index)
        {
            if (_activeMonsterIndex == index)
                return;

            _monsters[_activeMonsterIndex].Active = false;
            _storagePresenter.SetAsActiveMonster(_activeMonsterIndex, false);
            _monsters[index].Active = true;
            _storagePresenter.SetAsActiveMonster(index, true);

            _activeMonsterIndex = index;

            SaveMonstersToFile();

            SetSceneMonster(_monsters[index].MonsterType);
        }

        private void SetSceneMonster(MonsterListEnum monsterType)
        {
            var monsterPrefab = _monsterList.GetBattleGameObject(monsterType);

            foreach(Transform child in _sceneMonsterSpot)
            {
                Destroy(child.gameObject);
            }

            Instantiate(monsterPrefab, _sceneMonsterSpot);
        }

        private List<MonsterStorageData> GetStorageData()
        {
            var storageData = new List<MonsterStorageData>();

            foreach (var monster in _monsters)
            {
                var generalStorageData = _monsterList.GetStorageData(monster.MonsterType);
                storageData.Add(
                    new MonsterStorageData
                    {
                        StorageImage = generalStorageData.StorageImage,
                        Active = monster.Active
                    }
                );
            }

            return storageData;
        }
    }
}