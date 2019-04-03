using System.Collections.Generic;

[System.Serializable]
public class Order
{
    public int ID;
    public string ClientName;
    public List<InventoryItemInstance> Items;
}
