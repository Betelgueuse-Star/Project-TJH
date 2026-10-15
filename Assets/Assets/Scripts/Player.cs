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

    private GameObject currentTool; // Ferramenta atualmente na mão e o prefab "do mundo" correspondente
    private GameObject currentToolWorldPrefab; // usado para recriar o objeto no chão quando o jogador larga a ferramenta
    private ObjectGrabbable objectGrabbable; // Objeto físico que está sendo carregado.
    private InteractableOutline currentOutline; // Outline que está aceso no momento.
    private RaycastHit currentHit;
    private bool hasHit;
    private Collider lastCollider;

    private bool canInteract;
    private bool targetIsPickup;        // ObjectGrabbable ou Tool
    private bool targetIsInteractable;  // IInteractable

    public GameObject CurrentTool => currentTool;
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
        // Caso 1: objeto físico. Basta soltar.
        if (objectGrabbable != null)
        {
            objectGrabbable.Drop();
            objectGrabbable = null;
            return;
        }

        // Caso 2: ferramenta. Recria a versão "do mundo" e destrói a da mão.
        if (currentTool != null)
        {
            if (currentToolWorldPrefab != null)
            {
                Instantiate(
                    currentToolWorldPrefab,
                    toolHoldPointTransform.position,
                    transform.rotation);
            }
            else
            {
                Debug.LogWarning($"A ferramenta {currentTool.name} está sem WorldPrefab.", currentTool);
            }

            Destroy(currentTool);
            currentTool = null;

            currentToolWorldPrefab = null; // limpa a referência para não ficar um prefab antigo guardado.
        }
    }

    // Guarda o prefab "do mundo", cria a versão "equipada" na mão
    // e destrói o objeto que estava no mundo.
    private void GrabTool(Tool newTool)
    {
        if (IsHoldingSomething)
            return;

        // apaga o outline ANTES de destruir o objeto,
        // evitando referência para algo destruído.
        lastCollider = null;
        HideCurrentOutline();

        currentToolWorldPrefab = newTool.WorldPrefab;

        currentTool = Instantiate(newTool.EquippedPrefab, toolHoldPointTransform);

        currentTool.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        Destroy(newTool.gameObject);
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
}