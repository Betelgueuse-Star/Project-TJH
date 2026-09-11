using UnityEngine;

[CreateAssetMenu(
    fileName = "New Item Type",
    menuName = "Game/Items/Item Type"
)]
public class ItemTypeSO : ScriptableObject
{
    [SerializeField] private GameObject itemPrefab;

    public GameObject ItemPrefab => itemPrefab;
}