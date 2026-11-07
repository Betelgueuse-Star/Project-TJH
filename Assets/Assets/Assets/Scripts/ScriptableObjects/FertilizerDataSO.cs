using UnityEngine;

[CreateAssetMenu(fileName = "New Fertilizer", menuName = "Game/Data/Fertilizer Data")]
public class FertilizerDataSO : ScriptableObject
{
    [Header("Informações")]
    public string fertilizerName;

    [Header("Status")]
    [Min(0.1f)] //evita o 0 acidentalmente
    public float growthMultiplier;

    [Header("FertilizerParticle")]
    public ParticleSystem fertilizerParticle;
}