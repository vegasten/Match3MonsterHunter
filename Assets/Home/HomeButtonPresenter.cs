using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Home
{
    public class HomeButtonPresenter : MonoBehaviour
    {
        public event Action OnBattleButtonClicked;
        public event Action OnStorageButtonClicked;
        public event Action OnBackButtonClicked;

        [SerializeField] private Button _battleButton;
        [SerializeField] private Button _storageButton;
        [SerializeField] private Button _backButton;

        private void Start()
        {
            _battleButton.onClick.AddListener(onBattleButtonClicked);
            _storageButton.onClick.AddListener(onStorageButtonClicked);
            _backButton.onClick.AddListener(onBackButtonClicked);
        }

        private void onBattleButtonClicked()
        {
            OnBattleButtonClicked?.Invoke();

            SceneManager.LoadScene("BattleScene");
        }

        private void onStorageButtonClicked()
        {
            OnStorageButtonClicked?.Invoke();
        }

        private void onBackButtonClicked()
        {
            OnBackButtonClicked?.Invoke();
        }
    }
}