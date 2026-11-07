using UnityEngine;

[CreateAssetMenu(fileName = "New Soil", menuName = "Game/Data/Soil Data")]
public class SoilDataSO : ScriptableObject
{
    [Header("Informações")]
    public string soilName;

    [Header("Status")]
    [Min(0.1f)] //evita o 0 acidentalmente
    public float growthMultiplier;
    public int maxUses;

    [Header("SoilPrefab")]
    public GameObject soilPrefab;
    
}