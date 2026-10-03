using TMPro;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New Item Type",
    menuName = "Game/Items/Item Type"
)]
public class ItemTypeSO : ScriptableObject
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private string itemName;

    public GameObject ItemPrefab => itemPrefab;
    public string ItemName => itemName;
}