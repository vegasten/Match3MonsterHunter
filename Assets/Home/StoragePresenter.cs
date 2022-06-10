using System.Collections.Generic;
using UnityEngine;

namespace Home {
    public class StoragePresenter : MonoBehaviour
    {
        [SerializeField] Transform _storageSlotsParent;
        [SerializeField] StorageSlot _storageSlotPrefab;
        [SerializeField] int _numberOfStorageSlots = 12;


        [Header("test")]
        [SerializeField] MonsterScriptableObject _yellowSlime;
        [SerializeField] MonsterScriptableObject _greenSlime;

        private List<StorageSlot> _storageSlots = new List<StorageSlot>();
        private int _activeStorageSlotIndex = 0;

        private void Start()
        {
            for (int i = 0; i < _numberOfStorageSlots; i++)
            {
                var storageSlot = Instantiate(_storageSlotPrefab, _storageSlotsParent).GetComponent<StorageSlot>();
                _storageSlots.Add(storageSlot);
                storageSlot.SetIndex(i);
                storageSlot.OnStorageSlotClicked += OnStorageSlotClicked;
            }

            _storageSlots[0].SetMonster(_yellowSlime);
            _storageSlots[0].SetAsActiveMonster(true);
            _storageSlots[1].SetMonster(_greenSlime);
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