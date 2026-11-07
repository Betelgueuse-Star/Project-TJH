using System.Text;
using TMPro;
using UnityEngine;

// UI do carrinho no terminal: mostra os itens comprados que ainda esperam entrega
public class TerminalCartUI : MonoBehaviour
{
    [SerializeField] private ShoppingCart shoppingCart;

    [Header("Texto")]
    [SerializeField] private TextMeshProUGUI cartText;
    [SerializeField] private string emptyText = "Cart is empty";

    // Reaproveitado a cada atualização para não criar uma string nova por item.
    private readonly StringBuilder builder = new StringBuilder();

    private bool isConfigured;

    private void Awake()
    {
        isConfigured = shoppingCart != null;

        if (!isConfigured)
            Debug.LogError($"{name}: ShoppingCart faltando, a tela do carrinho não vai funcionar.", this);

        if (cartText == null)
            Debug.LogWarning($"{name}: cartText faltando, o carrinho não vai aparecer na tela.", this);
    }

    private void OnEnable()
    {
        if (!isConfigured)
            return;

        shoppingCart.OnCartChanged += RefreshText;

        // Cobre qualquer mudança que aconteceu enquanto a tela estava desativada
        // (ex.: itens entregues enquanto o jogador estava em outra aba).
        RefreshText();
    }

    private void OnDisable()
    {
        if (!isConfigured)
            return;

        shoppingCart.OnCartChanged -= RefreshText;
    }

    // Monta o texto a partir dos dados do carrinho: uma linha por tipo de item.
    // Ex.: "2x Seed" e "1x Shovel".
    private void RefreshText()
    {
        if (cartText == null)
            return;

        builder.Clear();

        foreach (CartEntry entry in shoppingCart.GetPendingEntries())
        {
            builder.AppendLine($"{entry.Amount}x {entry.Item.ItemName}");
        }

        cartText.text = builder.Length > 0 ? builder.ToString() : emptyText;
    }
}