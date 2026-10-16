using TMPro;
using UnityEngine;

// UI da tela de água do terminal. Segue o mesmo padrão do TerminalOrderUI:
// atualiza ao ser ativada e escuta o evento do tanque enquanto estiver ativa.
public class TerminalWaterUI : MonoBehaviour
{
    [SerializeField] private WaterTank waterTank;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI fillRateText;
    [SerializeField] private TextMeshProUGUI qualityText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;

    private void OnEnable()
    {
        waterTank.OnUpgradeLevelChanged += HandleUpgradeLevelChanged;

        // Cobre qualquer mudança que aconteceu enquanto a tela estava desativada.
        Refresh();
    }

    private void OnDisable()
    {
        waterTank.OnUpgradeLevelChanged -= HandleUpgradeLevelChanged;
    }

    private void HandleUpgradeLevelChanged(int level)
    {
        Refresh();
    }

    private void Refresh()
    {
        levelText.text = $"Level: {waterTank.CurrentLevel + 1}/{waterTank.MaxLevel + 1}";

        if (waterTank.TryGetLevelData(waterTank.CurrentLevel, out WaterTank.WaterUpgradeLevel current))
        {
            fillRateText.text = $"Fill rate: {current.fillRatePerSecond:0.##}/s";
            qualityText.text = $"Quality: {Mathf.RoundToInt(current.quality * 100f)}%";
        }

        if (waterTank.IsMaxLevel)
        {
            upgradeCostText.text = "MAX";
        }
        else if (waterTank.TryGetLevelData(waterTank.CurrentLevel + 1, out WaterTank.WaterUpgradeLevel next))
        {
            upgradeCostText.text = $"Upgrade: {next.cost}";
        }
    }
}