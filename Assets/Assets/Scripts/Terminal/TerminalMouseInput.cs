
using UnityEngine;
using UnityEngine.InputSystem;

public class TerminalMouseInput : MonoBehaviour
{
    [SerializeField] private TerminalModeController terminalMode;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask colliderbuttonMask;
    [SerializeField] private float maxDistance = 5f;

    private void Update()
    {
        if (!terminalMode.IsInTerminalMode)
            return;

        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = playerCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, colliderbuttonMask))
        {
            TerminalButton button =
                hit.collider.GetComponent < TerminalButton>();

            if (button != null)
                button.Click();
        }
    }
}