
using UnityEngine;

public class Terminal : MonoBehaviour, IInteractable
{
    [Header("Terminal")]
    [SerializeField] private TerminalModeController terminalModeController;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Shop")]
    [SerializeField] private Transform itemSpawnPoint;

    public void EnterTerminalMode()
    {
        terminalModeController.EnterTerminalMode();
    }

    public void ExitTerminalMode()
    {
        terminalModeController.ExitTerminalMode();
    }


    //metodo geral para comprar qualquer item, contem varias verificações de segurança para evitar erros
    public void BuyItem(ItemTypeSO item)
    {
        if (item == null || itemSpawnPoint == null || currencyManager == null)
        {
            Debug.LogWarning("Missing shop reference.", this);
            return;
        }

        if (item.ItemPrefab == null)
        {
            Debug.LogWarning("The item has no prefab assigned.", this);
            return;
        }

        if (!currencyManager.TrySpendCurrency(item.ItemPrice))
            return;

        Instantiate(
            item.ItemPrefab,
            itemSpawnPoint.position,
            itemSpawnPoint.rotation
        );
    }

    public void Interact(Player player)
    {
        if (player.IsHoldingSomething)
            return;

        EnterTerminalMode();
    }
}