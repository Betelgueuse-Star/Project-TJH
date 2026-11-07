using UnityEngine;

public class Seed : MonoBehaviour, IStorableItem, IAreaItem
{
    [Header("Seed")]
    [SerializeField] private SeedDataSO seedData;

    [Header("ItemTypeSO")]
    [SerializeField] private ItemTypeSO itemType;


    public SeedDataSO SeedData => seedData;
    public ItemTypeSO ItemType => itemType;

    //precisa de solo e a área precisa estar vazia.
    public bool CanApply(PlantingArea area) => area.HasSoil && area.IsEmpty;

    public void Apply(PlantingArea area)
    {
        area.PlantSeed(SeedData);
        Destroy(gameObject);
    }
}