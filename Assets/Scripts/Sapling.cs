using UnityEngine;

[RequireComponent(typeof(ObjectGrabbable))]
public class Sapling : MonoBehaviour
{
    private ObjectGrabbable objectGrabbable;
    private PlantingArea plantingArea;

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

    public void SetPlantingArea(PlantingArea area)
    {
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