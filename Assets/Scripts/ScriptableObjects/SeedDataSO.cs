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
}