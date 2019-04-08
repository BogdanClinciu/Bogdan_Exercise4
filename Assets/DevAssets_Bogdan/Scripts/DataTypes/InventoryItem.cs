
using UnityEngine;

[System.Serializable]
public class InventoryItemInstance
{
    public InventoryItem Item;
    public int Quantity;
    public int discount = 0;
    public int DiscountPercent
    {
        get
        {
            return discount;
        }
        set
        {
            discount = Mathf.Clamp(value, 0, 90);
        }
    }
    public float DiscountedPrice
    {
        get
        {
            return Item.BasePrice - Item.BasePrice * discount/100;
        }
    }

    public InventoryItemInstance(int quantity, int discount, InventoryItem item)
    {
        Quantity = quantity;
        DiscountPercent = discount;
        Item = item;
    }

    public InventoryItemInstance(InventoryItemInstance instanceToCopy)
    {
        Item = new InventoryItem(instanceToCopy.Item);
        Quantity = instanceToCopy.Quantity;
        DiscountPercent = instanceToCopy.DiscountPercent;
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

    public InventoryItem(InventoryItem itemToCopy)
    {
        ID = itemToCopy.ID;
        Name = itemToCopy.Name;
        BasePrice = itemToCopy.BasePrice;
    }
}
