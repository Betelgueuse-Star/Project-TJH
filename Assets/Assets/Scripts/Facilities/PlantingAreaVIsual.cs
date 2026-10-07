
using UnityEngine;
using TMPro;

public class PlantingAreaVisual : MonoBehaviour
{
    [Header("Planting")]
    [SerializeField] private Transform plantingPoint;

    [Header("Status UI")]
    [SerializeField] private TMP_Text waterText;
    [SerializeField] private TMP_Text growthStageText;

    private ParticleSystem currentFertilizerEffect;
    private GameObject currentPlantVisual;
    private GameObject currentSoilVisual;

    public void UpdateWaterText(float currentWater)
    {
        if (waterText == null) return;

        waterText.text = $"Água: {currentWater:0.##}";
    }

    public void UpdateGrowthStageText(PlantGrowthState currentState)
    {
        if (growthStageText == null) return;

        growthStageText.text = $"Estágio: {currentState}";
    }

    public void UpdateSoilVisual(SoilDataSO soilData)
    {
        if (currentSoilVisual != null)
            Destroy(currentSoilVisual);

        if (soilData != null && soilData.soilPrefab != null)
        {
            currentSoilVisual = Instantiate(
                soilData.soilPrefab,
                plantingPoint.position,
                plantingPoint.rotation
            );
        }
    }

    public void UpdatePlantVisual(
        PlantGrowthState currentState,
        SeedDataSO plantedSeedData,
        PlantingArea plantingArea)
    {
        if (currentPlantVisual != null)
            Destroy(currentPlantVisual);

        GameObject prefabToSpawn = null;

        if (plantedSeedData != null)
        {
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
        }

        if (prefabToSpawn != null)
        {
            currentPlantVisual = Instantiate(
                prefabToSpawn,
                plantingPoint.position,
                plantingPoint.rotation
            );

            if (currentState == PlantGrowthState.Ready &&
                currentPlantVisual.TryGetComponent(out Sapling sapling))
            {
                sapling.Setup(plantedSeedData, plantingArea);
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
            currentPlantVisual = null;
    }

    public void RemoveSoilVisual()
    {
        if (currentSoilVisual != null)
        {
            Destroy(currentSoilVisual);
            currentSoilVisual = null;
        }
    }

    public void PlayFertilizerEffect(FertilizerDataSO fertilizerData)
    {
        if (fertilizerData == null ||
            fertilizerData.fertilizerParticle == null)
            return;

        DestroyFertilizerEffect();

        currentFertilizerEffect = Instantiate(
            fertilizerData.fertilizerParticle,
            plantingPoint.position,
            plantingPoint.rotation
        );

        currentFertilizerEffect.Play();
    }

    public void DestroyFertilizerEffect()
    {
        if (currentFertilizerEffect != null)
        {
            Destroy(currentFertilizerEffect.gameObject);
            currentFertilizerEffect = null;
        }
    }
}