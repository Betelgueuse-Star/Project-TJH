using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class WateringCan : MonoBehaviour
{
    [Header("Água")]
    [SerializeField] private float maxWater = 10f;
    [SerializeField] private float currentWater;

    [Header("Quantidade Que Rega")]
    [SerializeField] private float waterAmount= 0.2f;

    public float WaterAmount => waterAmount;
    public float CurrentWater => currentWater;

    public float GetAndRemoveWater(float amount)
    {
        float waterGiven = Mathf.Min(amount, currentWater);

        currentWater -= waterGiven;

        return waterGiven;
    }

    public void AddWater(float amount)
    {
        currentWater = Mathf.Min(currentWater + amount, maxWater);
    }

    public void FillFromTank(WaterTank tank)
    {
        float missingWater = maxWater - currentWater;

        if (missingWater <= 0f)
            return;

        float waterReceived = tank.TakeWater(missingWater);

        AddWater(waterReceived);
    }
}