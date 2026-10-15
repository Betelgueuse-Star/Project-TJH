using UnityEngine;

[RequireComponent(typeof(ObjectGrabbable))]
public class Fertilizer : MonoBehaviour, IStorableItem, IAreaItem
{
    [Header("Fertilizer")]
    [SerializeField] private FertilizerDataSO fertilizerData;

    [Header("ItemTypeSO")]
    [SerializeField] private ItemTypeSO itemType;

    public FertilizerDataSO FertilizerData => fertilizerData;
    public ItemTypeSO ItemType => itemType;

    //  Precisa de solo, área vazia, sem outro fertilizante e com dados válidos.
    public bool CanApply(PlantingArea area) =>
       area.HasSoil && area.IsEmpty && !area.HasFertilizer && FertilizerData != null;

    public void Apply(PlantingArea area)
    {
        area.PlaceFertilizer(FertilizerData);
        Destroy(gameObject);
    }
}
