using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

// Entra e sai do modo terminal: trava o jogador, troca para a câmera do monitor,
// libera o cursor e liga os cliques do Canvas do monitor.
// Quem cuida das abas é o TerminalNavigationController.
public class TerminalModeController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private CinemachineCamera playerCamera;

    [Header("Terminal")]
    [SerializeField] private CinemachineCamera terminalCamera;

    [Tooltip("GraphicRaycaster do Canvas (World Space) do monitor")]
    [SerializeField] private GraphicRaycaster screenRaycaster;

    [SerializeField] private TerminalNavigationController navigationController;

    private bool isInTerminalMode;
    private bool isConfigured;

    // Prioridade da câmera do terminal fora do modo terminal (deve ser menor que a do jogador).
    private int terminalCameraRestPriority;

    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisibility;

    public bool IsInTerminalMode => isInTerminalMode;

    private void Awake()
    {
        isConfigured =
            playerController != null &&
            playerCamera != null &&
            terminalCamera != null &&
            screenRaycaster != null &&
            navigationController != null;

        if (!isConfigured)
        {
            Debug.LogError($"{name}: referência faltando, o modo terminal não vai funcionar.", this);
            return;
        }

        terminalCameraRestPriority = terminalCamera.Priority.Value;

        // Fora do modo terminal o cursor fica travado no centro da tela.
        // Com o raycaster ligado, um clique olhando para o monitor de longe acertaria um botão.
        screenRaycaster.enabled = false;
    }

    public void EnterTerminalMode()
    {
        if (!isConfigured || isInTerminalMode)
            return;

        isInTerminalMode = true;

        previousCursorLockState = Cursor.lockState;
        previousCursorVisibility = Cursor.visible;

        playerController.SetControlsEnabled(false);

        // O CinemachineBrain faz o blend até a câmera do monitor.
        terminalCamera.Priority = playerCamera.Priority.Value + 1;

        navigationController.EnterNavigation();
        screenRaycaster.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitTerminalMode()
    {
        if (!isConfigured || !isInTerminalMode)
            return;

        isInTerminalMode = false;

        screenRaycaster.enabled = false;
        navigationController.ExitNavigation();

        terminalCamera.Priority = terminalCameraRestPriority;

        playerController.SetControlsEnabled(true);

        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisibility;
    }
}