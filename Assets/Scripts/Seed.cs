using UnityEngine;

public class Seed : MonoBehaviour, IStorableItem
{
    [Header("Seed")]
    [SerializeField] private SeedDataSO seedData;

    [Header("ItemTypeSO")]
    [SerializeField] private ItemTypeSO itemType;


    public SeedDataSO SeedData => seedData;
    public ItemTypeSO ItemType => itemType;
}