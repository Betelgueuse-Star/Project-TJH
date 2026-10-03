
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerModeController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private FirstPersonController playerController;
    

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera computerCamera;

    private bool isInComputerMode;

    private int playerCameraPriority;
    private int computerCameraPriority;

    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisibility;

    public bool IsInComputerMode => isInComputerMode;

    private void Awake()
    {
        playerCameraPriority = playerCamera.Priority.Value;
        computerCameraPriority = computerCamera.Priority.Value;
    }
    public void OnInteract(InputValue value)
    {

       EnterComputerMode();
    }

    public void OnDrop(InputValue value)
    {

        ExitComputerMode();
    }

    public void EnterComputerMode()
    {
        if (isInComputerMode)
            return;

        isInComputerMode = true;

        previousCursorLockState = Cursor.lockState;
        previousCursorVisibility = Cursor.visible;

        playerController.SetControlsEnabled(false);

        computerCamera.Priority = playerCameraPriority + 1;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Entered computer mode.");
    }

    public void ExitComputerMode()
    {
        if (!isInComputerMode)
            return;

        isInComputerMode = false;

        computerCamera.Priority = computerCameraPriority;

        playerController.SetControlsEnabled(true);

        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisibility;
    }
}