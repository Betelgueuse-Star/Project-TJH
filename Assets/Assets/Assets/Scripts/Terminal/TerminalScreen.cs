using UnityEngine;

// Base de toda aba do terminal (Loja, Pedidos, Água, Energia, Chat, Mapa...).
// O script que herda desta classe deve ficar no objeto RAIZ do painel da aba,
// porque é esse objeto que o TerminalNavigationController liga e desliga.
//
// Em vez de OnEnable/OnDisable, cada aba usa OnShow/OnHide:
//   OnShow -> assina eventos e atualiza tudo uma vez (cobre mudanças feitas com a aba fechada).
//   OnHide -> cancela as assinaturas de eventos.
public abstract class TerminalScreen : MonoBehaviour
{
    // Chamados somente pelo TerminalNavigationController.
    public void Show()
    {
        gameObject.SetActive(true);
        OnShow();
    }

    public void Hide()
    {
        OnHide();
        gameObject.SetActive(false);
    }

    protected virtual void OnShow() { }

    protected virtual void OnHide() { }
}