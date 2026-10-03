
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Cinemachine;

public class ComputerScreenInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ComputerModeController computerMode;
    [SerializeField] private CinemachineCamera computerCamera;
    [SerializeField] private Camera computerUICamera;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private RenderTexture screenTexture;

    [Header("Raycast")]
    [SerializeField] private float maxDistance = 5f;

    private Collider screenCollider;
    private EventSystem eventSystem;

    private PointerEventData pointerData;
    private readonly List<RaycastResult> raycastResults = new();

    private GameObject currentTarget;
    private GameObject pressedTarget;
    private GameObject pressedClickHandler;

    private void Awake()
    {
        screenCollider = GetComponent<Collider>();
        eventSystem = EventSystem.current;

        if (screenCollider == null)
            Debug.LogError("ComputerScreenInput requires a Collider.", this);

        if (eventSystem == null)
            Debug.LogError("No EventSystem found in the scene.", this);

        if (graphicRaycaster != null && eventSystem != null)
            pointerData = new PointerEventData(eventSystem);
    }

    private void Update()
    {
        if (!computerMode.IsInComputerMode)
        {
            ClearHover();
            return;
        }

        if (Mouse.current == null || Camera.main == null)
            return;

        if (screenCollider == null ||
            graphicRaycaster == null ||
            screenTexture == null ||
            pointerData == null)
            return;

        UpdatePointer();
    }

    private void UpdatePointer()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        bool hitScreen =
            screenCollider.Raycast(ray, out RaycastHit hit, maxDistance);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log($"Screen hit: {hitScreen}");

            if (hitScreen)
                Debug.Log($"UV: {hit.textureCoord}");
        }

        if (!hitScreen)
        {
            ClearHover();
            HandleMouseRelease();
            return;
        }

        Vector2 uv = hit.textureCoord;
        Debug.Log($"UV do monitor: {uv}");

        pointerData.position = new Vector2(
            uv.x * screenTexture.width,
            uv.y * screenTexture.height
        );

        raycastResults.Clear();
        graphicRaycaster.Raycast(pointerData, raycastResults);

        Debug.Log(
    $"Pointer: {pointerData.position} | " +
    $"Texture: {screenTexture.width}x{screenTexture.height} | " +
    $"UI Camera: {graphicRaycaster.eventCamera}"
);

        GameObject target = raycastResults.Count > 0
            ? raycastResults[0].gameObject
            : null;

        pointerData.pointerCurrentRaycast =
            raycastResults.Count > 0
                ? raycastResults[0]
                : new RaycastResult();

        UpdateHover(target);
        HandleMousePress(target);
        HandleMouseRelease();
    }

    private void UpdateHover(GameObject target)
    {
        if (currentTarget == target)
            return;

        if (currentTarget != null)
        {
            ExecuteEvents.Execute(
                currentTarget,
                pointerData,
                ExecuteEvents.pointerExitHandler
            );
        }

        currentTarget = target;
        pointerData.pointerEnter = target;

        if (currentTarget != null)
        {
            ExecuteEvents.Execute(
                currentTarget,
                pointerData,
                ExecuteEvents.pointerEnterHandler
            );
        }
    }

    private void HandleMousePress(GameObject target)
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        pressedTarget = target;

        pressedClickHandler = target != null
            ? ExecuteEvents.GetEventHandler<IPointerClickHandler>(target)
            : null;

        pointerData.pointerPressRaycast =
            pointerData.pointerCurrentRaycast;

        pointerData.pointerPress = target != null
            ? ExecuteEvents.ExecuteHierarchy(
                target,
                pointerData,
                ExecuteEvents.pointerDownHandler
            )
            : null;

        if (pointerData.pointerPress == null)
        {
            pointerData.pointerPress = pressedClickHandler;
        }

        pointerData.rawPointerPress = target;
    }

    private void HandleMouseRelease()
    {
        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasReleasedThisFrame)
            return;

        if (pointerData.pointerPress != null)
        {
            ExecuteEvents.Execute(
                pointerData.pointerPress,
                pointerData,
                ExecuteEvents.pointerUpHandler
            );
        }

        GameObject clickHandler = currentTarget != null
            ? ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentTarget)
            : null;

        if (pressedClickHandler != null &&
            pressedClickHandler == clickHandler)
        {
            ExecuteEvents.Execute(
                pressedClickHandler,
                pointerData,
                ExecuteEvents.pointerClickHandler
            );
        }

        pointerData.pointerPress = null;
        pointerData.rawPointerPress = null;

        pressedTarget = null;
        pressedClickHandler = null;
    }

    private void ClearHover()
    {
        if (currentTarget != null && pointerData != null)
        {
            ExecuteEvents.Execute(
                currentTarget,
                pointerData,
                ExecuteEvents.pointerExitHandler
            );
        }

        currentTarget = null;

        if (pointerData != null)
            pointerData.pointerEnter = null;
    }
}