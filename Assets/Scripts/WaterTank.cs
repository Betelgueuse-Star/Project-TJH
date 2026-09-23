using UnityEngine;

public class WaterTank : MonoBehaviour, IInteractable, IConditionalInteractable
{
    [SerializeField] private float maxWater = 100f;

    [SerializeField] private float currentWater;

    private void Awake()
    {
        currentWater = maxWater;
    }

    public float TakeWater(float amount)
    {
        float waterTaken = Mathf.Min(amount, currentWater);

        currentWater -= waterTaken;

        return waterTaken;
    }

    public (bool, float) CanInteract(Player player)
    {
        if (player.CurrentTool == null)
            return (false, 0f);

        if (player.CurrentTool.TryGetComponent(out WateringCan wateringCan))
           return (true, 2f);

        else return (false, 0f);
    }

    public void Interact(Player player)
    {
        var (canInteract, interactionValue) = CanInteract(player);

        if (!canInteract)
            return;

        if (interactionValue == 2f)
        {
            if (player.CurrentTool.TryGetComponent(out WateringCan wateringCan))
            {
                wateringCan.FillFromTank(this);
            }
        }
    }
}