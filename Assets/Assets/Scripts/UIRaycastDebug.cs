
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIRaycastDebug : MonoBehaviour
{
    private readonly List<RaycastResult> results = new();

    private void Update()
    {
        if (Mouse.current == null || EventSystem.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        PointerEventData pointerData = new PointerEventData(
            EventSystem.current
        )
        {
            position = Mouse.current.position.ReadValue()
        };

        results.Clear();
        EventSystem.current.RaycastAll(pointerData, results);

        StringBuilder message = new StringBuilder();
        message.AppendLine($"UI Raycast hits: {results.Count}");

        foreach (RaycastResult result in results)
        {
            message.AppendLine(
                $"{result.gameObject.name} | " +
                $"Module: {result.module?.GetType().Name} | " +
                $"Depth: {result.depth}"
            );
        }

        Debug.Log(message.ToString());
    }
}
