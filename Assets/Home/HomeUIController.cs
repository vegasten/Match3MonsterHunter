using System;
using UnityEngine;

namespace Home
{
    public class HomeUIController : MonoBehaviour
    {
        public event Action OnBack;

        private enum Canvases
        {
            Main,
            Summoning
        }

        private enum MainScreens
        {
            Main,
            Storage,
            External
        };

        private enum SummoningScreens
        {
            Summoning
        }

        private enum CamType
        {
            Main,
            Summoning
        };

        [Header("Presenters")]
        [SerializeField] private HomeButtonPresenter _buttonPresenter;

        [Header("Controllers")]
        [SerializeField] private HomeBattleController _battleController;

        [Header("Cameras")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _summoningCamera;

        [Header("Canvas")]
        [SerializeField] private GameObject _mainCanvas;
        [SerializeField] private GameObject _summoningCanvas;

        [Header("Screens")]
        [SerializeField] private GameObject _mainScreen;
        [SerializeField] private GameObject _storageScreen;
        [SerializeField] private GameObject _summoningScreen;

        private CamType _activeCamera;
        private Canvases _activeCanvas;
        private MainScreens _activeScreen = MainScreens.Main;

        private void Start()
        {
            _buttonPresenter.OnStorageButtonClicked += () => ShowScreen(MainScreens.Storage);
            _buttonPresenter.OnBackButtonClicked += GoBack;
            _buttonPresenter.OnBattleButtonClicked += StartBattle;
            _buttonPresenter.OnSummonButtonClicked += SummonMonster;

            SetCamera(CamType.Main);
            ShowCanvas(Canvases.Main);
            ShowScreen(MainScreens.Main);
        }

        private void StartBattle()
        {
            _battleController.StartBattle();
        }

        private void GoBack()
        {
            if (_activeCamera != CamType.Main)
            {
                SetCamera(CamType.Main);
            }

            if (_activeCanvas != Canvases.Main)
            {
                ShowCanvas(Canvases.Main);
            }

            if (_activeScreen != MainScreens.Main)
            {
                ShowScreen(MainScreens.Main);
            }

            OnBack?.Invoke();
        }

        private void SummonMonster()
        {
            SetCamera(CamType.Summoning);
            ShowCanvas(Canvases.Summoning);
            ShowScreen(SummoningScreens.Summoning);
        }

        private void SetCamera(CamType camType)
        {
            _mainCamera.gameObject.SetActive(camType == CamType.Main);
            _summoningCamera.gameObject.SetActive(camType == CamType.Summoning);

            _activeCamera = camType;
        }

        private void ShowCanvas(Canvases canvas)
        {
            _mainCanvas.SetActive(canvas == Canvases.Main);
            _summoningCanvas.SetActive(canvas == Canvases.Summoning);

            _activeCanvas = canvas;
        }

        private void ShowScreen(MainScreens screen)
        {
            _mainScreen.SetActive(screen == MainScreens.Main);
            _storageScreen.SetActive(screen == MainScreens.Storage);

            _activeScreen = screen;
        }

        private void ShowScreen(SummoningScreens screen)
        {
            _summoningScreen.SetActive(screen == SummoningScreens.Summoning);

            _activeScreen = MainScreens.External;
        }
    }
}