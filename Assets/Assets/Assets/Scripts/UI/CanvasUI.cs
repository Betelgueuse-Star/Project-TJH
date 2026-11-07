using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    // Script para o Canvas geral

    [SerializeField] private Player player;
    [SerializeField] private GameObject interactableImage;

    private void OnEnable()
    {
        player.OnCanInteractChanged += SetVisible;
        SetVisible(false); // começa escondida
    }

    private void OnDisable()
    {
        player.OnCanInteractChanged -= SetVisible;
    }

    private void SetVisible(bool visible)
    {
        interactableImage.SetActive(visible);
    }
}