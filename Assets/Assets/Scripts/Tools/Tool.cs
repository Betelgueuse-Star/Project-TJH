using UnityEngine;

// MUDANÇA: esta classe substitui DOIS scripts antigos: o Tool (que guardava worldPrefab e
// equippedPrefab) e o EquippedTool. Agora a ferramenta do mundo e a ferramenta da mão são
// o MESMO GameObject. Ao pegar, ele vira filho da mão; ao largar, volta para o mundo.
//
// Por que é melhor:
// - Não há Instantiate/Destroy ao pegar e largar.
// - Não existem dois prefabs para manter sincronizados, nem referência de prefab
//   para si mesmo (que causava o erro ao dropar).
// - O estado da ferramenta é mantido (ex.: a água do regador não zera ao largar).
//
// Shovel e WateringCan herdam desta classe e só declaram o próprio Type.
//
// ESTRUTURA DO PREFAB (uma ferramenta = um prefab só):
//   Ferramenta (raiz)  -> Shovel/WateringCan, Rigidbody, Collider, InteractableOutline
//    |- WorldVisual    -> modelo que aparece no chão (arraste em "World Visual")
//    |- HeldVisual     -> modelo que aparece na mão; no futuro leva o Animator e a UI
//                         (arraste em "Held Visual")
// O Collider fica na RAIZ (nunca no HeldVisual). Assim o raycast do Player sempre
// encontra o componente Tool na raiz, e o Equip/Drop só alterna qual visual está ativo:
//   no mundo -> WorldVisual ligado, HeldVisual desligado
//   na mão   -> WorldVisual desligado, HeldVisual ligado
public abstract class Tool : MonoBehaviour
{
    public abstract ToolType Type { get; }

    [Header("Pose na mão")]
    // Posição/rotação local em relação ao ponto da mão. Ajuste por ferramenta.
    [SerializeField] private Vector3 heldLocalPosition;
    [SerializeField] private Vector3 heldLocalEulerAngles;

    [Header("Visuais (opcional)")]
    // Se o modelo na mão for diferente do modelo no chão, coloque os dois como filhos
    // e arraste aqui. Se for o mesmo modelo, deixe vazio.
    [SerializeField] private GameObject worldVisual;
    [SerializeField] private GameObject heldVisual;

    // FUTURO (animações): descomente quando as animações existirem.
    // O Animator deve ficar no filho HeldVisual (nunca na raiz), senão a animação briga
    // com a pose definida no Equip. Cada ferramenta usa o próprio Animator Controller.
    // [Header("Animação (futuro)")]
    // [SerializeField] private Animator animator;
    // private static readonly int UseHash = Animator.StringToHash("Use");
    //
    // // Chamado por quem usa a ferramenta (PlantingArea, WaterTank...). Veja as linhas
    // // "FUTURO (animação)" comentadas nesses scripts.
    // public void PlayUse()
    // {
    //     if (animator != null)
    //         animator.SetTrigger(UseHash);
    // }

    private Rigidbody rb;
    private Collider[] colliders;

    // "protected virtual" para que Shovel/WateringCan possam ter o próprio Awake,
    // chamando base.Awake().
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        SetHeldVisual(false); // começa no mundo
    }

    // Chamado pelo Player ao pegar a ferramenta.
    public void Equip(Transform holdPoint)
    {
        // Na mão: sem física e sem colisão (senão ela empurraria o jogador e o raycast
        // de interação poderia acertar a própria ferramenta).
        if (rb != null)
            rb.isKinematic = true;

        SetCollidersEnabled(false);
        SetHeldVisual(true);

        transform.SetParent(holdPoint, false);
        transform.SetLocalPositionAndRotation(heldLocalPosition, Quaternion.Euler(heldLocalEulerAngles));
    }

    // Chamado pelo Player ao largar a ferramenta.
    public void Drop(Vector3 position, Quaternion rotation)
    {
        transform.SetParent(null);
        transform.SetPositionAndRotation(position, rotation);

        SetHeldVisual(false);
        SetCollidersEnabled(true);

        if (rb != null)
            rb.isKinematic = false;
    }

    private void SetCollidersEnabled(bool value)
    {
        foreach (Collider col in colliders)
            col.enabled = value;
    }

    private void SetHeldVisual(bool held)
    {
        if (worldVisual != null)
            worldVisual.SetActive(!held);

        if (heldVisual != null)
            heldVisual.SetActive(held);
    }
}