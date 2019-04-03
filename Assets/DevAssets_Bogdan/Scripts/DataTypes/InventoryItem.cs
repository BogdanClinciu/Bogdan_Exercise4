
using UnityEngine;

[System.Serializable]
public class InventoryItemInstance
{
    public InventoryItem Item;
    public int Quantity;
    public int Discount
    {
        get
        {
            return Discount;
        }
        set
        {
            Discount = Mathf.Clamp(value, 0, 100);
        }
    }
    public float DiscountedPrice
    {
        get
        {
            return Item.BasePrice - Item.BasePrice * Discount/100;
        }
    }

    public InventoryItemInstance(int quantity, int discount, InventoryItem item)
    {
        Quantity = quantity;
        discount = Discount;
        Item = item;
    }
}

[System.Serializable]
public class InventoryItem
{
    public string ID;
    public string Name;
    public float BasePrice;

    public InventoryItem(string name, float basePrice)
    {
        ID = name.ToLower();
        Name = name;
        BasePrice = basePrice;
    }
}
