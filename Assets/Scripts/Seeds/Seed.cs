using UnityEngine;

public class Seed : MonoBehaviour
{
    [Header("Seed")]
    [SerializeField] private SeedDataSO seedData;

    public SeedDataSO SeedData => seedData; 

    public bool IsPlanted { get; private set; }
  
    public void Plant()
    {
        if (IsPlanted)
            return;

        IsPlanted = true;

        Debug.Log(seedData.seedName + " foi plantada!");
    }
}