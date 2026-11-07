using UnityEngine;

[CreateAssetMenu(fileName = "New Order", menuName = "Game/Orders/Order Data")]
public class OrderDataSO : ScriptableObject
{
    [Header("Pedido")]
    [SerializeField] private OrderRequirement[] requirements;

    [Header("Recompensa")]
    [SerializeField] private int rewardAmount = 10;

    public OrderRequirement[] Requirements => requirements;
    public int RewardAmount => rewardAmount;

    // This method is called when the script is loaded or a value is changed in the inspector (Editor only).
    // Impede que OrderDataSO nao receba itens que nao implementam IDeliverable, pois isso causaria problemas na hora de entregar o pedido.
    private void OnValidate()
    {
        if (requirements == null)
            return;

        foreach (OrderRequirement requirement in requirements)
        {
            if (requirement == null || requirement.Item == null)
                continue;

            if (requirement.Item.ItemPrefab == null)
            {
                Debug.LogWarning(
                    $"Order '{name}': Item '{requirement.Item.ItemName}' has no prefab.",
                    this
                );

                continue;
            }

            if (!requirement.Item.ItemPrefab.TryGetComponent<IDeliverable>(out _))
            {
                Debug.LogWarning(
                    $"Order '{name}': Item '{requirement.Item.ItemName}' cannot be delivered because its prefab does not implement IDeliverable.",
                    this
                );
            }
        }
    }
}