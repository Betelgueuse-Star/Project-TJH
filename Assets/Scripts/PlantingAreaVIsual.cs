using UnityEngine;
using System.Collections;


public class PlantingAreaVisual : MonoBehaviour
{
    [Header("Planting")]
    [SerializeField] private Transform plantingPoint;
    private GameObject currentPlantVisual;
    private GameObject currentSoilVisual;
    
    public void UpdateSoilVisual(SoilDataSO soilData)
    {
        if (currentSoilVisual != null)
        {
            Destroy(currentSoilVisual);
        }
        if (soilData != null && soilData.soilPrefab != null)
        {
            currentSoilVisual = Instantiate(
                soilData.soilPrefab,
                plantingPoint.position,
                plantingPoint.rotation
            );
        }
    }

    public void UpdatePlantVisual(PlantGrowthState currentState, SeedDataSO plantedSeedData, PlantingArea plantingArea)
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
                    sapling.Setup(plantedSeedData, plantingArea);
                }
            }
        }
    }

    public void DestroyPlant()
    {
        if (currentPlantVisual != null)
        {
            Destroy(currentPlantVisual);
            currentPlantVisual = null;
        }
    }

    public void RemovePlantVisual()
    {
        if (currentPlantVisual != null)
        {
            currentPlantVisual = null;
        }
    }

    public void RemoveSoilVisual()
    {
        if (currentSoilVisual != null)
        {
            Destroy(currentSoilVisual);
            currentSoilVisual = null;
        }
    }
}