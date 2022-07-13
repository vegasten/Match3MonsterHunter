using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Home
{
    public class StoragePresenter : MonoBehaviour
    {
        public event Action<int> OnNewActiveMonsterSet;
        public event Action<int> OnDeleteMonsterRequested;
        public event Action OnConfirmDeletion;
        public event Action OnCancelDeletion;

        [SerializeField] private Transform _storageSlotsParent;
        [SerializeField] private StorageSlot _storageSlotPrefab;
        [SerializeField] private int _numberOfStorageSlots = 12;

        [Header("Deletion")]
        [SerializeField] private Button _confirmDeletionButton;
        [SerializeField] private Button _cancelDeletionButton;
        [SerializeField] private GameObject _deletionModal;


        private List<StorageSlot> _storageSlots = new List<StorageSlot>();

        private void Awake()
        {
            for (int i = 0; i < _numberOfStorageSlots; i++)
            {
                var storageSlot = Instantiate(_storageSlotPrefab, _storageSlotsParent).GetComponent<StorageSlot>();
                _storageSlots.Add(storageSlot);
                storageSlot.SetIndex(i);
                storageSlot.OnStorageSlotClicked += OnStorageSlotClicked;
                storageSlot.OnDeleteMonsterRequested += RequestDeleteMonster;
            }
        }

        private void Start()
        {
            _confirmDeletionButton.onClick.AddListener(OnDeletionConfirmed);
            _cancelDeletionButton.onClick.AddListener(OnDeletionCanceled);
        }

        private void OnDeletionConfirmed()
        {
            OnConfirmDeletion?.Invoke();
        }

        private void OnDeletionCanceled()
        {
            OnCancelDeletion?.Invoke();
        }

        private void RequestDeleteMonster(int index)
        {
            OnDeleteMonsterRequested?.Invoke(index);
        }

        public void SetStorageMonsters(List<MonsterStorageData> monsters)
        {
            ClearStorageSlots();

            for (int i = 0; i < monsters.Count; i++)
            {
                var slot = _storageSlots[i];
                var monster = monsters[i];

                slot.SetMonster(monster);
                slot.SetIndex(i);
            }
        }

        public void ShowDeletionModal()
        {
            _deletionModal.SetActive(true);
        }

        public void HideDeletionModal()
        {
            _deletionModal.SetActive(false);
        }

        private void ClearStorageSlots()
        {
            for (int i = 0; i < _storageSlots.Count; i++)
            {
                _storageSlots[i].RemoveMonster();
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