using UnityEngine;
using System.Collections;


public class PlantingArea : MonoBehaviour, IInteractable, IConditionalInteractable
{
    [Header("Planting")]
    [SerializeField] private Transform plantingPoint;

    private SeedDataSO plantedSeedData;
    private SoilDataSO soilPlacedData;

    private PlantGrowthState currentState = PlantGrowthState.Empty;
    private GameObject currentPlantVisual;
    private GameObject currentSoilVisual;
    private float currentWater;
    //private float growthTimer;

    private void OnTriggerEnter(Collider other)
    {
        HandleObjectEntered(other);
    }

    private void HandleObjectEntered(Collider other)
    {
        if (other.TryGetComponent(out Seed seed))
        {
            TryPlantSeed(seed);
            return;
        }

        if (other.TryGetComponent(out SoilBag soilBag))
        {
            TryPlaceSoil(soilBag);
            return;
        }
    }

    private void TryPlaceSoil(SoilBag soilBag)
    {
        if (soilPlacedData != null) return; 
        if (!soilBag.TryGetComponent(out ObjectGrabbable grabbable)) return;
        if (grabbable.IsBeingHeld) return; 


        PlaceSoil(soilBag);
    }

    private void TryPlantSeed(Seed seed)
    {
        if (currentState != PlantGrowthState.Empty) return;
        if (soilPlacedData == null) return;
        if (!seed.TryGetComponent(out ObjectGrabbable grabbable)) return; 
        if (grabbable.IsBeingHeld) return; 


        PlantSeed(seed);
    }

    private void PlaceSoil(SoilBag soilBag)
    {
        soilPlacedData = soilBag.SoilData;

        Destroy(soilBag.gameObject);

        if (soilPlacedData.soilPrefab != null)
        {
            currentSoilVisual = Instantiate(
                soilPlacedData.soilPrefab,
                plantingPoint.position,
                plantingPoint.rotation
            );
        }
    }

    private void PlantSeed(Seed seed)
    {
        plantedSeedData = seed.SeedData;

        currentState = PlantGrowthState.Seed;

        Destroy(seed.gameObject);

        UpdatePlantVisual();

        StartCoroutine(GrowPlant());
    }

    private IEnumerator GrowPlant()
    {
        float totalGrowthTime = CalculateGrowthTime();
        float stageTime = totalGrowthTime / 3f;

        // Seed
        yield return new WaitForSeconds(stageTime);

        yield return WaitForCorrectWater();

        if (currentState == PlantGrowthState.Empty)
            yield break;

        ChangeState(PlantGrowthState.Germinating);
        currentWater = 0f;


        // Germinating
        yield return new WaitForSeconds(stageTime);

        yield return WaitForCorrectWater();

        if (currentState == PlantGrowthState.Empty)
            yield break;

        ChangeState(PlantGrowthState.Growing);
        currentWater = 0f;


        // Growing
        yield return new WaitForSeconds(stageTime);

        yield return WaitForCorrectWater();

        if (currentState == PlantGrowthState.Empty)
            yield break;

        ChangeState(PlantGrowthState.Ready);
        currentWater = 0f;
    }

    private float CalculateGrowthTime()
    {
        float multiplier = 1f;

        // Check if soilPlacedData is not null and matches the recommended soil for the planted seed
        if (soilPlacedData != null && soilPlacedData == plantedSeedData.recommendedSoil) {

            multiplier = soilPlacedData.growthMultiplier;
        }

        return plantedSeedData.baseGrowthTime / multiplier;
    }

    private void ChangeState(PlantGrowthState newState)
    {
        currentState = newState;
        UpdatePlantVisual();
    }

    private void UpdatePlantVisual()
    {
        if (currentPlantVisual != null)
        {
            Destroy(currentPlantVisual);
        }

        GameObject prefabToSpawn = null;

        switch (currentState)
        {
            case PlantGrowthState.Seed:
                prefabToSpawn = plantedSeedData.seedPrefab;
                break;

            case PlantGrowthState.Germinating:
                prefabToSpawn = plantedSeedData.germinatingPrefab;
                break;

            case PlantGrowthState.Growing:
                prefabToSpawn = plantedSeedData.growingPrefab;
                break;

            case PlantGrowthState.Ready:
                prefabToSpawn = plantedSeedData.readyPrefab;
                break;
        }

        if (prefabToSpawn != null)
        {
            currentPlantVisual = Instantiate(
                prefabToSpawn,
                plantingPoint.position,
                plantingPoint.rotation
            );

            if (currentState == PlantGrowthState.Ready)
            {
                if (currentPlantVisual.TryGetComponent(out Sapling sapling))
                {
                    sapling.Setup(plantedSeedData, this);
                }
            }
        }
    }

