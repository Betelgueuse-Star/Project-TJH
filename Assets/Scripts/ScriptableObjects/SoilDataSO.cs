using UnityEngine;

[CreateAssetMenu(fileName = "New Soil", menuName = "Game/Data/Soil Data")]
public class SoilDataSO : ScriptableObject
{
    [Header("Informações")]
    public string soilName;

    [Header("Status")]
    public float growthMultiplier;
    public int maxUses;

    [Header("SoilPrefab")]
    public GameObject soilPrefab;
    
}