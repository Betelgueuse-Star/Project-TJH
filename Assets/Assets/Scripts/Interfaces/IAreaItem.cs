// Interface para qualquer item que possa ser "usado" na PlantingArea (semente, saco de
// terra, fertilizante e itens futuros).
//
// Por que existe: antes a PlantingArea conhecia cada tipo de item e tinha um método Try...
// para cada um, quase iguais. Agora o item decide sozinho se pode ser aplicado
// (CanApply) e o que faz ao ser aplicado (Apply). Item novo = classe nova, sem editar
// a PlantingArea.
public interface IAreaItem
{
    // Devolve true se o item pode ser aplicado NESTE momento (ex.: semente só entra
    // se a área tem solo e está vazia).
    bool CanApply(PlantingArea area);

    // Aplica o efeito na área e destrói o próprio item.
    void Apply(PlantingArea area);
}


// ---- ObjectGrabbable ----
// A PlantingArea usa grabbable.IsBeingHeld. Se essa propriedade ainda não existir, adicione:
//   public bool IsBeingHeld { get; private set; }
// e coloque IsBeingHeld = true dentro de Grab(...) e IsBeingHeld = false dentro de Drop().
// ---------------------------------------------------------------------------------