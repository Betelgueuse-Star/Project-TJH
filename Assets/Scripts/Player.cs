using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerCameraGameObject;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private Transform toolHoldPointTransform;
    [SerializeField] private LayerMask interactionLayerMask;
    [SerializeField] private float interactionRange = 3f;

    private GameObject currentTool;
    private GameObject currentToolWorldPrefab;

    private ObjectGrabbable objectGrabbable;
    private InteractableOutline currentOutline; 

    public GameObject CurrentTool => currentTool;
    public bool IsHoldingSomething => objectGrabbable != null || currentTool != null;

    private void Update()
    {
        UpdateOutline();
    }

    private void UpdateOutline()
    {
        if (IsHoldingSomething)
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

    private void DropCurrentItem()
    {
        if (objectGrabbable != null)
        {
            objectGrabbable.Drop();
            objectGrabbable = null;
            return;
        }

        //criar um prefab do objeto que estava no mundo, instanciar ele e destruir o objeto que estava na mão
        if (currentTool != null)  //&& currentToolWorldPrefab != null - teste
        {
            Instantiate(
                currentToolWorldPrefab,
                toolHoldPointTransform.position,
                transform.rotation
            );

            Destroy(currentTool);

            currentTool = null;
            return;
        }
    }

     //armazena a referencia do prefab do objeto que foi pego e instancia ele,e destrói o objeto que estava no mundo mas tbm guarda a referencia.
    private void GrabTool(Tool newTool) 
    {
        if (IsHoldingSomething)
            return; 

        currentToolWorldPrefab = newTool.WorldPrefab;

        currentTool = Instantiate(
            newTool.EquippedPrefab,
            toolHoldPointTransform
        );

        currentTool.transform.localPosition = Vector3.zero;
        currentTool.transform.localRotation = Quaternion.identity;

        Destroy(newTool.gameObject);
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

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
        if (raycastHitInfo.transform.TryGetComponent(out Tool tool)) //atribui o objeto que foi pego para a variavel objectGrabbable
        {
            GrabTool(tool);
            return;
        }
        if (raycastHitInfo.transform.TryGetComponent(out IInteractable interactable))//busca se o scriptpossui a interface IInteractable
        {
            interactable.Interact(this);
        }
    }

    public void OnDrop(InputValue value)
    {
        if (!value.isPressed)
            return;

        DropCurrentItem();
    }

    public void GrabObject(ObjectGrabbable newObjectGrabbable)
    {
        if (IsHoldingSomething)
            return;

        objectGrabbable = newObjectGrabbable;

        objectGrabbable.Grab(objectGrabPointTransform);
    }
}
