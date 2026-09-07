using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUpDrop : MonoBehaviour
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
                if (raycastHitInfo.transform.TryGetComponent(out objectGrabbable)) //atribui o objeto que foi pego para a variavel objectGrabbable
                {
                    objectGrabbable.Grab(objectGrabPointGameObject.transform);
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
