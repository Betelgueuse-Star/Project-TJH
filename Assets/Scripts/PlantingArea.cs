using UnityEngine;
using System.Collections;


public class PlantingArea : MonoBehaviour
{
    [Header("Planting")]
    [SerializeField] private Transform plantingPoint;

    private SeedDataSO plantedSeedData;
    private SoilDataSO soilPlacedData;

    private PlantGrowthState currentState = PlantGrowthState.Empty;
    private GameObject currentPlantVisual;
    private GameObject currentSoilVisual;

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

        yield return new WaitForSeconds(stageTime);

        ChangeState(PlantGrowthState.Germinating);

        yield return new WaitForSeconds(stageTime);

        ChangeState(PlantGrowthState.Growing);

        yield return new WaitForSeconds(stageTime);

        ChangeState(PlantGrowthState.Ready);
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

    public void RemovePlant()
    {
        currentPlantVisual = null;

        plantedSeedData = null;

        currentState = PlantGrowthState.Empty;

        Debug.Log("Planta removida. Área disponível novamente.");
    }
}