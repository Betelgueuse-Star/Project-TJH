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

    private bool isConfigured;

    //valida as referências UMA vez, aqui, em vez de checar em cada uso
    private void Awake()
    {
        isConfigured =
            waterTank != null &&
            levelText != null &&
            fillRateText != null &&
            qualityText != null &&
            upgradeCostText != null;

        if (!isConfigured)
            Debug.LogError($"{name}: referência faltando na tela de água, ela não vai funcionar.", this);
    }

    private void OnEnable()
    {
        if (!isConfigured)
            return;

        waterTank.OnUpgradeLevelChanged += HandleUpgradeLevelChanged;

        // Cobre qualquer mudança que aconteceu enquanto a tela estava desativada.
        Refresh();
    }

    private void OnDisable()
    {
        if (!isConfigured)
            return;

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

        // usa TryGetNextLevelData em vez de IsMaxLevel + CurrentLevel + 1.
        if (waterTank.TryGetNextLevelData(out WaterTank.WaterUpgradeLevel next))
        {
            upgradeCostText.text = $"Upgrade: {next.cost}";
        }
        else
        {
            upgradeCostText.text = "MAX";
        }
    }
}