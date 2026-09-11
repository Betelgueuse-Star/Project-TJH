using UnityEngine;
using System.Collections;


public class PlantingArea : MonoBehaviour
{
    [Header("Planting")]
    [SerializeField] private Transform plantingPoint;

    private SeedDataSO plantedSeedData;
    private PlantGrowthState currentState = PlantGrowthState.Empty;

    private GameObject currentPlantVisual;

    //private float growthTimer;

    private void OnTriggerEnter(Collider other)
    {
        if (currentState != PlantGrowthState.Empty)
            return;

        if (!other.TryGetComponent(out Seed seed))
            return;

        if (!other.TryGetComponent(out ObjectGrabbable grabbable))
        return;

        if (grabbable.IsBeingHeld)
            return;

        PlantSeed(seed);
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
        float stageTime = plantedSeedData.growthTime / 3f;

        yield return new WaitForSeconds(stageTime);

        ChangeState(PlantGrowthState.Germinating);

        yield return new WaitForSeconds(stageTime);

        ChangeState(PlantGrowthState.Growing);

        yield return new WaitForSeconds(stageTime);

        ChangeState(PlantGrowthState.Ready);
    }

    private void ChangeState(PlantGrowthState newState)
    {
        currentState = newState;

        UpdatePlantVisual();

        //Debug.Log("Planta mudou para: " + currentState);
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