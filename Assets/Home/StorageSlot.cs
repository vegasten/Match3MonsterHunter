using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Home
{
    public class StorageSlot : MonoBehaviour, IPointerClickHandler
    {
        public event Action<int> OnStorageSlotClicked;
        public event Action<int> OnDeleteMonsterRequested;

        [SerializeField] private Image _monsterSprite;
        [SerializeField] private GameObject _checkMark;
        [SerializeField] private Button _deleteMonsterButton;

        public int Index { get; private set; }
        public bool HasMonster { get; private set; }

        private Color _visible = new Color(1, 1, 1, 1);
        private Color _invisible = new Color(1, 1, 1, 0);

        private void Start()
        {
            _deleteMonsterButton.onClick.AddListener(DeleteMonster);
        }

        private void DeleteMonster()
        {
            OnDeleteMonsterRequested?.Invoke(Index);
        }

        public void SetMonster(MonsterStorageData monsterData)
        {
            _monsterSprite.color = _visible;
            _monsterSprite.sprite = monsterData.StorageImage;
            HasMonster = true;
            _checkMark.SetActive(monsterData.Active);
            _deleteMonsterButton.gameObject.SetActive(true);
        }

        public void RemoveMonster()
        {
            _monsterSprite.color = _invisible;
            _monsterSprite.sprite = null;
            HasMonster = false;
            _checkMark.SetActive(false);
            _deleteMonsterButton.gameObject.SetActive(false);
        }

        public void SetAsActiveMonster(bool isActive)
        {
            _checkMark.SetActive(isActive);
        }    

        public void SetIndex(int index)
        {
            Index = index;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnStorageSlotClicked?.Invoke(Index);
        }
    }
}