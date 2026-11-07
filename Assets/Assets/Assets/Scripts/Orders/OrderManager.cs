using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    private class RequirementProgress
    {
        public ItemTypeSO Item;
        public int RequiredAmount;
        public int DeliveredAmount;
    }
    private class ActiveOrder //estrutura para armazenar o pedido ativo e o progresso dos requisitos
    {
        public OrderDataSO Order;
        public RequirementProgress[] RequirementProgress;
    }

    public event Action OnOrderChanged;
    public event Action OnOrderProgressChanged;

    public static OrderManager Instance { get; private set; }

    [Header("Pedidos disponíveis")]
    [SerializeField] private OrderDataSO[] availableOrders;

    [Header("Currency")]
    [SerializeField] private CurrencyManager currencyManager;

    private int activeOrderIndex;
    private List<ActiveOrder> activeOrders = new List<ActiveOrder>(); // Lista de pedidos ativos

    //current order é o indice 0 da lista de pedidos ativos
    public OrderDataSO CurrentOrder =>
    activeOrders.Count > 0 ? activeOrders[activeOrderIndex].Order: null;

    public int ActiveOrderIndex => activeOrderIndex;
    public int ActiveOrderCount => activeOrders.Count;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartNewOrder();
    }

    // Inicia um novo pedido aleatório a partir da lista de pedidos disponíveis, caso ja houver um pedido ativo, ele não será adicionado novamente
    // e assim criando um novo pedido diferente, caso não haja mais pedidos disponíveis, ele não fará nada
    public void StartNewOrder()
    {
        if (availableOrders.Length == 0)
        {
            Debug.LogWarning("Nenhum pedido foi configurado.");
            return;
        }

        List<OrderDataSO> availableNewOrders = new List<OrderDataSO>();

        foreach (OrderDataSO order in availableOrders)
        {
            bool alreadyActive = false;

            foreach (ActiveOrder activeOrder in activeOrders)
            {
                if (activeOrder.Order == order)
                {
                    alreadyActive = true;
                    break;
                }
            }

            if (!alreadyActive)
                availableNewOrders.Add(order);
        }

        if (availableNewOrders.Count == 0)
        {
            Debug.LogWarning("Não há mais pedidos disponíveis.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, availableNewOrders.Count);
        OrderDataSO selectedOrder = availableNewOrders[randomIndex];

        RequirementProgress[] progress =
            new RequirementProgress[selectedOrder.Requirements.Length];

        for (int i = 0; i < selectedOrder.Requirements.Length; i++)
        {
            OrderRequirement requirement = selectedOrder.Requirements[i];

            progress[i] = new RequirementProgress
            {
                Item = requirement.Item,
                RequiredAmount = requirement.Amount,
                DeliveredAmount = 0
            };
        }

        ActiveOrder newActiveOrder = new ActiveOrder
        {
            Order = selectedOrder,
            RequirementProgress = progress
        };

        activeOrders.Add(newActiveOrder);

        OnOrderChanged?.Invoke();
    }

    // o pedido atual é o índice 0 da lista de pedidos ativos, caso não haja pedidos ativos, ele retorna false
    public bool TryDeliver(IDeliverable deliverable, GameObject deliveredObject)
    {
        if (activeOrders.Count == 0)
            return false;

        ActiveOrder activeOrder = activeOrders[activeOrderIndex];

        RequirementProgress matchingRequirement = null;

        foreach (RequirementProgress progress in activeOrder.RequirementProgress)
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

        OnOrderProgressChanged?.Invoke();

        if (IsOrderComplete())
        {
            CompleteOrder();
        }

        return true;
    }

    private bool IsOrderComplete()
    {
        ActiveOrder activeOrder = activeOrders[activeOrderIndex];

        foreach (RequirementProgress progress in activeOrder.RequirementProgress)
        {
            if (progress.DeliveredAmount < progress.RequiredAmount)
                return false;
        }

        return true;
    }

    private void CompleteOrder()
    {
        ActiveOrder completedOrder = activeOrders[activeOrderIndex];

        Debug.Log("PEDIDO COMPLETO!");

        currencyManager.AddCurrency(completedOrder.Order.RewardAmount);

        activeOrders.RemoveAt(activeOrderIndex);


        //corrigi o índice do pedido ativo caso o pedido atual seja removido, para evitar que o índice fique fora do intervalo da lista de pedidos ativos
        if (activeOrders.Count == 0)
        {
            activeOrderIndex = 0;
        }
        else if (activeOrderIndex >= activeOrders.Count)
        {
            activeOrderIndex = activeOrders.Count - 1;
        }

        OnOrderChanged?.Invoke();
    }

    //troca o pedido atual com o proxmimo pedido ativo na lista de pedidos ativos, caso não haja pedidos ativos, ele não fará nada
    public void SwitchActiveOrder()
    {
        if (activeOrders.Count == 0)
            return;

        activeOrderIndex++;

        if (activeOrderIndex >= activeOrders.Count)//volta para o primeiro
            activeOrderIndex = 0;

        OnOrderChanged?.Invoke();
    }

    public int GetDeliveredAmount(int orderIndex, ItemTypeSO item)
    {
        if (orderIndex < 0 || orderIndex >= activeOrders.Count)
            return 0;

        foreach (RequirementProgress progress in activeOrders[orderIndex].RequirementProgress)
        {
            if (progress.Item == item)
                return progress.DeliveredAmount;
        }

        return 0;
    }

    public OrderDataSO GetOrder(int index)
    {
        if (index < 0 || index >= activeOrders.Count)
            return null;

        return activeOrders[index].Order;
    }
}