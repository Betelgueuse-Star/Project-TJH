using UnityEngine;

public class TerminalOrderUI : MonoBehaviour
{
    [Header("Orders")]
    [SerializeField] private Transform ordersContainer;
    [SerializeField] private GameObject orderPrefab;
    [SerializeField] private GameObject requirementPrefab;

    private void OnEnable()
    {
        if (OrderManager.Instance == null)
            return;

        OrderManager.Instance.OnOrderChanged += Refresh;
        OrderManager.Instance.OnOrderProgressChanged += Refresh;

        Refresh();
    }

    public void Refresh()
    {
        ClearOrders();

        int orderCount = OrderManager.Instance.ActiveOrderCount;

        for (int i = 0; i < orderCount; i++)
        {
            CreateOrderUI(i);
        }
    }

    private void CreateOrderUI(int orderIndex)
    {
        OrderDataSO order = OrderManager.Instance.GetOrder(orderIndex);

        if (order == null)
            return;

        GameObject orderObject = Instantiate(
            orderPrefab,
            ordersContainer
        );

        OrderUI orderUI = orderObject.GetComponent<OrderUI>();

        orderUI.ActiveIndicator.SetActive(
            orderIndex == OrderManager.Instance.ActiveOrderIndex
        );

        orderUI.RewardText.text = $"Reward: ${order.RewardAmount}";

        foreach (OrderRequirement requirement in order.Requirements)
        {
            GameObject requirementObject = Instantiate(
                requirementPrefab,
                orderUI.RequirementsContainer
            );

            OrderRequirementUI requirementUI =
                requirementObject.GetComponent<OrderRequirementUI>();

            int deliveredAmount =
                OrderManager.Instance.GetDeliveredAmount(
                    orderIndex,
                    requirement.Item
                );

            requirementUI.Setup(
                requirement.Item.ItemName,
                deliveredAmount,
                requirement.Amount
            );
        }
    }

    private void ClearOrders()
    {
        for (int i = ordersContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(ordersContainer.GetChild(i).gameObject);
        }
    }

    private void OnDisable()
    {
        if (OrderManager.Instance == null)
            return;

        OrderManager.Instance.OnOrderChanged -= Refresh;
        OrderManager.Instance.OnOrderProgressChanged -= Refresh;
    }
}