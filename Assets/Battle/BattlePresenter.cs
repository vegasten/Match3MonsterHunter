using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Battle
{
    public class BattlePresenter : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Image _playerHealthBar;
        [SerializeField] private Image _playerAttackerIndicator;

        [Header("Enemy")]
        [SerializeField] private Image _enemyHealthBar;
        [SerializeField] private Image _enemyAttackerIndicator;

        [Header("Combo Text")]
        [SerializeField] private TMP_Text _comboText;

        [Header("End battle modal")]
        [SerializeField] private GameObject _endBattleModal;
        [SerializeField] private GameObject _victoryScreen;
        [SerializeField] private GameObject _defeatScreen;
        [SerializeField] private Button _goBackVictoryButton;
        [SerializeField] private Button _goBackDefeatButton;
        [SerializeField] private TMP_Text _lootText;

        [Header("MenuButtons")]
        [SerializeField] private Button _goBackButton;
        [SerializeField] private Button _confirmCancelButton;
        [SerializeField] private Button _dontCancelButton;
        [SerializeField] private GameObject _cancelBattleScreen;

        private void Start()
        {
            _comboText.text = "";

            _playerHealthBar.rectTransform.localScale = new Vector3(0.5f, 1, 1);
            _enemyHealthBar.rectTransform.localScale = new Vector3(0.2f, 1, 1);

            _goBackVictoryButton.onClick.AddListener(GoBackAfterEndBattle);
            _goBackDefeatButton.onClick.AddListener(GoBackAfterEndBattle);

            _goBackButton.onClick.AddListener(CancelBattle);
            _confirmCancelButton.onClick.AddListener(DoCancelBattle);
            _dontCancelButton.onClick.AddListener(DontCancelBattle);

            _victoryScreen.SetActive(false);
            _defeatScreen.SetActive(false);
            _endBattleModal.SetActive(false);
        }

        private void GoBackAfterEndBattle()
        {
            SceneManager.LoadScene("HomeScene");
        }

        public void SetHealthBar(float percent, Players player)
        {
            if (percent < 0)
            {
                percent = 0;
            }
            else if (percent > 1)
            {
                percent = 1;
            }

            switch (player)
            {
                case Players.Friendly:
                    _playerHealthBar.rectTransform.localScale = new Vector3(percent, 1, 1);
                    break;
                case Players.Enemy:
                    _enemyHealthBar.rectTransform.localScale = new Vector3(percent, 1, 1);
                    break;
                default:
                    Debug.LogError("Trying to change health bar on unknown player.");
                    break;
            }
        }

        public void EndBattleWithVictory(string loot = "")
        {
            _comboText.text = "";
            _endBattleModal.SetActive(true);
            _victoryScreen.SetActive(true);

            if (!string.IsNullOrEmpty(loot))
            {
                _lootText.text = $"Found loot:";
            }
            else
            {
                _lootText.text = "";
            }
        }

        public void EndBattleWithDefeat()
        {
            _comboText.text = "";
            _endBattleModal.SetActive(true);
            _defeatScreen.SetActive(true);
        }
        private void CancelBattle()
        {
            _endBattleModal.SetActive(true);
            _cancelBattleScreen.SetActive(true);
        }

        private void DoCancelBattle()
        {
            SceneManager.LoadScene("HomeScene");
        }

        private void DontCancelBattle()
        {
            _cancelBattleScreen.SetActive(false);
            _endBattleModal.SetActive(false);
        }


        public void ClearComboText()
        {
            _comboText.text = "";
        }

        public void DisplayCombos(int numberOfCombos)
        {
            if (numberOfCombos > 0)
            {
                _comboText.transform.localScale = Vector3.zero;

                var targetScale = 1 + numberOfCombos / 10.0f;
                _comboText.transform.DOScale(targetScale, 0.5f).SetEase(Ease.OutBounce);                

                _comboText.text = $"{numberOfCombos} Combo";
            }
            else
            {
                _comboText.text = "";
            }
        }

        public void DisplayActiveAttacer(Players player)
        {
            if (player == Players.Friendly)
            {
                _playerAttackerIndicator.gameObject.SetActive(true);
                _enemyAttackerIndicator.gameObject.SetActive(false);
            }
            else
            {
                _playerAttackerIndicator.gameObject.SetActive(false);
                _enemyAttackerIndicator.gameObject.SetActive(true);
            }
        }
    }
}