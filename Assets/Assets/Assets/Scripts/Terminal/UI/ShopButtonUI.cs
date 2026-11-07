using TMPro;
using UnityEngine;

// Script para o botão de compra de item na tela do terminal
// Futuramente para estetica, animacao e efeitos sonoros, mas por enquanto apenas compra o item
public class ShopButtonUI : MonoBehaviour
{
    [SerializeField] private ItemTypeSO itemTypeSO;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;

    Terminal terminal;

    private void Awake()
    {
        terminal = GetComponentInParent<Terminal>();

        if (itemTypeSO == null)
        {
            Debug.LogWarning("ItemTypeSO is not assigned in ShopButtonUI.", this);
            return;
        }
        if (priceText == null)
        {
            Debug.LogWarning("Price Text is not assigned in ShopButtonUI.", this);
            return;
        }

        nameText.text = itemTypeSO.ItemName;
        priceText.text = itemTypeSO.ItemPrice.ToString();
    }

    public void BuyItem()
    {
        terminal.BuyItem(itemTypeSO);
    }
}
