using System;
using System.Collections.Generic;
using UnityEngine;

namespace Home
{
    public class StoragePresenter : MonoBehaviour
    {
        public event Action<int> OnNewActiveMonsterSet;

        [SerializeField] Transform _storageSlotsParent;
        [SerializeField] StorageSlot _storageSlotPrefab;
        [SerializeField] int _numberOfStorageSlots = 12;

        private List<StorageSlot> _storageSlots = new List<StorageSlot>();

        private void Awake()
        {
            for (int i = 0; i < _numberOfStorageSlots; i++)
            {
                var storageSlot = Instantiate(_storageSlotPrefab, _storageSlotsParent).GetComponent<StorageSlot>();
                _storageSlots.Add(storageSlot);
                storageSlot.SetIndex(i);
                storageSlot.OnStorageSlotClicked += OnStorageSlotClicked;
            }
        }

        public void SetStorageMonsters(List<MonsterStorageData> monsters)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                var slot = _storageSlots[i];
                var monster = monsters[i];

                slot.SetMonster(monster);
                slot.SetIndex(i);
            }
        }

        public void SetAsActiveMonster(int index, bool isActive)
        {
            _storageSlots[index].SetAsActiveMonster(isActive);
        }

        private void OnStorageSlotClicked(int index)
        {            
            var storageSlot = _storageSlots[index];

            if (storageSlot.HasMonster)
            {
                OnNewActiveMonsterSet?.Invoke(index);
            }
        }
    }
}