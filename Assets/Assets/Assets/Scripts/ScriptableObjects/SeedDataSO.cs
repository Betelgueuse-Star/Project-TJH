using UnityEngine;

[CreateAssetMenu(fileName = "New Seed", menuName = "Game/Data/Seed Data")]
public class SeedDataSO : ScriptableObject
{
    [Header("Informações")]
    public string seedName;

    [Header("Status")]
    [Min(1)]
    public float baseGrowthTime = 120f;
    public float waterTolerance = 2f;
    public SoilDataSO recommendedSoil;

    [Header("Água por Estágio")]
    [Min(0)]
    public float seedRequiredWater = 10f;

    [Min(0)]
    public float germinatingRequiredWater = 10f;

    [Min(0)]
    public float growingRequiredWater = 10f;

    [Header("Estágios Visuais")]
    public GameObject seedPrefab;
    public GameObject germinatingPrefab;
    public GameObject growingPrefab;

    [Header("Resultado")]
    public GameObject readyPrefab;

    // MUDANÇA: antes o switch por estágio existia em dois lugares (PlantingArea para a
    // água e PlantingAreaVisual para o prefab). Como os dados estão aqui, as
    // consultas também ficam aqui: um lugar só para mexer ao criar um novo estágio.

    // Prefab visual correspondente ao estágio (null se não houver).
    public GameObject GetStagePrefab(PlantGrowthState state)
    {
        switch (state)
        {
            case PlantGrowthState.Seed:
                return seedPrefab;

            case PlantGrowthState.Germinating:
                return germinatingPrefab;

            case PlantGrowthState.Growing:
                return growingPrefab;

            case PlantGrowthState.Ready:
                return readyPrefab;

            default:
                return null;
        }
    }

    // Quantidade de água necessária no estágio (0 nos estágios sem exigência).
    public float GetRequiredWater(PlantGrowthState state)
    {
        switch (state)
        {
            case PlantGrowthState.Seed:
                return seedRequiredWater;

            case PlantGrowthState.Germinating:
                return germinatingRequiredWater;

            case PlantGrowthState.Growing:
                return growingRequiredWater;

            default:
                return 0f;
        }
    }
}