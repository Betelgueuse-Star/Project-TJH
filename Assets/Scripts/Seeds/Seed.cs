using UnityEngine;

public class Seed : MonoBehaviour
{
    [Header("Seed")]
    [SerializeField] private SeedDataSO seedData;

    private ObjectGrabbable objectGrabbable;

    public SeedDataSO SeedData => seedData;

    public bool IsBeingHeld => objectGrabbable != null && objectGrabbable.IsBeingHeld;

    public bool IsPlanted { get; private set; }


    private void Awake()
    {
        objectGrabbable = GetComponent<ObjectGrabbable>();
    }

    public void Plant()
    {
        if (IsPlanted)
            return;

        IsPlanted = true;

        Debug.Log(seedData.seedName + " foi plantada!");
    }
}