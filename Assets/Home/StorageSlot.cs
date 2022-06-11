using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Home
{
    public class StorageSlot : MonoBehaviour, IPointerClickHandler
    {
        public event Action<int> OnStorageSlotClicked;

        [SerializeField] private Image _monsterSprite;
        [SerializeField] private GameObject _checkMark;

        public int Index { get; private set; }
        public bool HasMonster { get; private set; }

        private Color _visible = new Color(1, 1, 1, 1);
        private Color _invisible = new Color(1, 1, 1, 0);

        public void SetMonster(MonsterStorageData monsterData)
        {
            _monsterSprite.color = _visible;
            _monsterSprite.sprite = monsterData.StorageImage;
            HasMonster = true;
            _checkMark.SetActive(monsterData.Active);
            
        }

        public void RemoveMonster()
        {
            _monsterSprite.color = _invisible;
            _monsterSprite.sprite = null;
            HasMonster = false;
            _checkMark.SetActive(false);
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