// MUDANÇA: herda de Tool (a classe base única das ferramentas, que cuida de pegar e
// largar) e declara o próprio tipo. Antes a classe era vazia e servia só de "etiqueta"
// para o TryGetComponent<Shovel>().
public class Shovel : Tool
{
    public override ToolType Type => ToolType.Shovel;
}