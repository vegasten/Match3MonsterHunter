using System;
using UnityEngine;
using UnityEngine.UI;

namespace Home
{
    public class HomeButtonPresenter : MonoBehaviour
    {
        public event Action OnBattleButtonClicked;
        public event Action OnStorageButtonClicked;
        public event Action OnSummonButtonClicked;
        public event Action OnBackButtonClicked;

        [SerializeField] private Button _battleButton;
        [SerializeField] private Button _storageButton;
        [SerializeField] private Button _summonButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _summoningBackButton;

        private void Start()
        {
            _battleButton.onClick.AddListener(onBattleButtonClicked);
            _storageButton.onClick.AddListener(onStorageButtonClicked);
            _summonButton.onClick.AddListener(onSummonButtonClicked);
            _backButton.onClick.AddListener(onBackButtonClicked);
            _summoningBackButton.onClick.AddListener(onBackButtonClicked);
        }

        private void onBattleButtonClicked()
        {
            OnBattleButtonClicked?.Invoke();
        }

        private void onStorageButtonClicked()
        {
            OnStorageButtonClicked?.Invoke();
        }

        private void onSummonButtonClicked()
        {
            OnSummonButtonClicked?.Invoke();
        }

        private void onBackButtonClicked()
        {
            OnBackButtonClicked?.Invoke();
        }
    }
}