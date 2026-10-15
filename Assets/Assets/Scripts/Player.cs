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

    // MUDANÇA: antes eram três campos para a ferramenta (currentTool como GameObject,
    // currentToolWorldPrefab e currentToolComponent). Agora é UM só: a própria ferramenta
    // (componente Tool). O GameObject e o tipo saem dela; não há mais nada para sincronizar.
    private Tool currentTool;

    private ObjectGrabbable objectGrabbable; // Objeto físico que está sendo carregado.
    private InteractableOutline currentOutline; // Outline que está aceso no momento.
    private RaycastHit currentHit;
    private bool hasHit;
    private Collider lastCollider;

    private bool canInteract;
    private bool targetIsPickup;        // ObjectGrabbable ou Tool
    private bool targetIsInteractable;  // IInteractable

    // MUDANÇA: o tipo agora vem direto do currentTool.
    public ToolType CurrentToolType =>
        currentTool != null ? currentTool.Type : ToolType.None;

    // MUDANÇA: devolve o componente Tool em vez do GameObject. Código que fazia
    // CurrentTool.TryGetComponent / GetComponent continua compilando (Tool é um Component).
    // Se algum script usa "CurrentTool" como GameObject, troque por CurrentTool.gameObject.
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

        // Ao pegar um item, o outline some; ao largar, volta
        if (IsHoldingSomething)
            HideCurrentOutline();
    }

    private void SetCanInteract(bool value)
    {
        if (canInteract == value)
            return; // só dispara o evento quando o estado muda

        canInteract = value;
        OnCanInteractChanged?.Invoke(canInteract);
    }

    private void HideCurrentOutline()
    {
        // "== null" do Unity também cobre objetos já destruídos.
        if (currentOutline == null)
            return;

        currentOutline.Hide();
        currentOutline = null;
    }

    private void DropCurrentItem()
    {
        // MUDANÇA: zera o cache do alvo para o outline do objeto que você está olhando
        // voltar a acender logo depois de largar o item.
        lastCollider = null;

        // Caso 1: objeto físico. Basta soltar.
        if (objectGrabbable != null)
        {
            objectGrabbable.Drop();
            objectGrabbable = null;
            return;
        }

        // Caso 2: ferramenta.
        // MUDANÇA: antes recriava o prefab "do mundo" (Instantiate) e destruía o da mão.
        // Agora é o MESMO objeto: só o devolvemos ao mundo. Sem Instantiate/Destroy e sem
        // depender de referência de prefab (que se perdia ao dropar).
        if (currentTool != null)
        {
            currentTool.Drop(toolHoldPointTransform.position, transform.rotation);
            currentTool = null;
        }
    }

    // MUDANÇA: antes guardava o prefab "do mundo", criava a versão "equipada" na mão e
    // destruía o objeto do mundo. Agora só coloca a própria ferramenta na mão.
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

    // Pega a ferramenta como um tipo específico (ex.: WateringCan), sem TryGetComponent
    // MUDANÇA: T agora é Tool (antes EquippedTool) e o cast é feito direto no currentTool.
    public bool TryGetCurrentTool<T>(out T tool) where T : Tool
    {
        tool = currentTool as T;
        return tool != null;
    }
}