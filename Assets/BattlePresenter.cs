using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePresenter : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Image _playerHealthBar;

    [Header("Enemy")]
    [SerializeField] private Image _enemyHealthBar;

    [Header("Combo Text")]
    [SerializeField] private TMP_Text _comboText;

    private void Start()
    {
        _comboText.text = "";

        _playerHealthBar.rectTransform.localScale = new Vector3(0.5f, 1, 1);
        _enemyHealthBar.rectTransform.localScale = new Vector3(0.2f, 1, 1);
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

    public void SetVictoryText()
    {
        _comboText.text = "Victory!";
    }

    public void SetDefeatText()
    {
        _comboText.text = "Defeat...";
    }

    public void ClearComboText()
    {
        _comboText.text = "";
    }

    public void DisplayCombos(int numberOfCombos)
    {
        if (numberOfCombos > 0)
        {
            if (numberOfCombos == 1)
            {
                _comboText.text = "1 Combo";
            }
            else
            {
                _comboText.text = $"{numberOfCombos} Combos";
            }
        }
        else
        {
            _comboText.text = "";
        }
    }

    
}
