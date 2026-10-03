
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    private int currentCurrency = 0;

    public int CurrentCurrency => currentCurrency;

    public void AddCurrency(int amount)
    {
        if (amount <= 0)
            return;

        currentCurrency += amount;

        Debug.Log($"Currency added: {amount}. Current currency: {currentCurrency}");
    }

    public bool TrySpendCurrency(int amount)
    {
        if (amount <= 0 || currentCurrency < amount)
            return false;

        currentCurrency -= amount;

        Debug.Log($"Currency spent: {amount}. Current currency: {currentCurrency}");

        return true;
    }
}