    private IEnumerator WaitForCorrectWater()
    {
        while (CheckWater() == WaterStatus.TooLittle)//esperar até que a planta receba a quantidade correta de água
        {
            yield return null;
        }

        if (CheckWater() == WaterStatus.TooMuch)//se ultrapassar a quantidade de água necessária, a planta morre
        {
            KillPlant();
            yield break;
        }
    }

    private WaterStatus CheckWater()
    {
        float requiredWater = GetRequiredWater();
        float tolerance = plantedSeedData.waterTolerance;

        if (currentWater < requiredWater - tolerance)
            return WaterStatus.TooLittle;

        if (currentWater > requiredWater + tolerance)
            return WaterStatus.TooMuch;

        return WaterStatus.Correct;
    }

    private float GetRequiredWater()
    {
        switch (currentState)
        {
            case PlantGrowthState.Seed:
                return plantedSeedData.seedRequiredWater;

            case PlantGrowthState.Germinating:
                return plantedSeedData.germinatingRequiredWater;

            case PlantGrowthState.Growing:
                return plantedSeedData.growingRequiredWater;

            default:
                return 0f;
        }
    }

    private void KillPlant()
    {
        StopAllCoroutines();

        if (currentPlantVisual != null)
        {
            Destroy(currentPlantVisual);
            currentPlantVisual = null;
        }

        plantedSeedData = null;
        currentWater = 0f;
        currentState = PlantGrowthState.Empty;

        Debug.Log("A planta morreu.");
    }

    public void RemoveGrowPlant()
    {
        currentPlantVisual = null;

        plantedSeedData = null;

        currentState = PlantGrowthState.Empty;

        Debug.Log("Planta removida. Área disponível novamente.");
    }

    //remove tudo
    public void RemoveEverything(Player player)
    {
        if (soilPlacedData == null)
            return;

        if (player.CurrentTool == null)
            return;

        if (!player.CurrentTool.TryGetComponent(out Shovel shovel))
            return;

        StopAllCoroutines();

        // Remover planta, se existir
        if (currentPlantVisual != null)
        {
            Destroy(currentPlantVisual);
            currentPlantVisual = null;
        }

        plantedSeedData = null;
        currentState = PlantGrowthState.Empty;

        // Remover solo
        if (currentSoilVisual != null)
        {
            Destroy(currentSoilVisual);
            currentSoilVisual = null;
        }

        soilPlacedData = null;
    }

    //segura um regador/2 ou pá/1 
    public (bool, float) CanInteract(Player player)
    {
        if (player.CurrentTool == null)
            return (false, 0f);

        if (player.CurrentTool.TryGetComponent(out Shovel shovel))
        {
            if (soilPlacedData == null) return (false, 0f);

            else return (true, 1f);
        }

        if (player.CurrentTool.TryGetComponent(out WateringCan wateringCan))
        {
            if (plantedSeedData == null) return (false, 0f);

            if (wateringCan.CurrentWater <= 0f)
                return (false, 0f);

            else return (true, 2f);
        }

        else return (false, 0f);
    }

    public void Interact(Player player)
    {
        var (canInteract, interactionValue) = CanInteract(player);

        if (!canInteract)
            return;

        if (interactionValue == 1f)
        {
            KillPlant();
            RemoveEverything(player);
        }
        
        if (interactionValue == 2f)
        {
            if (player.CurrentTool.TryGetComponent(out WateringCan wateringCan))
            {
                AddPlantWater(wateringCan);
            }
        }
    }


    public void AddPlantWater(WateringCan wateringCan)
    {
        currentWater += wateringCan.GetAndRemoveWater(wateringCan.WaterAmount);
        Debug.Log($"Água adicionada à planta. Água atual: {currentWater}");
    }
}