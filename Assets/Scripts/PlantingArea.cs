using UnityEngine;

public class PlantingArea : MonoBehaviour
{
    [Header("Planting")]
    [SerializeField] private Transform plantingPoint;

    private SeedDataSO plantedSeedData;
    private PlantGrowthState currentState = PlantGrowthState.Empty;

    private GameObject currentPlantVisual;

    private float growthTimer;


    private void Update()
    {
        if (currentState == PlantGrowthState.Empty)
            return;

        if (currentState == PlantGrowthState.Ready)
            return;

        growthTimer += Time.deltaTime;
        
        UpdateGrowth();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (currentState != PlantGrowthState.Empty)
            return;

        if (!other.TryGetComponent(out Seed seed))
            return;

        if (seed.IsBeingHeld)
            return;

        PlantSeed(seed);
    }


    private void PlantSeed(Seed seed)
    {
        plantedSeedData = seed.SeedData;

        currentState = PlantGrowthState.Seed;

        Destroy(seed.gameObject);

        UpdatePlantVisual();
    }


    private void UpdateGrowth()
    {
        float progress = growthTimer / plantedSeedData.growthTime;

        if (progress >= 1f)
        {
            ChangeState(PlantGrowthState.Ready);
        }
        else if (progress >= 0.66f)
        {
            ChangeState(PlantGrowthState.Growing);
        }
        else if (progress >= 0.33f)
        {
            ChangeState(PlantGrowthState.Germinating);
        }
    }


    private void ChangeState(PlantGrowthState newState)
    {
        currentState = newState;

        UpdatePlantVisual();

        Debug.Log("Planta mudou para: " + currentState);
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
                    sapling.SetPlantingArea(this);
                }
            }
        }
    }

    public void RemovePlant()
    {
        currentPlantVisual = null;

        plantedSeedData = null;

        currentState = PlantGrowthState.Empty;

        growthTimer = 0f;

        Debug.Log("Planta removida. Área disponível novamente.");
    }
}