
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;

public class TerminalModeController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private FirstPersonController playerController;
    

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera computerCamera;

    private bool isInTerminalMode;

    private int playerCameraPriority;
    private int computerCameraPriority;

    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisibility;

    public bool IsInTerminalMode => isInTerminalMode;

    private void Awake()
    {
        playerCameraPriority = playerCamera.Priority.Value;
        computerCameraPriority = computerCamera.Priority.Value;
    }

    public void EnterTerminalMode()
    {
        if (isInTerminalMode)
            return;

        isInTerminalMode = true;

        previousCursorLockState = Cursor.lockState;
        previousCursorVisibility = Cursor.visible;

        playerController.SetControlsEnabled(false);

        computerCamera.Priority = playerCameraPriority + 1;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitTerminalMode()
    {
        if (!isInTerminalMode)
            return;

        isInTerminalMode = false;

        computerCamera.Priority = computerCameraPriority;

        playerController.SetControlsEnabled(true);

        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisibility;
    }
}