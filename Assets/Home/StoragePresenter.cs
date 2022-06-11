using System.Collections.Generic;
using UnityEngine;

namespace Home
{
    public class StoragePresenter : MonoBehaviour
    {
        [SerializeField] Transform _storageSlotsParent;
        [SerializeField] StorageSlot _storageSlotPrefab;
        [SerializeField] int _numberOfStorageSlots = 12;

        private List<StorageSlot> _storageSlots = new List<StorageSlot>();
        private int _activeStorageSlotIndex = 0; // TODO, The presenter should probably not have control of this state

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

        public int GetActiveMonsterIndex()
        {
            return _activeStorageSlotIndex;
        }

        public void SetStorageMonsters(List<MonsterStorageData> monsters)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                var slot = _storageSlots[i];
                var monster = monsters[i];

                slot.SetMonster(monster);
                slot.SetIndex(i);

                if (monster.Active)
                {
                    _activeStorageSlotIndex = i;
                }
            }
        }

        private void OnStorageSlotClicked(int index)
        {
            if (index == _activeStorageSlotIndex)
                return;

            var storageSlot = _storageSlots[index];

            if (storageSlot.HasMonster)
            {
                _storageSlots[_activeStorageSlotIndex].SetAsActiveMonster(false);
                storageSlot.SetAsActiveMonster(true);
                _activeStorageSlotIndex = index;
            }
        }
    }
}