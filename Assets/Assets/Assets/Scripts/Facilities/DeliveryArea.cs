using UnityEngine;

public class DeliveryArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDeliverable deliverable))
            return;

        if (!other.TryGetComponent(out ObjectGrabbable grabbable))
            return;

        if (grabbable.IsBeingHeld)
            return;

        OrderManager.Instance.TryDeliver(deliverable, other.gameObject);
    }
}