using UnityEngine;

public class DeliveryArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.TryGetComponent(out Sapling sapling))
            return;
        if (sapling.IsBeingHeld)
            return;

        OrderManager.Instance.TryDeliver(sapling);
    }
}