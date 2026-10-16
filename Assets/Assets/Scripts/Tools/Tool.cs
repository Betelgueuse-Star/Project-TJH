using UnityEngine;

// Ferramentas Herdam de Tool
public abstract class Tool : MonoBehaviour
{
    public abstract ToolType Type { get; }

    [Header("Pose na mão")]
    // Posição/rotação local em relação ao ponto da mão. Ajuste por ferramenta.
    [SerializeField] private Vector3 heldLocalPosition;
    [SerializeField] private Vector3 heldLocalEulerAngles;

    [Header("Visuais (opcional)")]
    [SerializeField] private GameObject worldVisual; // Modelo no mundo
    [SerializeField] private GameObject heldVisual; // Modelo segurado pelo Player

    // FUTURO (animações)
    // O Animator deve ficar no filho HeldVisual
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
    // chamando base.Awake()
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        SetHeldVisual(false); // começa no mundo
    }

    // Chamado pelo Player ao pegar a ferramenta
    public void Equip(Transform holdPoint)
    {
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