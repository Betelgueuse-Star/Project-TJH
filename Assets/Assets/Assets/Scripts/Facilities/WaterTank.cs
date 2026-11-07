using System;
using UnityEngine;

public class WaterTank : MonoBehaviour, IInteractable, IConditionalInteractable
{
    // Dados de cada nível de upgrade do abastecimento.
    // O nível 0 é o padrão com que o jogador começa o jogo.
    [Serializable]
    public struct WaterUpgradeLevel
    {
        [Tooltip("Quantidade de água adicionada ao tanque por segundo")]
        public float fillRatePerSecond;

        public int cost; //custo por upgrade

        // FUTURO (infecção): a água das geleiras carrega bactérias.
        // Quanto maior a qualidade (filtro melhor), menor o risco de doença nas plantas.
        [Range(0f, 1f)] public float quality;
    }

    [SerializeField] private float maxWater = 100f;
    [SerializeField] private float currentWater;

    [Header("Upgrades (índice 0 = nível inicial)")]
    [SerializeField] private WaterUpgradeLevel[] upgradeLevels;
    [SerializeField] private int currentLevel;

    // Disparado quando a água muda: (água atual, água máxima).
    // Quem mostra a informação (texto do tanque, tela do terminal) apenas escuta este evento.
    public event Action<float, float> OnWaterChanged;

    // Disparado quando o nível de upgrade muda (útil para a tela de melhorias do terminal).
    public event Action<int> OnUpgradeLevelChanged;

    public float CurrentWater => currentWater;
    public float MaxWater => maxWater;
    public int CurrentLevel => currentLevel;
    public int MaxLevel => HasLevels ? upgradeLevels.Length - 1 : 0;
    public bool IsMaxLevel => currentLevel >= MaxLevel;

    // FUTURO (infecção): usado pelas plantas/incubadoras para calcular o risco de doença.
    public float WaterQuality => HasLevels ? upgradeLevels[currentLevel].quality : 1f;
    private bool HasLevels => upgradeLevels != null && upgradeLevels.Length > 0;

    private int lastNotifiedWater;

    private void Awake()
    {
        currentWater = maxWater;

        // começa sincronizado com o valor inicial, senão o primeiro frame
        // de enchimento dispararia um evento desnecessário.
        lastNotifiedWater = Mathf.FloorToInt(currentWater);

        if (!HasLevels)
            Debug.LogWarning($"{name}: nenhum nível de upgrade configurado, o tanque não vai reabastecer.", this);
    }

    private void OnValidate()
    {
        maxWater = Mathf.Max(0f, maxWater);
        currentLevel = Mathf.Clamp(currentLevel, 0, MaxLevel);
    }

    private void Update()
    {
        // Tanque cheio ou sem níveis configurados: não faz nada (sem coroutine, sem custo).
        if (currentWater >= maxWater || !HasLevels)
            return;

        // FUTURO (energia): se derreter as geleiras passar a consumir energia do bunker,
        // basta travar o enchimento aqui quando a energia acabar.
        float fillRate = upgradeLevels[currentLevel].fillRatePerSecond;
        currentWater = Mathf.Min(currentWater + fillRate * Time.deltaTime, maxWater);

        // só avisa quando o inteiro mudou OU quando acabou de encher.
        if (Mathf.FloorToInt(currentWater) != lastNotifiedWater || currentWater >= maxWater)
            NotifyWaterChanged();
    }

    public float TakeWater(float amount)
    {
        float waterTaken = Mathf.Min(amount, currentWater);

        if (waterTaken <= 0f)
            return 0f;

        currentWater -= waterTaken;
        NotifyWaterChanged();

        return waterTaken;
    }

    // único lugar que dispara OnWaterChanged.
    private void NotifyWaterChanged()
    {
        lastNotifiedWater = Mathf.FloorToInt(currentWater);
        OnWaterChanged?.Invoke(currentWater, maxWater);
    }

    // Chamado pelo terminal/loja DEPOIS de validar a compra (dinheiro, requisitos etc.).
    // O tanque não sabe nada sobre custo, só aplica o nível.
    public bool SetUpgradeLevel(int level)
    {
        if (!HasLevels || level < 0 || level > MaxLevel)
            return false;

        if (level == currentLevel)
            return true;

        currentLevel = level;
        OnUpgradeLevelChanged?.Invoke(currentLevel);

        return true;
    }

    // Permite ao terminal mostrar o que o próximo nível vai trazer antes de comprar.
    public bool TryGetLevelData(int level, out WaterUpgradeLevel data)
    {
        if (HasLevels && level >= 0 && level <= MaxLevel)
        {
            data = upgradeLevels[level];
            return true;
        }

        data = default;
        return false;
    }

    // devolve os dados do PRÓXIMO nível, ou false se já está no máximo.
    public bool TryGetNextLevelData(out WaterUpgradeLevel data)
    {
        if (IsMaxLevel)
        {
            data = default;
            return false;
        }

        return TryGetLevelData(currentLevel + 1, out data);
    }

    public InteractionType GetInteraction(Player player)
    {
        if (player.CurrentToolType == ToolType.WateringCan)
            return InteractionType.FillWateringCan;

        return InteractionType.None;
    }

    public void Interact(Player player)
    {
        if (GetInteraction(player) != InteractionType.FillWateringCan)
            return;

        if (player.TryGetCurrentTool(out WateringCan wateringCan))
        {
            // FUTURO (animação): wateringCan.PlayUse();
            wateringCan.FillFromTank(this);
        }
    }
}