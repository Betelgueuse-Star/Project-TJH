using UnityEngine;
using UnityEngine.Events;

public class QuickInteract : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private UnityEvent onInteract; //funciona que nem os botoes, voce pode escolher o metodo que quer chamar no inspetor,
                                                    //e ele vai chamar quando o jogador interagir com o objeto

    public void Interact(Player player)
    {
        onInteract?.Invoke();
    }
}