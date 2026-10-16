using TMPro;
using UnityEngine;

// Mostra a quantidade de água do tanque. Fica separado do WaterTank
// para que o mesmo evento possa alimentar outras telas (ex.: terminal).
public class WaterTankUI : MonoBehaviour
{
    [SerializeField] private WaterTank waterTank;
    [SerializeField] private TextMeshProUGUI waterAmountText;

    private int lastDisplayedWater = -1;
    private bool started;

    private void OnEnable()
    {
        waterTank.OnWaterChanged += HandleWaterChanged;

        // Ao reativar (ex.: terminal ligando/desligando), atualiza o texto.
        // O primeiro refresh acontece no Start, depois do Awake do tanque.
        if (started)
            Refresh(waterTank.CurrentWater);
    }

    private void Start()
    {
        started = true;
        Refresh(waterTank.CurrentWater);
    }

    private void OnDisable()
    {
        waterTank.OnWaterChanged -= HandleWaterChanged;
    }

    private void HandleWaterChanged(float currentWater, float maxWater)
    {
        Refresh(currentWater);
    }

    private void Refresh(float currentWater)
    {
        int displayedWater = Mathf.FloorToInt(currentWater);

        // Só atualiza o texto quando o número inteiro muda,
        // já que o tanque enche um pouco a cada frame.
        if (displayedWater == lastDisplayedWater)
            return;

        lastDisplayedWater = displayedWater;

        // SetText com formato evita gerar lixo de string a cada atualização.
        waterAmountText.SetText("{0}", displayedWater);
    }
}