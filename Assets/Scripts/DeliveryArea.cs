using UnityEngine;

public class DeliveryArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.TryGetComponent(out Sapling sapling))
            return;

        if (!other.TryGetComponent(out ObjectGrabbable grabbable))
            return;

        if (grabbable.IsBeingHeld)
            return;

        OrderManager.Instance.TryDeliver(sapling);
    }
}