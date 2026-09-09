using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerCameraGameObject;
    [SerializeField] private GameObject objectGrabPointGameObject;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private float pickUpRange = 3f;

    private ObjectGrabbable objectGrabbable;

    public bool IsHoldingObject => objectGrabbable != null;

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (IsHoldingObject)
        {
            objectGrabbable.Drop();
            objectGrabbable = null;

            return;
        }

        //se nao acertar nada, return
        if (!Physics.Raycast(playerCameraGameObject.transform.position, playerCameraGameObject.transform.forward, out RaycastHit raycastHitInfo, pickUpRange, pickUpLayerMask))
        {
            return; 
        }

        //Debug.Log("Hit object: " + raycastHitInfo.transform.name);
        if (raycastHitInfo.transform.TryGetComponent(out ObjectGrabbable grabbable)) //atribui o objeto que foi pego para a variavel objectGrabbable
        {
            GrabObject(grabbable);

            return;
        }
        if (raycastHitInfo.transform.TryGetComponent(out IInteractable interactable))//busca se o scriptpossui a interface IInteractable
        {
            interactable.Interact(this);
        }
    }

    public void GrabObject(ObjectGrabbable newObjectGrabbable)
    {
        if (IsHoldingObject)
            return;

        objectGrabbable = newObjectGrabbable;

        objectGrabbable.Grab(objectGrabPointGameObject.transform);
    }
}
