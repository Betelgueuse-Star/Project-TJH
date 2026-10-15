using UnityEngine;

[RequireComponent(typeof(ObjectGrabbable))]
public class SoilBag : MonoBehaviour, IStorableItem, IAreaItem
{
    
    [Header("Soil")]
    [SerializeField] private SoilDataSO soilData;

    [Header("ItemTypeSO")]
    [SerializeField] private ItemTypeSO itemType;

    public SoilDataSO SoilData => soilData;
    public ItemTypeSO ItemType => itemType;


    // Só entra se a área ainda não tem solo.
    public bool CanApply(PlantingArea area) => !area.HasSoil;

    public void Apply(PlantingArea area)
    {
        area.PlaceSoil(SoilData);
        Destroy(gameObject);
    }
}