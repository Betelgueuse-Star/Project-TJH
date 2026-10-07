using TMPro;
using UnityEngine;

public class OrderRequirementUI : MonoBehaviour
{
    // Esse script é pra ser colocado em uma prefab

    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text amountText;

    public void Setup(string itemName, int deliveredAmount, int requiredAmount)
    {
        itemNameText.text = itemName;
        amountText.text = $"{deliveredAmount} / {requiredAmount}";
    }
}