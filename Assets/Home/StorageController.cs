using System.Collections.Generic;
using UnityEngine;

namespace Home
{
    public class StorageController : MonoBehaviour
    {
        [SerializeField] private StoragePresenter _storagePresenter;
        [SerializeField] private MonsterList _monsterList;

        private List<MonsterData> _monsters;

        public MonsterData GetActiveMonster()
        {
            var index = _storagePresenter.GetActiveMonsterIndex();
            return _monsters[index];
        }

        private void Start()
        {
            _monsters = StateSaver.Instance.LoadMonsters();
            if (_monsters.Count == 0)
                Debug.LogWarning("Loaded zero monsters");

            _storagePresenter.SetStorageMonsters(GetStorageData());
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