using UnityEngine;

public class Terminal : MonoBehaviour, IInteractable
{
    [Header("Terminal")]
    [SerializeField] private TerminalModeController terminalModeController;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Shop")]
    [SerializeField] private Transform itemSpawnPoint;

    [Header("Water")]
    [SerializeField] private WaterTank waterTank;

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

    // Compra o próximo nível de upgrade do abastecimento de água.
    // Chamado pelo TerminalButton da tela de água (UnityEvent onClick).
    public void BuyWaterUpgrade()
    {
        if (waterTank == null || currencyManager == null)
        {
            Debug.LogWarning("Missing water upgrade reference.", this);
            return;
        }

        if (waterTank.IsMaxLevel)
            return;

        int nextLevel = waterTank.CurrentLevel + 1;

        if (!waterTank.TryGetLevelData(nextLevel, out WaterTank.WaterUpgradeLevel nextLevelData))
            return;

        if (!currencyManager.TrySpendCurrency(nextLevelData.cost))
        {
            // FUTURO: feedback de "dinheiro insuficiente" (som, mensagem no monitor).
            return;
        }

        waterTank.SetUpgradeLevel(nextLevel);
    }

    public void Interact(Player player)
    {
        if (player.IsHoldingSomething)
            return;

        EnterTerminalMode();
    }
}