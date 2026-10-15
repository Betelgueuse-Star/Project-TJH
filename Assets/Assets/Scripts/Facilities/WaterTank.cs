using TMPro;
using UnityEngine;

public class WaterTank : MonoBehaviour, IInteractable, IConditionalInteractable
{
    [SerializeField] private TextMeshProUGUI waterAmountText;
    [SerializeField] private float maxWater = 100f;

    [SerializeField] private float currentWater;

    private void Awake()
    {
        currentWater = maxWater;
        waterAmountText.text = currentWater.ToString();
    }

    public float TakeWater(float amount)
    {
        float waterTaken = Mathf.Min(amount, currentWater);

        currentWater -= waterTaken;
        waterAmountText.text = currentWater.ToString();

        return waterTaken;
    }

    // MUDANÇA: no lugar do CanInteract que devolvia (bool, float). Agora é uma comparação
    // só: se o jogador está com o regador, a interação disponível é encher o regador.
    // Sem TryGetComponent aqui (o Player já sabe o tipo da ferramenta).
    public InteractionType GetInteraction(Player player)
    {
        if (player.CurrentToolType == ToolType.WateringCan)
            return InteractionType.FillWateringCan;

        return InteractionType.None;
    }

    public void Interact(Player player)
    {
        // MUDANÇA: antes pegava o (canInteract, interactionValue) e comparava com 2f.
        if (GetInteraction(player) != InteractionType.FillWateringCan)
            return;

        // Só aqui, na hora de realmente interagir, precisamos do componente do regador.
        if (player.TryGetCurrentTool(out WateringCan wateringCan))
        {
            // FUTURO (animação): wateringCan.PlayUse();
            wateringCan.FillFromTank(this);
        }
    }
}