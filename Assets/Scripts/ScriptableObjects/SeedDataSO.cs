using UnityEngine;

[CreateAssetMenu(fileName = "New Seed", menuName = "Game/Data/Seed Data")]
public class SeedDataSO : ScriptableObject
{
    [Header("Informações")]
    public string seedName;

    [Header("Status")]
    [Min(1)]
    public float baseGrowthTime = 120f;
    public SoilDataSO recommendedSoil;

    [Header("Estágios Visuais")]
    public GameObject seedPrefab;
    public GameObject germinatingPrefab;
    public GameObject growingPrefab;

    [Header("Resultado")]
    public GameObject readyPrefab;
}