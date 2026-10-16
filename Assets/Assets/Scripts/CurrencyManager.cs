using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    private int currentCurrency = 0;

    public int CurrentCurrency => currentCurrency;

    // Disparado sempre que o dinheiro muda (ganho ou gasto), com o valor atual.
    // Qualquer tela que mostre dinheiro (loja, água, etc.) só precisa escutar este evento.
    public event Action<int> OnCurrencyChanged;

    public void AddCurrency(int amount)
    {
        if (amount <= 0)
            return;

        currentCurrency += amount;
        OnCurrencyChanged?.Invoke(currentCurrency);
    }

    public bool TrySpendCurrency(int amount)
    {
        if (amount <= 0 || currentCurrency < amount)
            return false;

        currentCurrency -= amount;
        OnCurrencyChanged?.Invoke(currentCurrency);

        return true;
    }
}