using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerCameraGameObject;
    [SerializeField] private GameObject objectGrabPointGameObject;
    [SerializeField] private LayerMask interactionLayerMask;
    [SerializeField] private float interactionRange = 3f;

    private ObjectGrabbable objectGrabbable;
    private InteractableOutline currentOutline;

    public bool IsHoldingObject => objectGrabbable != null;

    private void Update()
    {
        UpdateOutline();
    }

    private void UpdateOutline()
    {
        if (IsHoldingObject)
        {
            HideCurrentOutline();
            return;
        }

        if (!Physics.Raycast(
            playerCameraGameObject.transform.position,
            playerCameraGameObject.transform.forward,
            out RaycastHit raycastHitInfo,
            interactionRange,
            interactionLayerMask))
        {
            HideCurrentOutline();
            return;
        }

        if (!raycastHitInfo.transform.TryGetComponent(
            out InteractableOutline outline))
        {
            HideCurrentOutline();
            return;
        }

        if (currentOutline == outline)
            return;

        HideCurrentOutline();

        currentOutline = outline;
        currentOutline.Show();
    }

    private void HideCurrentOutline()
    {
        if (currentOutline == null)
            return;

        currentOutline.Hide();
        currentOutline = null;
    }

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
        if (!Physics.Raycast(playerCameraGameObject.transform.position, playerCameraGameObject.transform.forward, out RaycastHit raycastHitInfo, interactionRange, interactionLayerMask))
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
