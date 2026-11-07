using UnityEngine;

[RequireComponent(typeof(PlantingAreaVisual))]
public class PlantingArea : MonoBehaviour, IInteractable, IConditionalInteractable
{
    private const int GrowthStageCount = 3; //quantidade de estágios com espera (Seed, Germinating e Growing).

    private PlantingAreaVisual plantingAVisual;
    private SeedDataSO plantedSeedData;
    private SoilDataSO soilPlacedData;
    private FertilizerDataSO fertilizerPlacedData;
    private PlantGrowthState currentState = PlantGrowthState.Empty;

    private float currentWater;

    private float stageDuration;
    private bool waitingForWater;

    // propriedades de leitura usadas pelos itens (IAreaItem) para
    // decidir se podem ser aplicados
    public bool HasSoil => soilPlacedData != null;
    public bool IsEmpty => currentState == PlantGrowthState.Empty;
    public bool HasFertilizer => fertilizerPlacedData != null;

    private void Awake()
    {
        plantingAVisual = GetComponent<PlantingAreaVisual>();

        plantingAVisual.UpdateWaterText(currentWater);
        plantingAVisual.UpdateGrowthStageText(currentState);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleObjectEntered(other);
    }

    // O OnTriggerStay repete a checagem
    // Depois : colocar os itens em uma layer própria e configure a matriz de colisão
    // (Project Settings > Physics) para a área só enxergar essa layer
    private void OnTriggerStay(Collider other)
    {
        HandleObjectEntered(other);
    }

    // cada item implementa IAreaItem e decide
    // sozinho se pode ser aplicado
    private void HandleObjectEntered(Collider other)
    {
        if (!other.TryGetComponent(out ObjectGrabbable grabbable)) return;

        if (grabbable.IsBeingHeld) return;

        if (other.TryGetComponent(out IAreaItem item) && item.CanApply(this))
        {
            item.Apply(this);
        }
    }

    public void PlaceSoil(SoilDataSO soilData)
    {
        soilPlacedData = soilData;

        plantingAVisual.UpdateSoilVisual(soilPlacedData);
    }

    public void PlantSeed(SeedDataSO seedData)
    {
        plantedSeedData = seedData;

        // Tempo de cada estágio é calculado uma única vez aqui
        stageDuration = CalculateGrowthTime() / GrowthStageCount;
        waitingForWater = false;

        ChangeState(PlantGrowthState.Seed);

        StartStageTimer();
    }

    public void PlaceFertilizer(FertilizerDataSO fertilizerData)
    {
        fertilizerPlacedData = fertilizerData;

        plantingAVisual.PlayFertilizerEffect(fertilizerPlacedData);
    }

    // Antigamente usava Update, depois Coutine, agora usa Invoke para chamar OnStageTimeFinished depois de stageDuration segundos.
    private void StartStageTimer()
    {
        Invoke(nameof(OnStageTimeFinished), stageDuration);
    }

    // Chamado pelo Invoke quando o tempo do estágio termina
    private void OnStageTimeFinished()
    {
        waitingForWater = true;
        EvaluateStage();
    }

    // É chamado quando o tempo acaba e também
    // sempre que a água muda (AddPlantWater)
    private void EvaluateStage()
    {
        switch (CheckWater())
        {
            case WaterStatus.TooLittle:
                //esperar até que a planta receba a quantidade correta de água
                // (o AddPlantWater chama EvaluateStage de novo quando chegar mais água)
                return;

            case WaterStatus.TooMuch:
                //se ultrapassar a quantidade de água necessária, a planta morre
                KillPlant();
                return;

            default:
                AdvanceStage();
                return;
        }
    }

    // Avança para o próximo estágio, reseta a água e reinicia o timer
    private void AdvanceStage()
    {
        waitingForWater = false;

        ChangeState(GetNextState(currentState));
        ResetPlantWater();

        if (currentState != PlantGrowthState.Ready)
            StartStageTimer();
    }

    private PlantGrowthState GetNextState(PlantGrowthState state)
    {
        switch (state)
        {
            // Seed
            case PlantGrowthState.Seed:
                return PlantGrowthState.Germinating;

            // Germinating
            case PlantGrowthState.Germinating:
                return PlantGrowthState.Growing;

            // Growing
            case PlantGrowthState.Growing:
                return PlantGrowthState.Ready;

            default:
                return state;
        }
    }

