using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Pedidos disponíveis")]
    [SerializeField] private OrderDataSO[] availableOrders;

    private OrderDataSO currentOrder;

    private int deliveredAmount;


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

        int randomIndex = Random.Range(0, availableOrders.Length);

        currentOrder = availableOrders[randomIndex];

        deliveredAmount = 0;

        Debug.Log(
            "Novo pedido: " +
            currentOrder.RequestedAmount +
            "x " +
            currentOrder.RequestedSeed.seedName
        );
    }


    public bool TryDeliver(Sapling sapling)
    {
        if (currentOrder == null)
            return false;

        if (sapling.SeedData != currentOrder.RequestedSeed)
        {
            Debug.Log("Esta planta não pertence ao pedido atual.");

            return false;
        }

        deliveredAmount++;

        Debug.Log(
            "Entrega: " +
            deliveredAmount +
            "/" +
            currentOrder.RequestedAmount
        );

        Destroy(sapling.gameObject);

        if (deliveredAmount >= currentOrder.RequestedAmount)
        {
            CompleteOrder();
        }

        return true;
    }


    private void CompleteOrder()
    {
        Debug.Log("PEDIDO COMPLETO!");

        StartNewOrder();
    }
}