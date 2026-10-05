using TMPro;
using UnityEngine;

public class TerminalOrderUI : MonoBehaviour
{
    [Header("Order")]
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Transform requirementsContainer;
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
        ClearRequirements();

        OrderDataSO order = OrderManager.Instance.CurrentOrder;

        if (order == null)
            return;

        rewardText.text = $"Recompensa: ${order.RewardAmount}";

        foreach (OrderRequirement requirement in order.Requirements)
        {
            GameObject requirementObject = Instantiate(
                requirementPrefab,
                requirementsContainer
            );

            OrderRequirementUI requirementUI =
                requirementObject.GetComponent<OrderRequirementUI>();

            int deliveredAmount =
                OrderManager.Instance.GetDeliveredAmount(requirement.Item);

            requirementUI.Setup(
                requirement.Item.ItemName,
                deliveredAmount,
                requirement.Amount
            );
        }
    }

    private void ClearRequirements()
    {
        for (int i = requirementsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(requirementsContainer.GetChild(i).gameObject);
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