    private float CalculateGrowthTime()
    {
        float soilMultiplier = 1f;
        float fertilizerMultiplier = 1f;

        if (soilPlacedData != null &&
            soilPlacedData == plantedSeedData.recommendedSoil)
        {
            soilMultiplier = Mathf.Max(1f, soilPlacedData.growthMultiplier);
        }

        if (fertilizerPlacedData != null)
        {
            fertilizerMultiplier = Mathf.Max(1f, fertilizerPlacedData.growthMultiplier);
        }

        float multiplier = soilMultiplier * fertilizerMultiplier;
        float totalTime = plantedSeedData.baseGrowthTime / multiplier;

        Debug.Log(
            $"[Growth] Base: {plantedSeedData.baseGrowthTime}s | " +
            $"Soil: {soilMultiplier}x | " +
            $"Fertilizer: {fertilizerMultiplier}x | " +
            $"Final: {totalTime}s | " +
            $"Per stage: {totalTime / 3f}s"
        );

        return totalTime;
    }

    private void ChangeState(PlantGrowthState newState)
    {
        currentState = newState;

        plantingAVisual.UpdatePlantVisual(
            currentState,
            plantedSeedData,
            this
        );

        plantingAVisual.UpdateGrowthStageText(currentState);
    }

    private WaterStatus CheckWater()
    {
        float requiredWater = plantedSeedData.GetRequiredWater(currentState);
        float tolerance = plantedSeedData.waterTolerance;

        if (currentWater < requiredWater - tolerance)
            return WaterStatus.TooLittle;

        if (currentWater > requiredWater + tolerance)
            return WaterStatus.TooMuch;

        return WaterStatus.Correct;
    }

    private void KillPlant()
    {
        CancelInvoke(nameof(OnStageTimeFinished));
        waitingForWater = false;

        plantingAVisual.DestroyPlant();
        plantingAVisual.DestroyFertilizerEffect();

        fertilizerPlacedData = null;
        plantedSeedData = null;
        currentState = PlantGrowthState.Empty;

        ResetPlantWater();
        plantingAVisual.UpdateGrowthStageText(currentState);
    }

    private void ResetPlantWater()
    {
        currentWater = 0f;
        plantingAVisual.UpdateWaterText(currentWater);
    }

    // Por enquanto esse metodo é apenas chamado com o jogador interagindo com a sampling crecida 
    public void RemoveGrowPlant()
    {
        // Precisa vir antes do KillPlant, que destruiria o visual.
        plantingAVisual.RemovePlantVisual();

        KillPlant();

        Debug.Log("Planta removida. Área disponível novamente.");
    }

    public void RemoveEverything()
    {
        KillPlant();

        // Remover visual, e atualizar textos
        plantingAVisual.RemoveSoilVisual();
        soilPlacedData = null;
    }

    //segura um regador ou pá
    public InteractionType GetInteraction(Player player)
    {
        switch (player.CurrentToolType)
        {
            // pá: precisa ter solo
            case ToolType.Shovel:
                return HasSoil ? InteractionType.RemoveAll : InteractionType.None;

            // regador: precisa de planta, e o regador precisa ter água
            case ToolType.WateringCan:
                if (plantedSeedData != null &&
                    currentState != PlantGrowthState.Ready &&
                    player.TryGetCurrentTool(out WateringCan wateringCan) &&
                    wateringCan.CurrentWater > 0f)
                {
                    return InteractionType.Water;
                }
                return InteractionType.None;

            default:
                return InteractionType.None;
        }
    }

    public void Interact(Player player)
    {
        switch (GetInteraction(player))
        {
            case InteractionType.RemoveAll:
                // FUTURO (animação): player.CurrentTool.PlayUse();
                RemoveEverything();
                break;

            case InteractionType.Water:
                // O GetInteraction já garantiu que a ferramenta é um regador.
                if (player.TryGetCurrentTool(out WateringCan wateringCan))
                {
                    // FUTURO (animação): wateringCan.PlayUse();
                    AddPlantWater(wateringCan);
                }
                break;
        }
    }
    public void AddPlantWater(WateringCan wateringCan)
    {
        currentWater += wateringCan.GetAndRemoveWater(wateringCan.WaterAmount);

        plantingAVisual.UpdateWaterText(currentWater);
        Debug.Log($"Água adicionada à planta. Água atual: {currentWater}");

        if (waitingForWater) // Checa se a planta estava esperando por água, e se sim, reavalia o estágio
            EvaluateStage();
    }
}