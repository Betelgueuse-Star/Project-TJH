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

    //É genérico para servir
    // tanto para GameObject quanto para ParticleSystem.
    private T SpawnAtPoint<T>(T prefab) where T : Object
    {
        return Instantiate(prefab, plantingPoint.position, plantingPoint.rotation);
    }

    private static void DestroyAndClear(ref GameObject visual)
    {
        if (visual == null)
            return;

        Destroy(visual);
        visual = null;
    }

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
        DestroyAndClear(ref currentSoilVisual);

        if (soilData != null && soilData.soilPrefab != null)
        {
            currentSoilVisual = SpawnAtPoint(soilData.soilPrefab);
        }
    }

    public void UpdatePlantVisual(PlantGrowthState currentState, SeedDataSO plantedSeedData, PlantingArea plantingArea)
    {
        DestroyAndClear(ref currentPlantVisual);

        // Pega o prefab do estágio atual da planta, se houver um seedData plantado.
        GameObject prefabToSpawn = plantedSeedData != null
            ? plantedSeedData.GetStagePrefab(currentState)
            : null;

        if (prefabToSpawn == null)
            return;

        currentPlantVisual = SpawnAtPoint(prefabToSpawn);

        if (currentState == PlantGrowthState.Ready && currentPlantVisual.TryGetComponent(out Sapling sapling))
        {
            sapling.Setup(plantedSeedData, plantingArea);
        }
    }

    public void DestroyPlant()
    {
        DestroyAndClear(ref currentPlantVisual);
    }

    public void RemovePlantVisual() 
    {
        currentPlantVisual = null;
    }

    public void RemoveSoilVisual()
    {
        DestroyAndClear(ref currentSoilVisual);
    }

    public void PlayFertilizerEffect(FertilizerDataSO fertilizerData)
    {
        if (fertilizerData == null || fertilizerData.fertilizerParticle == null)
            return;

        DestroyFertilizerEffect();

        currentFertilizerEffect = SpawnAtPoint(fertilizerData.fertilizerParticle);
        currentFertilizerEffect.Play();
    }

    // O efeito é um ParticleSystem, então destrói o gameObject dele (por isso não
    // usa o DestroyAndClear, que trabalha com GameObject).
    public void DestroyFertilizerEffect()
    {
        if (currentFertilizerEffect == null)
            return;

        Destroy(currentFertilizerEffect.gameObject);
        currentFertilizerEffect = null;
    }
}