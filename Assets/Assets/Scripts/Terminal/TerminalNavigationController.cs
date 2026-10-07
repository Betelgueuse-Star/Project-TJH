using System;
using Unity.Cinemachine;
using UnityEngine;

public class TerminalNavigationController : MonoBehaviour
{
    [Serializable]
    private class TerminalScreen
    {
        public CinemachineCamera camera;
        public GameObject interfaceRoot;
        public Camera terminalUICamera;
    }


[Header("Screens")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private TerminalScreen[] screens;

    private int currentScreenIndex;
    private int playerCameraPriority;
    private int[] originalPriorities;

    public int CurrentScreenIndex => currentScreenIndex;

    private void Awake()
    {
        playerCameraPriority = playerCamera.Priority.Value;
        originalPriorities = new int[screens.Length];

        for (int i = 0; i < screens.Length; i++)
        {
            originalPriorities[i] = screens[i].camera.Priority.Value;

            screens[i].interfaceRoot.SetActive(false);
            screens[i].terminalUICamera.enabled = false;

            ClearTargetTexture(screens[i].terminalUICamera);
        }
    }

    public void EnterNavigation()
    {
        if (screens.Length == 0)
            return;

        currentScreenIndex = 0;
        ActivateScreen(currentScreenIndex);
    }

    public void ExitNavigation()
    {
        ShowInterface(-1);

        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].camera.Priority = originalPriorities[i];
        }
    }

    public void NextScreen()
    {
        currentScreenIndex++;

        if (currentScreenIndex >= screens.Length)
            currentScreenIndex = 0;

        ActivateScreen(currentScreenIndex);
    }

    public void PreviousScreen()
    {
        currentScreenIndex--;

        if (currentScreenIndex < 0)
            currentScreenIndex = screens.Length - 1;

        ActivateScreen(currentScreenIndex);
    }

    private void ActivateScreen(int index)
    {
        ShowInterface(index);

        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].camera.Priority =
                i == index
                    ? playerCameraPriority + 1
                    : playerCameraPriority - 1;
        }
    }

    private void ShowInterface(int index)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            TerminalScreen screen = screens[i];
            bool isActive = i == index;

            screen.interfaceRoot.SetActive(isActive);

            if (isActive)
            {
                screen.terminalUICamera.enabled = true;
            }
            else
            {
                ClearTargetTexture(screen.terminalUICamera);
                screen.terminalUICamera.enabled = false;
            }
        }
    }

    private void ClearTargetTexture(Camera targetCamera)
    {
        RenderTexture target = targetCamera.targetTexture;

        if (target == null)
            return;

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = target;

        GL.Clear(true, true, Color.black);

        RenderTexture.active = previous;
    }


}
