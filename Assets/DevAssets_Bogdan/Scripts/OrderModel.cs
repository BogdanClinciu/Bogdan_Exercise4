using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderModel : MonoBehaviour
{
    [SerializeField]
    private GameObject itemObjectPrefab;
    [SerializeField]
    private GameObject orderObjectPrefab;
    [SerializeField]
    private GameObject itemObjectPrefabEditable;
    [SerializeField]
    private GameObject orderObjectPrefabEditable;

    [SerializeField]
    private RectTransform inventoryItemParent;
    [SerializeField]
    private RectTransform selectedOrderItemParent;

    private static BinaryST<InventoryItemInstance> inventory;
    private static BinaryST<Order> orders;
    private static BinaryST<Order> orderHistory;


    private List<ItemObject> inventoryItemObjects;



    private void Start()
    {
        inventoryItemObjects = new List<ItemObject>();
    }

    public void CompleteItemAdd(string name, string basePrice, string quantity, int discount)
    {
        int quant = 0;
        int.TryParse(quantity, out quant);
        float price = 1.0f;
        float.TryParse(basePrice, out price);

        InventoryItem itemBase = new InventoryItem(name, price);
        InventoryItemInstance itemInstance = new InventoryItemInstance(quant, discount, itemBase);

        inventory.Add(new Node<InventoryItemInstance>(itemInstance, itemBase.ID));
        // view -> update inventory items
    }

    ///<summary>
    /// Searches the item inventory for the requested item query, if the query string is entirely composed of digits it will insead return the item matching that id if any.
    ///</summary>
    public void SearchInventory(string searchQuery)
    {
        List<InventoryItemInstance> searchResults = new List<InventoryItemInstance>();
        //get results from bst -> request update in view

        //find all items with matching name
        if(!string.IsNullOrEmpty(searchQuery))
        {
            if(inventory.ContainsId(searchQuery.ToLower()))
            {
                searchResults.Add(inventory.GetNodeAt(searchQuery.ToLower()).NodeValue);
                CreateSearchItemObjects(Constants.ItemInteraction.AddToCart, inventoryItemParent, searchResults);
                return;
            }

            //display items matching
            searchResults = inventory.All(item => item.id.ToLower().StartsWith(searchQuery.ToLower()));
            CreateSearchItemObjects(Constants.ItemInteraction.AddToCart, inventoryItemParent, searchResults);
            return;
        }

        //display all items
        CreateSearchItemObjects(Constants.ItemInteraction.AddToCart, inventoryItemParent, searchResults);
        searchResults = inventory.ToOrderedList();

    }

    public static bool ItemExists(string id)
    {
        return inventory.GetNodeAt(id) != null;
    }

    private void CreateSearchItemObjects(Constants.ItemInteraction interaction, RectTransform parentTransform, List<InventoryItemInstance> resultsList)
    {
        foreach (ItemObject item in inventoryItemObjects)
        {
            item.gameObject.SetActive(false);
        }

        foreach (InventoryItemInstance itemInstance in resultsList)
        {
            if(!inventoryItemObjects.Exists(i => i.ID.Equals(itemInstance.Item.ID)))
            {
                ItemObject newItemObject = Instantiate(itemObjectPrefabEditable, parentTransform).GetComponent<ItemObject>();
                newItemObject.UpdateItemObject(interaction, itemInstance.Item.Name, itemInstance.Quantity, itemInstance.DiscountedPrice, () => DEBUG_ITEM_ACTION(itemInstance));
                inventoryItemObjects.Add(newItemObject);
            }
            else
            {
                inventoryItemObjects.Find(i => i.ID.Equals(itemInstance.Item.ID)).UpdateItemObject(interaction, itemInstance.Item.Name, itemInstance.Quantity, itemInstance.DiscountedPrice, () => DEBUG_ITEM_ACTION(itemInstance));
            }
        }
    }

    private void DEBUG_ITEM_ACTION(InventoryItemInstance instance)
    {
        Debug.Log("Pressed: " + instance.Item.Name + "   " + instance.Item.Name);
    }

}
