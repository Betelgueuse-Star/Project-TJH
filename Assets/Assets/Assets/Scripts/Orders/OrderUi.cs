using TMPro;
using UnityEngine;

public class OrderUI : MonoBehaviour
{
    [SerializeField] private GameObject activeIndicator;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Transform requirementsContainer;

    public GameObject ActiveIndicator => activeIndicator;
    public TMP_Text RewardText => rewardText;
    public Transform RequirementsContainer => requirementsContainer;
}