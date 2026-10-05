
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;

public class TerminalModeController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private FirstPersonController playerController;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera playerCamera;

    [Header("Navigation")]
    [SerializeField] private TerminalNavigationController navigationController;

    private bool isInTerminalMode;

    private int playerCameraPriority;

    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisibility;

    public bool IsInTerminalMode => isInTerminalMode;

    private void Awake()
    {
        playerCameraPriority = playerCamera.Priority.Value;
    }

    public void EnterTerminalMode()
    {
        if (isInTerminalMode)
            return;

        isInTerminalMode = true;

        previousCursorLockState = Cursor.lockState;
        previousCursorVisibility = Cursor.visible;

        playerController.SetControlsEnabled(false);

        navigationController.EnterNavigation();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitTerminalMode()
    {
        if (!isInTerminalMode)
            return;

        isInTerminalMode = false;

        navigationController.ExitNavigation();

        playerController.SetControlsEnabled(true);

        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisibility;
    }
}