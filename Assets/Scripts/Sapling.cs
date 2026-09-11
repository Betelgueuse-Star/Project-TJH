using UnityEngine;

[RequireComponent(typeof(ObjectGrabbable))]
public class Sapling : MonoBehaviour, IDeliverable
{
    [Header("Delivery")]
    [SerializeField] private ItemTypeSO itemType;

    private ObjectGrabbable objectGrabbable;
    private PlantingArea plantingArea;

    public SeedDataSO SeedData { get; private set; }

    public ItemTypeSO ItemType => itemType;


    private void Awake()
    {
        objectGrabbable = GetComponent<ObjectGrabbable>();

        objectGrabbable.OnGrabbed += RemoveFromPlantingArea;
    }


    private void OnDestroy()
    {
        if (objectGrabbable != null)
        {
            objectGrabbable.OnGrabbed -= RemoveFromPlantingArea;
        }
    }


    public void Setup(SeedDataSO seedData, PlantingArea area)
    {
        SeedData = seedData;
        plantingArea = area;
    }


    public void RemoveFromPlantingArea()
    {
        if (plantingArea == null)
            return;

        plantingArea.RemovePlant();

        plantingArea = null;
    }
}