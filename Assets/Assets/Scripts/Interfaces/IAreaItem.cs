// Interface para qualquer item que possa ser "usado" na PlantingArea
public interface IAreaItem
{
    // Devolve true se o item pode ser aplicado NESTE momento (ex.: semente só entra
    // se a área tem solo e está vazia).
    bool CanApply(PlantingArea area);

    // Aplica o efeito na área e destrói o próprio item.
    void Apply(PlantingArea area);
}