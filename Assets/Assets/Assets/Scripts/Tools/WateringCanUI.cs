// 1) No prefab do regador, dentro do filho HeldVisual, crie um Canvas (World Space) com
//    uma Image do tipo "Filled" (barra de água).
// 2) Coloque este script no Canvas e arraste o WateringCan e a Image nos campos.

// using UnityEngine;
// using UnityEngine.UI;
//
// public class WateringCanUI : MonoBehaviour
// {
//     [SerializeField] private WateringCan can;
//     [SerializeField] private Image fill; // Image do tipo "Filled"
//
//     private void OnEnable()
//     {
//         can.WaterChanged += Refresh;
//         Refresh(can.CurrentWater, can.MaxWater); // mostra o valor atual assim que equipa
//     }
//
//     private void OnDisable()
//     {
//         can.WaterChanged -= Refresh;
//     }
//
//     private void Refresh(float current, float max)
//     {
//         fill.fillAmount = current / max;
//     }
// }