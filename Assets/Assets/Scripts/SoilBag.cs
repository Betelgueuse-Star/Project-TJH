using UnityEngine;

[RequireComponent(typeof(ObjectGrabbable))]
public class SoilBag : MonoBehaviour, IStorableItem
{
    
    [Header("Soil")]
    [SerializeField] private SoilDataSO soilData;

    [Header("ItemTypeSO")]
    [SerializeField] private ItemTypeSO itemType;

    public SoilDataSO SoilData => soilData;
    public ItemTypeSO ItemType => itemType;
}