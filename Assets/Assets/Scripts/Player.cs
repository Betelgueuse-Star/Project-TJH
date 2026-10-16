using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public event Action<bool> OnCanInteractChanged;

    [Header("Referências")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private Transform toolHoldPointTransform;

    [Header("Interação")]
    [SerializeField] private LayerMask interactionLayerMask;
    [SerializeField] private float interactionRange = 3f;

    private Tool currentTool;

    private ObjectGrabbable objectGrabbable; // Objeto físico que está sendo carregado.
    private InteractableOutline currentOutline; // Outline que está aceso no momento.

    private RaycastHit currentHit;
    private bool hasHit;

    private Collider lastCollider;

    private bool canInteract;
    private bool targetIsPickup;        // ObjectGrabbable ou Tool
    private bool targetIsInteractable;  // IInteractable

    public ToolType CurrentToolType => currentTool != null ? currentTool.Type : ToolType.None;
    public Tool CurrentTool => currentTool;
    public bool IsHoldingSomething => objectGrabbable != null || currentTool != null;


    private void Update()
    {
        hasHit = Physics.Raycast(
            cameraTransform.position,
            cameraTransform.forward,
            out currentHit,
            interactionRange,
            interactionLayerMask);

        UpdateTarget();
    }

    private void UpdateTarget()
    {
        if (!hasHit)
        {
            lastCollider = null;
            targetIsPickup = false;
            targetIsInteractable = false;
            HideCurrentOutline();
            SetCanInteract(false);
            return;
        }

        // Só busca os componentes quando muda de alvo
        if (currentHit.collider != lastCollider)
        {
            lastCollider = currentHit.collider;
            Transform t = currentHit.transform;

            targetIsPickup = t.TryGetComponent(out ObjectGrabbable _) || t.TryGetComponent(out Tool _);
            targetIsInteractable = t.TryGetComponent(out IInteractable _);

            HideCurrentOutline();

            if (!IsHoldingSomething && t.TryGetComponent(out InteractableOutline outline))
            {
                currentOutline = outline;
                currentOutline.Show();
            }
        }

        // Segurando algo, pegar outra coisa não é possível, mas IInteractable ainda é
        bool pickupAvailable = targetIsPickup && !IsHoldingSomething;
        SetCanInteract(pickupAvailable || targetIsInteractable);

        if (IsHoldingSomething) // Esonde a outline caso esteja segurando algo
            HideCurrentOutline();
    }

    private void SetCanInteract(bool value)
    {
        if (canInteract == value) // só dispara o evento quando o estado muda
            return; 

        canInteract = value;
        OnCanInteractChanged?.Invoke(canInteract);
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
        lastCollider = null;

        // Caso 1: objeto físico
        if (objectGrabbable != null)
        {
            objectGrabbable.Drop();
            objectGrabbable = null;
            return;
        }

        // Caso 2: ferramenta
        if (currentTool != null)
        {
            currentTool.Drop(toolHoldPointTransform.position, transform.rotation);
            currentTool = null;
        }
    }

    private void GrabTool(Tool newTool)
    {
        if (IsHoldingSomething)
            return;

        // apaga o outline antes de a ferramenta ir para a mão (o collider dela é desligado).
        lastCollider = null;
        HideCurrentOutline();

        newTool.Equip(toolHoldPointTransform);
        currentTool = newTool;
    }

    public void GrabObject(ObjectGrabbable newObjectGrabbable)
    {
        if (IsHoldingSomething)
            return;

        lastCollider = null;
        HideCurrentOutline();

        objectGrabbable = newObjectGrabbable;
        objectGrabbable.Grab(objectGrabPointTransform);
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (!hasHit)
            return;

        Transform target = currentHit.transform;

        // A ordem importa: objeto pegável > ferramenta > interagível genérico.
        if (target.TryGetComponent(out ObjectGrabbable grabbable))
        {
            GrabObject(grabbable);
        }
        else if (target.TryGetComponent(out Tool tool))
        {
            GrabTool(tool);
        }
        else if (target.TryGetComponent(out IInteractable interactable))
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

    // Pega a ferramenta como um tipo específico com generic (ex.: WateringCan)
    public bool TryGetCurrentTool<T>(out T tool) where T : Tool
    {
        tool = currentTool as T;
        return tool != null;
    }
}