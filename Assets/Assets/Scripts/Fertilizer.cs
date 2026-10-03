using UnityEngine;

[RequireComponent(typeof(ObjectGrabbable))]
public class Fertilizer : MonoBehaviour, IStorableItem
{
    [Header("Fertilizer")]
    [SerializeField] private FertilizerDataSO fertilizerData;

    [Header("ItemTypeSO")]
    [SerializeField] private ItemTypeSO itemType;

    public FertilizerDataSO FertilizerData => fertilizerData;
    public ItemTypeSO ItemType => itemType;
}