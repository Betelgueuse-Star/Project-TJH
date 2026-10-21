using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Um tipo de item e quantas unidades dele estão esperando entrega.
// É só um pacote de dados: quem mostra na tela (texto do terminal) monta o texto como quiser.
public readonly struct CartEntry
{
    public readonly ItemTypeSO Item;
    public readonly int Amount;

    public CartEntry(ItemTypeSO item, int amount)
    {
        Item = item;
        Amount = amount;
    }
}

// Carrinho de compras do terminal.
// O Terminal cobra e manda o item para a fila daqui. O carrinho instancia os itens
// UM POR VEZ no ponto de entrega do elevador, mas só enquanto o elevador estiver em cima.
// Se o elevador estiver embaixo, os itens esperam na fila e a entrega começa sozinha
// quando o elevador disparar ReachedTop (o jogador manda o elevador subir manualmente).
public class ShoppingCart : MonoBehaviour
{
    [Header("Delivery")]
    [SerializeField] private Elevator elevator;

    [Tooltip("Tempo em segundos entre um item e outro. Evita itens nascendo sobrepostos.")]
    [SerializeField] private float spawnInterval = 0.3f;

    // Itens comprados que ainda não foram instanciados, na ordem da compra.
    private readonly Queue<ItemTypeSO> pendingItems = new Queue<ItemTypeSO>();

    private Coroutine deliveryCoroutine;
    private bool isConfigured;

    // Disparado sempre que a fila muda (item entrou ou saiu para entrega).
    // A UI do carrinho só precisa escutar este evento e chamar GetPendingEntries().
    public event Action OnCartChanged;

    public int PendingCount => pendingItems.Count;

    // O Terminal consulta isto ANTES de cobrar, para nunca gastar dinheiro
    // com um carrinho que não está funcionando.
    public bool IsReady => isConfigured;

    // Valida as referências UMA vez. Elevador faltando ou sem ponto de entrega
    // é erro de configuração: o carrinho nunca conseguiria entregar nada.
    private void Awake()
    {
        isConfigured = elevator != null;

        if (!isConfigured)
        {
            Debug.LogError($"{name}: Elevator faltando, o carrinho não vai funcionar.", this);
            return;
        }

        // Os campos serializados do Elevator já estão preenchidos aqui, mesmo que o
        // Awake dele ainda não tenha rodado.
        if (elevator.ItemSpawnPoint == null)
        {
            Debug.LogError($"{name}: o Elevator não tem itemSpawnPoint, o carrinho não vai funcionar.", this);
            isConfigured = false;
            return;
        }

        // Intervalo 0 faria os itens nascerem praticamente juntos e a física os empurrar.
        if (spawnInterval <= 0f)
            Debug.LogWarning($"{name}: spawnInterval é 0, os itens podem nascer sobrepostos.", this);
    }

    private void OnValidate()
    {
        spawnInterval = Mathf.Max(0f, spawnInterval);
    }

    private void OnEnable()
    {
        if (!isConfigured)
            return;

        // O jogador mandou o elevador subir: se tem itens esperando, a entrega começa.
        elevator.ReachedTop += HandleElevatorReachedTop;
    }

    private void OnDisable()
    {
        if (!isConfigured)
            return;

        elevator.ReachedTop -= HandleElevatorReachedTop;

        // Ao desativar o objeto o Unity para as corrotinas sozinho, mas a variável
        // continuaria guardando uma corrotina que não existe mais. Sem zerar aqui,
        // TryStartDelivery acharia que a entrega ainda está rodando e nunca mais começaria.
        deliveryCoroutine = null;
    }

    // Chamado pelo Terminal DEPOIS de cobrar a compra.
    public void Enqueue(ItemTypeSO item)
    {
        if (!isConfigured)
            return;

        if (item == null)
        {
            Debug.LogError($"{name}: tentaram colocar um item nulo no carrinho.", this);
            return;
        }

        pendingItems.Enqueue(item);
        OnCartChanged?.Invoke();

        // Se o elevador já está em cima, a entrega começa agora.
        // Se não, os itens esperam o ReachedTop.
        TryStartDelivery();
    }

    // Devolve os itens que ainda esperam entrega, agrupados por tipo e na ordem da primeira compra.
    // Ex.: comprou Semente, Pá, Semente -> [(Semente, 2), (Pá, 1)].
    // A UI monta o texto com entry.Item.ItemName e entry.Amount.
    // Cria uma lista nova a cada chamada: só deve ser usado quando OnCartChanged disparar,
    // não todo frame.
    public List<CartEntry> GetPendingEntries()
    {
        List<CartEntry> entries = new List<CartEntry>();

        foreach (ItemTypeSO item in pendingItems)
        {
            int foundIndex = -1;

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Item == item)
                {
                    foundIndex = i;
                    break;
                }
            }

            if (foundIndex >= 0)
                entries[foundIndex] = new CartEntry(item, entries[foundIndex].Amount + 1);
            else
                entries.Add(new CartEntry(item, 1));
        }

        return entries;
    }

    private void HandleElevatorReachedTop()
    {
        TryStartDelivery();
    }

    // Só começa se: está configurado, o objeto está ativo (StartCoroutine dá erro em objeto
    // inativo), não tem entrega em andamento, há itens na fila e o elevador está em cima.
    private void TryStartDelivery()
    {
        if (!isConfigured || !isActiveAndEnabled)
            return;

        if (deliveryCoroutine != null || pendingItems.Count == 0 || !elevator.IsAtTop)
            return;

        deliveryCoroutine = StartCoroutine(DeliveryRoutine());
    }

    // Instancia um item por vez, enquanto houver itens e o elevador continuar em cima.
    // Se o elevador sair antes de acabar, os itens que sobraram ficam na fila e a entrega
    // continua no próximo ReachedTop.
    private IEnumerator DeliveryRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while (pendingItems.Count > 0 && elevator.IsAtTop)
        {
            ItemTypeSO item = pendingItems.Dequeue();
            OnCartChanged?.Invoke();

            // O Terminal já valida o prefab antes de cobrar. Isto só evita travar a fila
            // inteira se algum dado ruim chegar aqui: o item é descartado e a entrega segue.
            if (item == null || item.ItemPrefab == null)
            {
                Debug.LogError($"{name}: item sem prefab na fila, foi descartado.", this);
                continue;
            }

            Transform spawnPoint = elevator.ItemSpawnPoint;

            Instantiate(
                item.ItemPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            yield return wait;
        }

        // Zerar aqui permite que um novo Enqueue (ou ReachedTop) comece outra entrega.
        deliveryCoroutine = null;
    }
}