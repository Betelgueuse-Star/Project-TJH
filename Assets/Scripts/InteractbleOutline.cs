using ITISKIRUHERE;
using UnityEngine;

public class InteractableOutline : MonoBehaviour
{
    private AdvancedOutline outline;

    private void Awake()
    {
        outline = GetComponent<AdvancedOutline>();
        Hide();
    }

    public void Show()
    {
        outline.OutlineWidth = 10f;
    }

    public void Hide()
    {
        outline.OutlineWidth = 0f;
    }
}