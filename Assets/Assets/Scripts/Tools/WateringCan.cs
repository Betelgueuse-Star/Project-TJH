using UnityEngine;
// FUTURO (UI do regador): para usar o evento WaterChanged mais abaixo, descomente também:
// using System;
// MUDANÇA: removidos "using System.Collections;" e "using UnityEngine.Rendering;",
// que não eram usados.

// MUDANÇA: herda de Tool (a classe base única das ferramentas, que cuida de pegar e
// largar) e declara o tipo. Como a ferramenta do mundo e a da mão agora são o MESMO
// objeto, a água do regador se mantém ao largar e pegar de novo.
public class WateringCan : Tool
{
    public override ToolType Type => ToolType.WateringCan;

    [Header("Água")]
    [SerializeField] private float maxWater = 10f;
    [SerializeField] private float currentWater;

    [Header("Quantidade Que Rega")]
    [SerializeField] private float waterAmount = 0.2f;

    public float WaterAmount => waterAmount;
    public float CurrentWater => currentWater;

    // FUTURO (UI do regador): descomente junto com o WateringCanUI.cs e com as linhas
    // "WaterChanged?.Invoke" dos métodos abaixo.
    // public float MaxWater => maxWater;
    // public event Action<float, float> WaterChanged; // (água atual, água máxima)

    public float GetAndRemoveWater(float amount)
    {
        float waterGiven = Mathf.Min(amount, currentWater);

        currentWater -= waterGiven;

        // FUTURO (UI): avisa a UI que a água mudou.
        // WaterChanged?.Invoke(currentWater, maxWater);

        return waterGiven;
    }

    public void AddWater(float amount)
    {
        currentWater = Mathf.Min(currentWater + amount, maxWater);

        // FUTURO (UI): avisa a UI que a água mudou.
        // WaterChanged?.Invoke(currentWater, maxWater);
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