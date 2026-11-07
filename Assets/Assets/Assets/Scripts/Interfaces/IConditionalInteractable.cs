public interface IConditionalInteractable
{
    // Devolve um único InteractionType: None = não dá para interagir; qualquer
    // outro valor = qual interação está disponível agora (útil também para a UI mostrar
    // um ícone diferente por tipo de interação).
    InteractionType GetInteraction(Player player);
}