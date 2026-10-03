
using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerMouseInput : MonoBehaviour
{
    [SerializeField] private TerminalModeController terminalMode;
    [SerializeField] private Camera playerCamera;
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

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            ComputerButton button =
                hit.collider.GetComponent<ComputerButton>();

            if (button != null)
                button.Click();
        }
    }
}