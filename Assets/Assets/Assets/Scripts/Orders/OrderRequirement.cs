using System;
using UnityEngine;

[Serializable]
public class OrderRequirement
{
    [SerializeField] private ItemTypeSO item;

    [Min(1)]
    [SerializeField] private int amount = 1;

    public ItemTypeSO Item => item;
    public int Amount => amount;
}