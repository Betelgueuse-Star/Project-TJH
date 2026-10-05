using UnityEngine;
using System;

public class OrderManager : MonoBehaviour
{
    private class RequirementProgress
    {
        public ItemTypeSO Item;
        public int RequiredAmount;
        public int DeliveredAmount;
    }

    public event Action OnOrderChanged;
    public event Action OnOrderProgressChanged;

    public static OrderManager Instance { get; private set; }

    [Header("Pedidos disponíveis")]
    [SerializeField] private OrderDataSO[] availableOrders;

    [Header("Currency")]
    [SerializeField] private CurrencyManager currencyManager;

    private OrderDataSO currentOrder;
    private RequirementProgress[] requirementProgress;

    public OrderDataSO CurrentOrder => currentOrder;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartNewOrder();
    }

    public void StartNewOrder()
    {
        if (availableOrders.Length == 0)
        {
            Debug.LogWarning("Nenhum pedido foi configurado.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, availableOrders.Length);

        currentOrder = availableOrders[randomIndex];

        requirementProgress = new RequirementProgress[currentOrder.Requirements.Length];

        for (int i = 0; i < currentOrder.Requirements.Length; i++)
        {
            OrderRequirement requirement = currentOrder.Requirements[i];

            requirementProgress[i] = new RequirementProgress
            {
                Item = requirement.Item,
                RequiredAmount = requirement.Amount,
                DeliveredAmount = 0
            };
        }

        OnOrderChanged?.Invoke();
    }

    public bool TryDeliver(IDeliverable deliverable, GameObject deliveredObject)
    {
        if (currentOrder == null)
            return false;

        RequirementProgress matchingRequirement = null;

        foreach (RequirementProgress progress in requirementProgress)
        {
            if (progress.Item == deliverable.ItemType)
            {
                matchingRequirement = progress;
                break;
            }
        }

        if (matchingRequirement == null)
        {
            Debug.Log("Este item não pertence ao pedido atual.");
            return false;
        }

        if (matchingRequirement.DeliveredAmount >= matchingRequirement.RequiredAmount)
        {
            Debug.Log("A quantidade deste item já foi completada.");
            return false;
        }

        matchingRequirement.DeliveredAmount++;

        Destroy(deliveredObject);

        if (IsOrderComplete())
        {
            CompleteOrder();
        }

        return true;
    }

    private bool IsOrderComplete()
    {
        foreach (RequirementProgress progress in requirementProgress)
        {
            if (progress.DeliveredAmount < progress.RequiredAmount)
                return false;
        }

        return true;
    }

    private void CompleteOrder()
    {
        Debug.Log("PEDIDO COMPLETO!");
        OnOrderProgressChanged?.Invoke();

        currencyManager.AddCurrency(currentOrder.RewardAmount);

        StartNewOrder();
    }
    public int GetDeliveredAmount(ItemTypeSO item)
    {
        foreach (RequirementProgress progress in requirementProgress)
        {
            if (progress.Item == item)
                return progress.DeliveredAmount;
        }

        return 0;
    }
}