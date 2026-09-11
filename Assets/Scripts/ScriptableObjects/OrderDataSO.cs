using UnityEngine;

[CreateAssetMenu(fileName = "New Order", menuName = "Game/Orders/Order Data")]
public class OrderDataSO : ScriptableObject
{
    [Header("Pedido")]
    [SerializeField] private ItemTypeSO requestedItem;

    [Min(1)]
    [SerializeField] private int requestedAmount = 1;

    public ItemTypeSO RequestedItem => requestedItem;
    public int RequestedAmount => requestedAmount;
}