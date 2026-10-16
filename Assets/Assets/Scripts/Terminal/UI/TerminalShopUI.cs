using TMPro;
using UnityEngine;

public class TerminalShopUI : MonoBehaviour
{
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private TextMeshProUGUI currencyText;

    private void OnEnable()
    {
        if (currencyManager == null)
            return;

        currencyManager.OnCurrencyChanged += UpdateCurrencyText;

        // Cobre qualquer mudança que aconteceu enquanto a tela estava desativada.
        UpdateCurrencyText(currencyManager.CurrentCurrency);
    }

    private void OnDisable()
    {
        if (currencyManager == null)
            return;

        currencyManager.OnCurrencyChanged -= UpdateCurrencyText;
    }

    private void UpdateCurrencyText(int amount)
    {
        currencyText.text = $"Currency: {amount}";
    }
}