using TMPro;
using UnityEngine;

// Canvas (World Space) do monitor único do terminal.
// Cuida da parte FIXA da tela, a que aparece em TODAS as abas (o cabeçalho).
// Hoje mostra só o dinheiro; dias, energia etc. entram aqui depois.

// COMO ADICIONAR UMA INFORMAÇÃO NOVA (dias, hora, energia...), sempre no mesmo padrão:
//   1. Campos [SerializeField]: o manager que tem o dado + o TextMeshProUGUI.
//   2. Um bool próprio (ex.: isDayConfigured) validado no Awake:
//      manager faltando = LogError, texto faltando = LogWarning.
//      Um bool por informação, assim um manager faltando não desliga o resto do cabeçalho.
//   3. Assinar o evento do manager no OnEnable e cancelar no OnDisable.
//   4. Um método UpdateXxxText próprio e uma chamada dele no RefreshAll.
public class MonitorCanvas : MonoBehaviour
{
    [Header("Currency")]
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private TextMeshProUGUI currencyText;

    // FUTURO (dias): mostrar o dia atual do jogo (ex.: "Day 12").
    // Precisa de um manager que conte os dias e dispare um evento quando o dia mudar.
    // [Header("Days")]
    // [SerializeField] private TimeManager timeManager;   // ainda não existe
    // [SerializeField] private TextMeshProUGUI dayText;
    // Evento esperado: timeManager.OnDayChanged (Action<int>, com o número do dia).

    // FUTURO (energia): mostrar a energia do bunker (o sistema de energia também vai
    // parar o enchimento da água e o crescimento das plantas quando acabar).
    // [Header("Energy")]
    // [SerializeField] private EnergyManager energyManager;   // ainda não existe
    // [SerializeField] private TextMeshProUGUI energyText;

    // FUTURO (notificações): indicador de mensagem nova no chat.
    // Pode usar o TerminalNavigationController.ShowScreen(índice) para abrir a aba do chat
    // quando o jogador clicar no indicador.

    private bool isCurrencyConfigured;

    // Valida as referências UMA vez, aqui, em vez de checar em cada uso.
    private void Awake()
    {
        isCurrencyConfigured = currencyManager != null;

        if (!isCurrencyConfigured)
            Debug.LogError($"{name}: CurrencyManager faltando, o dinheiro não vai aparecer no monitor.", this);

        if (currencyText == null)
            Debug.LogWarning($"{name}: currencyText faltando, o dinheiro não vai aparecer no monitor.", this);

        // FUTURO (dias): validar timeManager (erro) e dayText (aviso) aqui,
        // com um bool próprio (isDayConfigured).
    }

    private void OnEnable()
    {
        if (isCurrencyConfigured)
            currencyManager.OnCurrencyChanged += UpdateCurrencyText;

        // FUTURO (dias): timeManager.OnDayChanged += UpdateDayText;

        // Cobre qualquer mudança que aconteceu enquanto o canvas estava desativado.
        RefreshAll();
    }

    private void OnDisable()
    {
        if (isCurrencyConfigured)
            currencyManager.OnCurrencyChanged -= UpdateCurrencyText;

        // FUTURO (dias): timeManager.OnDayChanged -= UpdateDayText;
    }

    // Atualiza tudo de uma vez com os valores atuais.
    // Cada informação nova ganha uma linha aqui.
    private void RefreshAll()
    {
        if (isCurrencyConfigured)
            UpdateCurrencyText(currencyManager.CurrentCurrency);

        // FUTURO (dias): if (isDayConfigured) UpdateDayText(timeManager.CurrentDay);
    }

    private void UpdateCurrencyText(int amount)
    {
        if (currencyText == null)
            return;

        currencyText.text = $"Currency: {amount}";
    }

    // FUTURO (dias):
    // private void UpdateDayText(int day)
    // {
    //     if (dayText == null)
    //         return;
    //
    //     dayText.text = $"Day {day}";
    // }
}