using UnityEngine;

public class Tool : MonoBehaviour
{
    [Header("Tool Prefabs")]
    [SerializeField] private GameObject worldPrefab;
    [SerializeField] private GameObject equippedPrefab;

    public GameObject WorldPrefab => worldPrefab;
    public GameObject EquippedPrefab => equippedPrefab;
}