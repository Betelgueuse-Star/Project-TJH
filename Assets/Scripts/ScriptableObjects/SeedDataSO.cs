using UnityEngine;

[CreateAssetMenu(fileName = "New Seed", menuName = "Game/Data/Seed Data")]
public class SeedDataSO : ScriptableObject
{
    [Header("Informações")]
    public string seedName;

    [Header("Crescimento")]
    [Min(1)]
    public float growthTime = 30f;

    [Header("Estágios Visuais")]
    public GameObject seedPrefab;
    public GameObject germinatingPrefab;
    public GameObject growingPrefab;

    [Header("Resultado")]
    public GameObject readyPrefab;
}