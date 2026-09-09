using UnityEngine;

[CreateAssetMenu(fileName = "New Order", menuName = "Game/Orders/Order Data")]
public class OrderDataSO : ScriptableObject
{
    [Header("Pedido")]
    [SerializeField] private SeedDataSO requestedSeed;

    [Min(1)]
    [SerializeField] private int requestedAmount = 1;

    public SeedDataSO RequestedSeed => requestedSeed;
    public int RequestedAmount => requestedAmount;
}