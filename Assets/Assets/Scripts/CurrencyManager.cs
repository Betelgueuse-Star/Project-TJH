
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] TerminalShopUI terminalShopUI;
    private int currentCurrency = 0;

    public int CurrentCurrency => currentCurrency;

    public void AddCurrency(int amount)
    {
        if (amount <= 0)
            return;

        currentCurrency += amount;
        terminalShopUI.UpdateCurrencyText(currentCurrency);
    }

    public bool TrySpendCurrency(int amount)
    {
        if (amount <= 0 || currentCurrency < amount)
            return false;

        currentCurrency -= amount;
        terminalShopUI.UpdateCurrencyText(currentCurrency);

        return true;
    }
}