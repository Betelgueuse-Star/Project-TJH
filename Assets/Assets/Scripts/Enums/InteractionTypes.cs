
// Qual ferramenta o jogador está segurando.
public enum ToolType
{
    None = 0,
    Shovel = 1,
    WateringCan = 2
}

// O que acontece quando o jogador interage com algo, neste momento.
public enum InteractionType
{
    None = 0,
    RemoveAll = 1,       // pá na PlantingArea
    Water = 2,           // regador na PlantingArea
    FillWateringCan = 3  // regador no WaterTank
}