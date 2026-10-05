using TMPro;
using UnityEngine;

public class TerminalShopUI : MonoBehaviour
{
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private TextMeshProUGUI currencyText;

    private void Start()
    {
        UpdateCurrencyText(currencyManager.CurrentCurrency);
    }

    public void UpdateCurrencyText(int amount)
    {
        currencyText.text = $"Currency: {amount}";
    }
}
