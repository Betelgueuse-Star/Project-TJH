using System;
using UnityEngine;

// Controla as abas do terminal (monitor único): só uma aba fica visível por vez
// e a troca é instantânea, sem câmera e sem animação.
// Este script não sabe o que existe dentro de cada aba, ele só mostra e esconde os painéis.
public class TerminalNavigationController : MonoBehaviour
{
    [Header("Abas (a ordem aqui é o índice usado pelos botões)")]
    [SerializeField] private TerminalScreen[] screens;

    [Tooltip("Aba aberta na primeira vez que o terminal é usado")]
    [SerializeField] private int defaultScreenIndex;

    private TerminalScreen visibleScreen;
    private int currentScreenIndex;
    private bool isOpen;

    public int CurrentScreenIndex => currentScreenIndex;

    // Disparado ao trocar de aba (útil para destacar o botão da aba atual na barra de abas).
    public event Action<int> OnScreenChanged;

    private void Awake()
    {
        currentScreenIndex = Mathf.Clamp(defaultScreenIndex, 0, Mathf.Max(0, screens.Length - 1));

        // Começa com todas as abas fechadas.
        // Não chama Hide() porque essas abas nunca foram mostradas (não houve OnShow).
        for (int i = 0; i < screens.Length; i++)
        {
            if (screens[i] != null)
                screens[i].gameObject.SetActive(false);
        }
    }

    // Chamado pelo TerminalModeController ao entrar no terminal.
    // Reabre na última aba usada.
    public void EnterNavigation()
    {
        if (isOpen || screens.Length == 0)
            return;

        isOpen = true;

        DisplayScreen(currentScreenIndex);
    }

    // Chamado pelo TerminalModeController ao sair do terminal.
    // O índice da aba atual é mantido para a próxima vez.
    public void ExitNavigation()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (visibleScreen != null)
        {
            visibleScreen.Hide();
            visibleScreen = null;
        }
    }

    // Chamado pelos botões da barra de abas (Button > OnClick, com o índice da aba como argumento).
    // Também serve para abrir uma aba específica por código (ex.: notificação do chat).
    public void ShowScreen(int index)
    {
        if (!IsValidIndex(index))
        {
            Debug.LogWarning($"{name}: aba {index} inválida ou não configurada.", this);
            return;
        }

        // Terminal fechado: só guarda qual aba deve abrir na próxima entrada.
        if (!isOpen)
        {
            currentScreenIndex = index;
            return;
        }

        DisplayScreen(index);
    }

    private void DisplayScreen(int index)
    {
        if (!IsValidIndex(index))
            return;

        TerminalScreen target = screens[index];
        currentScreenIndex = index;

        // Já é a aba visível: nada a fazer.
        if (target == visibleScreen)
            return;

        if (visibleScreen != null)
            visibleScreen.Hide();

        visibleScreen = target;
        visibleScreen.Show();

        OnScreenChanged?.Invoke(index);
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < screens.Length && screens[index] != null;
    }
}