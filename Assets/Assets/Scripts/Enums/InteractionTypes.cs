// Qual ferramenta o jogador está segurando.
// MUDANÇA: antes cada script descobria isso com TryGetComponent (Shovel, WateringCan...)
// em todo CanInteract. Agora o Player guarda o tipo uma única vez, ao pegar a ferramenta,
// e todo mundo só compara este valor.
// Ferramenta nova = adicionar uma linha aqui.
public enum ToolType
{
    None = 0,
    Shovel = 1,
    WateringCan = 2
}

// O que acontece quando o jogador interage com algo, neste momento.
// MUDANÇA: substitui a tupla (bool, float) do IConditionalInteractable. O "bool" virou
// "None" (não dá para interagir) e o "float" (1f, 2f, números mágicos que se repetiam
// com significados diferentes) virou um nome claro. Um único valor diz tudo.
// Interação nova = adicionar uma linha aqui.
public enum InteractionType
{
    None = 0,
    RemoveAll = 1,       // pá na PlantingArea
    Water = 2,           // regador na PlantingArea
    FillWateringCan = 3  // regador no WaterTank
}