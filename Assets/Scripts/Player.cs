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

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;
        if (objectGrabbable == null)//nao esta carregando um objeto
        {
            if (Physics.Raycast(playerCameraGameObject.transform.position, playerCameraGameObject.transform.forward, out RaycastHit raycastHitInfo, pickUpRange, pickUpLayerMask))
            {
                Debug.Log("Hit object: " + raycastHitInfo.transform.name);
                if (raycastHitInfo.transform.TryGetComponent(out objectGrabbable)) //atribui o objeto que foi pego para a variavel objectGrabbable
                {
                    objectGrabbable.Grab(objectGrabPointGameObject.transform);
                }
                if (raycastHitInfo.transform.TryGetComponent(out  IInteractable interactable))//busca se o scriptpossui a interface IInteractable
                {
                    interactable.Interact();
                }
            }
        }
        else
        {
            //atualmente segurando algo
            objectGrabbable.Drop();
            objectGrabbable = null; //limpa o campo
        }
    }
}
