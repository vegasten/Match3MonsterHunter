using System;
using UnityEngine;

namespace Home
{
    public class HomeUIController : MonoBehaviour
    {
        private enum Screen
        {
            Main,
            Storage
        }

        [Header("Presenters")]
        [SerializeField] private HomeButtonPresenter _buttonPresenter;

        [Header("Controllers")]
        [SerializeField] private HomeBattleController _battleController;

        [Header("Screens")]
        [SerializeField] private GameObject _mainScreen;
        [SerializeField] private GameObject _storageScreen;

        private Screen _activeScreen = Screen.Main;

        private void Start()
        {
            _buttonPresenter.OnStorageButtonClicked += () => ShowScreen(Screen.Storage);
            _buttonPresenter.OnBackButtonClicked += GoBack;
            _buttonPresenter.OnBattleButtonClicked += StartBattle;

            ShowScreen(Screen.Main);
        }

        private void StartBattle()
        {
            _battleController.StartBattle();
        }

        private void GoBack()
        {
            if (_activeScreen != Screen.Main)
            {
                ShowScreen(Screen.Main);
            }
        }

        private void ShowScreen(Screen screen)
        {
            _mainScreen.SetActive(screen == Screen.Main);
            _storageScreen.SetActive(screen == Screen.Storage);

            _activeScreen = screen;
        }
    }
}