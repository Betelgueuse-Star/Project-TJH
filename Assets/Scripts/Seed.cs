using UnityEngine;

public class Seed : MonoBehaviour, IStorableItem
{
    [Header("Seed")]
    [SerializeField] private SeedDataSO seedData;

    [Header("Soil")]
    [SerializeField] private SoilDataSO soilData;

    [Header("ItemTypeSO")]
    [SerializeField] private ItemTypeSO itemType;


    public SeedDataSO SeedData => seedData;
    public ItemTypeSO ItemType => itemType;


    public bool IsPlanted { get; private set; }


    public void Plant()
    {
        if (IsPlanted)
            return;

        IsPlanted = true;

        Debug.Log(seedData.seedName + " foi plantada!");
    }
}