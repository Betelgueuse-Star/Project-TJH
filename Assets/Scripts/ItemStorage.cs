using TMPro;
using UnityEngine;

public class ItemStorage : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] private ItemTypeSO startingItemType;

    [Header("Storage")]
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private int currentAmount = 10;
    //[SerializeField] private int maxAmount = 20;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    private ItemTypeSO storedItemType;

    public bool IsEmpty => currentAmount <= 0;

    private void Awake()
    {
        //se o estoque ja comecar com zero, e tiver um item inicial, ele vai armazenar o tipo do item inicial direto
        //entao temos uma condicao para verificar se o currentAmount é maior que zero, se for, ele vai armazenar o tipo do item inicial
        //caso for zero, ele nao vai armazenar o tipo do item inicial, e vai ficar null, podendo armazenar outro item
        if (currentAmount > 0) 
        {
            storedItemType = startingItemType;
        }
        if (startingItemType == null)
        {
            currentAmount = 0;
        }
        UpdateAmountText();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IStorableItem storableItem))
            return;

        if (!other.TryGetComponent(out ObjectGrabbable grabbable))
            return;

        if (grabbable.IsBeingHeld)
            return;

        if (IsEmpty)
        {
            storedItemType = storableItem.ItemType;
        }
        if (storedItemType != storableItem.ItemType)
            return;

        currentAmount++;

        UpdateAmountText();

        Destroy(other.gameObject);
    }

    private void UpdateAmountText()
    {
        amountText.text = currentAmount.ToString();
    }
    public void Interact(Player player)
    {
        if (player.IsHoldingObject)
            return;

        if (IsEmpty)
        {
            Debug.Log("Estoque vazio!");
            return;
        }


        GameObject itemObject = Instantiate(
            storedItemType.ItemPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );


        if (!itemObject.TryGetComponent(out ObjectGrabbable grabbable))
        {
            Debug.LogError(
                "O item do estoque não possui ObjectGrabbable!"
            );

            Destroy(itemObject);

            return;
        }


        currentAmount--;
        if (currentAmount == 0)
        {
            storedItemType = null;
        }
        amountText.text = currentAmount.ToString();

        player.GrabObject(grabbable);
    }
}