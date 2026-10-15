using UnityEngine;

[RequireComponent(typeof(PlantingAreaVisual))]
public class PlantingArea : MonoBehaviour, IInteractable, IConditionalInteractable
{
    public enum PlantingInteraction
    {
        None = 0,
        RemoveAll = 1, // pá
        Water = 2      // regador
    }
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
    // decidir se podem ser aplicados, sem expor os campos privados.
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

    // MUDANÇA: antes só o OnTriggerEnter tentava aplicar o item. Como agora o item
    // só é aplicado quando NÃO está na mão, quem solta dentro da área precisa de uma
    // nova tentativa. O OnTriggerStay repete a checagem enquanto o item estiver lá.
    // Dica: coloque os itens em uma layer própria e configure a matriz de colisão
    // (Project Settings > Physics) para a área só enxergar essa layer.
    private void OnTriggerStay(Collider other)
    {
        HandleObjectEntered(other);
    }

    // MUDANÇA: antes eram três TryGetComponent (Seed, SoilBag, Fertilizer) e três
    // métodos Try... quase iguais. Agora cada item implementa IAreaItem e decide
    // sozinho se pode ser aplicado. A PlantingArea não precisa mais conhecer cada
    // tipo de item: item novo = classe nova, sem mexer aqui.
    private void HandleObjectEntered(Collider other)
    {
        // MUDANÇA: a checagem do ObjectGrabbable, que antes se repetia nos três
        // métodos (e estava sem uso, com o IsBeingHeld comentado), agora é feita
        // uma vez só. Item na mão não é aplicado.
        if (!other.TryGetComponent(out ObjectGrabbable grabbable)) return;
        if (grabbable.IsBeingHeld) return;

        if (other.TryGetComponent(out IAreaItem item) && item.CanApply(this))
        {
            item.Apply(this);
        }
    }

    // MUDANÇA: os três métodos abaixo agora são públicos e recebem o dado (SO) em vez
    // do objeto. Quem destrói o objeto é o próprio item, dentro do IAreaItem.Apply.
    public void PlaceSoil(SoilDataSO soilData)
    {
        soilPlacedData = soilData;

        plantingAVisual.UpdateSoilVisual(soilPlacedData);
    }

    public void PlantSeed(SeedDataSO seedData)
    {
        plantedSeedData = seedData;

        // MUDANÇA: o tempo de cada estágio é calculado uma única vez, aqui.
        stageDuration = CalculateGrowthTime() / GrowthStageCount;
        waitingForWater = false;

        // MUDANÇA: ChangeState já atualiza o visual da planta E o texto do estágio.
        // Antes o texto só era atualizado ao chegar em Germinating (ficava "Empty"
        // enquanto era semente).
        ChangeState(PlantGrowthState.Seed);

        StartStageTimer();
    }

    public void PlaceFertilizer(FertilizerDataSO fertilizerData)
    {
        fertilizerPlacedData = fertilizerData;

        plantingAVisual.PlayFertilizerEffect(fertilizerPlacedData);
    }

    // MUDANÇA: no lugar da coroutine GrowPlant. Agenda UMA chamada para o fim do
    // estágio. Não existe Update nem loop: entre um estágio e outro, a planta não
    // gasta nada por frame. O Invoke do Unity respeita o Time.timeScale (como o
    // WaitForSeconds) e não aloca memória.
    private void StartStageTimer()
    {
        Invoke(nameof(OnStageTimeFinished), stageDuration);
    }

    // Chamado pelo Invoke quando o tempo do estágio termina.
    private void OnStageTimeFinished()
    {
        waitingForWater = true;
        EvaluateStage();
    }

    // MUDANÇA: junta o que antes era WaitForCorrectWater + o "ChangeState + ResetPlantWater"
    // repetido três vezes na coroutine. É chamado quando o tempo acaba e também
    // sempre que a água muda (AddPlantWater), em vez de ficar checando todo frame.
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

    // MUDANÇA: um único método para avançar de estágio, no lugar das três cópias
    // (Germinating, Growing, Ready) que existiam dentro da coroutine.
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
        // CancelInvoke no lugar de StopAllCoroutines. Cancela só o timer
        // do estágio, sem afetar mais nada do objeto.
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

    public void RemoveGrowPlant()
    {
        // Solta a referência do visual SEM destruir (a planta colhida cuida de si mesma).
        // Precisa vir antes do KillPlant, que destruiria o visual.
        plantingAVisual.RemovePlantVisual();

        KillPlant();

        Debug.Log("Planta removida. Área disponível novamente.");
    }

    // MUDANÇA: não recebe mais o Player. As checagens de "tem solo / está com a pá"
    // agora ficam no GetInteraction, então aqui só resta remover.
    public void RemoveEverything()
    {
        KillPlant();

        // Remover visual, e atualizar textos
        plantingAVisual.RemoveSoilVisual();
        soilPlacedData = null;
    }

    // MUDANÇA: toda a lógica de "o que dá para fazer com a ferramenta na mão" ficou
    // aqui. Antes os TryGetComponent de Shovel e WateringCan se repetiam em
    // CanInteract, RemoveEverything e Interact.
    public PlantingInteraction GetInteraction(Player player)
    {
        GameObject tool = player.CurrentTool;

        if (tool == null)
            return PlantingInteraction.None;

        // pá: precisa ter solo
        if (HasSoil && tool.TryGetComponent(out Shovel _))
            return PlantingInteraction.RemoveAll;

        // regador: precisa de planta, e o regador precisa ter água
        if (plantedSeedData != null &&
            currentState != PlantGrowthState.Ready &&
            tool.TryGetComponent(out WateringCan wateringCan) &&
            wateringCan.CurrentWater > 0f)
        {
            return PlantingInteraction.Water;
        }

        return PlantingInteraction.None;
    }

    //segura um regador/2 ou pá/1 
    public (bool, float) CanInteract(Player player)
    {
        PlantingInteraction interaction = GetInteraction(player);

        return (interaction != PlantingInteraction.None, (float)interaction);
    }

    public void Interact(Player player)
    {
        switch (GetInteraction(player))
        {
            case PlantingInteraction.RemoveAll:
                RemoveEverything();
                break;

            case PlantingInteraction.Water:
                // O GetInteraction já garantiu que a ferramenta é um regador.
                AddPlantWater(player.CurrentTool.GetComponent<WateringCan>());
                break;
        }
    }

    public void AddPlantWater(WateringCan wateringCan)
    {
        currentWater += wateringCan.GetAndRemoveWater(wateringCan.WaterAmount);

        plantingAVisual.UpdateWaterText(currentWater);
        Debug.Log($"Água adicionada à planta. Água atual: {currentWater}");

        // É aqui que reavaliamos se a planta que
        // estava esperando já pode avançar (ou morrer por excesso de água).
        // Não precisa mais de loop checando todo frame.
        if (waitingForWater)
            EvaluateStage();
    }
}