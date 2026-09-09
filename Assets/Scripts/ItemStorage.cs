using TMPro;
using UnityEngine;

public class ItemStorage : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] private GameObject itemPrefab;

    [Header("Storage")]
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private int currentAmount = 10;
    //[SerializeField] private int maxAmount = 20;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        amountText.text = currentAmount.ToString();
    }
    public void Interact(Player player)
    {
        if (player.IsHoldingObject)
            return;

        if (currentAmount <= 0)
        {
            Debug.Log("Estoque vazio!");

            return;
        }


        GameObject itemObject = Instantiate(
            itemPrefab,
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
        amountText.text = currentAmount.ToString();

        player.GrabObject(grabbable);
    }
}