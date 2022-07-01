using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    private int _numberOfBasicScrolls;

    private void Awake()
    {
        _numberOfBasicScrolls = StateSaver.Instance.LoadCurrency().BasicScrolls;
    }

    public void UseBasicScroll()
    {
        if (_numberOfBasicScrolls <= 0)
        {
            Debug.LogError("Tried use a basic scroll, but there shouldn't be any scrolls left");
            throw new Exception("No scrolls left, something went wrong");
        }

        _numberOfBasicScrolls--;
        SaveCurrency();
    }

    public int GetNumberOfBasicScrolls()
    {
        return _numberOfBasicScrolls;
    }

    private void SaveCurrency()
    {
        var currencyData = new CurrencyData()
        {
            BasicScrolls = _numberOfBasicScrolls
        };

        StateSaver.Instance.SaveCurrency(currencyData);
    }
}
