
using UnityEngine;

public class ShopItemButton : MonoBehaviour
{
    [SerializeField] private Terminal terminal;
    [SerializeField] private ItemTypeSO item;

    public void Buy()
    {
        terminal.BuyItem(item);
    }
}