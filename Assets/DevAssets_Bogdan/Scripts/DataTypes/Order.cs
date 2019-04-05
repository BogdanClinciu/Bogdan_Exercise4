using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Order
{
    public string ID;
    public string ClientName;
    public List<InventoryItemInstance> Items;

    public Order(string id, string clientName, List<InventoryItemInstance> items)
    {
        ID = id;
        ClientName = clientName;
        Items = items;
    }

    public float TotalCost()
    {
        float totalCost = 0;
        foreach(InventoryItemInstance item in Items)
        {
            totalCost += item.DiscountedPrice;
        }
        return totalCost;
    }

    public float TotalDiscount()
    {
        float baseCost = 0;
        foreach(InventoryItemInstance item in Items)
        {
            baseCost += item.Item.BasePrice;
        }
        return baseCost - TotalCost();
    }
}
