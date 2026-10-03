using UnityEngine;

public class Computer : MonoBehaviour, IInteractable
{
    [SerializeField] private TerminalModeController terminalModeController;

    public void EnterTerminalMode()
    {
        terminalModeController.EnterTerminalMode();
    }
    public void ExitTerminalMode()
    {
        terminalModeController.ExitTerminalMode();
    }

    public void BuySeed()
    {
        // Implement the logic to buy a seed here
        Debug.Log("Seed purchased!");
    }

    public void Interact(Player player)
    {
        if (player.IsHoldingSomething)
            return;
        EnterTerminalMode();
    }
}

