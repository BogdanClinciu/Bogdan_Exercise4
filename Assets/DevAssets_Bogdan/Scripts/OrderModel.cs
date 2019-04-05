using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderModel : MonoBehaviour
{
    [SerializeField]
    private OrderView view;

    #region ObjectPrefabs
        [SerializeField]
        private GameObject itemObjectPrefab;
        [SerializeField]
        private GameObject orderObjectPrefab;
    #endregion

    #region Data sets
        private BinaryST<InventoryItemInstance> inventory;
        private BinaryST<Order> orderHistory;

        private List<InventoryItemInstance> itemsInCurentOrder;
        private List<Order> outgoingOrders;
    #endregion

    #region Spawned item Parents and lists (consider changing to dict)
        [SerializeField]
        private RectTransform inventoryItemParent;
        [SerializeField]
        private RectTransform currentOrderItemParent;


        private List<ItemObject> spawnedItemObjects;
        private List<ItemObject> spawnedItemObjectsInOrder;
    #endregion


    private InventoryItemInstance changeAmmountTargetItem;



    private void Start()
    {
        #region Initializations
            spawnedItemObjectsInOrder = new List<ItemObject>();
            spawnedItemObjects = new List<ItemObject>();

            inventory = new BinaryST<InventoryItemInstance>();
            itemsInCurentOrder = new List<InventoryItemInstance>();
        #endregion

        LoadDatabases();
        SearchInventory(null);
    }

    public bool CompleteItemAdd(string name, string basePrice, string quantity, int discount)
    {
        if(inventory.ContainsId(name))
        {
            view.ToggleAddItemWarning(true);
            return false;
        }

        int quant = 0;
        int.TryParse(quantity, out quant);
        float price = 1.0f;
        float.TryParse(basePrice, out price);

        InventoryItem itemBase = new InventoryItem(name, price);
        InventoryItemInstance itemInstance = new InventoryItemInstance(quant, discount, itemBase);

        inventory.Add(new Node<InventoryItemInstance>(itemInstance, itemBase.ID));
        // view -> update inventory items
        SearchInventory(null);

        SavedDatabaseHandler.SaveDatabase<InventoryItemInstance>(inventory.ToOrderedList(), false);
        return true;
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
                CreateItemObjects(Constants.ItemInteraction.AddToCart, inventoryItemParent, searchResults, spawnedItemObjects, false);
                return;
            }

            //display items matching
            searchResults = inventory.All(item => item.id.ToLower().StartsWith(searchQuery.ToLower()));
            CreateItemObjects(Constants.ItemInteraction.AddToCart, inventoryItemParent, searchResults, spawnedItemObjects, false);
            return;
        }

        //display all items
        searchResults = inventory.ToOrderedList();
        CreateItemObjects(Constants.ItemInteraction.AddToCart, inventoryItemParent, searchResults, spawnedItemObjects, false);

    }

    ///<summary>
    /// Creates the item objects from the given <paramref name="itemList"/> if they do not already exist in the provided <paramref name="spawnedList"/>, and disables ones that are not present.
    /// Objects are instantiated into the apropriate <paramref name="parentTransform"/>, and the <paramref name="interaction"/> enum determines the possible functions of that object.
    ///</summary>
    private void CreateItemObjects(Constants.ItemInteraction interaction, RectTransform parentTransform, List<InventoryItemInstance> itemList, List<ItemObject> spawnedList, bool destroyObjects)
    {
        foreach (ItemObject item in spawnedList)
        {
            if(!destroyObjects)
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                Destroy(item.gameObject);
            }
        }

        if(destroyObjects)
        {
            spawnedList.Clear();
        }

        foreach (InventoryItemInstance itemInstance in itemList)
        {
            if(!spawnedList.Exists(i => i.ID.Equals(itemInstance.Item.ID)))
            {
                ItemObject newItemObject = Instantiate(itemObjectPrefab, parentTransform).GetComponent<ItemObject>();
                newItemObject.UpdateItemObject(interaction, itemInstance.Item.Name, itemInstance.Quantity, itemInstance.DiscountedPrice, () => ItemButtonAction(itemInstance, interaction));
                spawnedList.Add(newItemObject);
            }
            else
            {
                spawnedList.Find(i => i.ID.Equals(itemInstance.Item.ID)).UpdateItemObject(interaction, itemInstance.Item.Name, itemInstance.Quantity, itemInstance.DiscountedPrice, () => ItemButtonAction(itemInstance, interaction));
            }
        }
    }

    ///<summary>
    /// Loads the item, order and historyDatabases and populates the relevant binary search trees.
    ///</summary>
    private void LoadDatabases()
    {
        List<InventoryItemInstance> data;
        List<string> dataIDs = new List<string>();

        SavedDatabaseHandler.LoadDatabase<InventoryItemInstance>(out data, false);

        for (int i = 0; i < data.Count; i++)
        {
            dataIDs.Add(data[i].Item.Name);
        }

        inventory = new BinaryST<InventoryItemInstance>(data, dataIDs);
    }


    #region Item object actions
        ///<summary>
        /// Item object possible click actions.
        ///</summary>
        private void ItemButtonAction(InventoryItemInstance instance, Constants.ItemInteraction interaction)
        {
            switch (interaction)
            {
                case Constants.ItemInteraction.AddToCart:
                {
                    BeginAddItemToCurentOrder(instance);
                    break;
                }
                case Constants.ItemInteraction.RemoveFromCart:
                {
                    RemoveItemFromCurentOrder(instance);
                    break;
                }
                case Constants.ItemInteraction.NoInteraction:
                {
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        private void BeginAddItemToCurentOrder(InventoryItemInstance instanceToAdd)
        {
            changeAmmountTargetItem = instanceToAdd;
            view.ToggleAmmountPrompt(true, MaxQuantity(instanceToAdd), CartQuantity(instanceToAdd));
        }

        private int MaxQuantity(InventoryItemInstance instanceToAdd)
        {
            int maxAmmount = 0;

            if(itemsInCurentOrder.Exists(a => a.Item.ID == changeAmmountTargetItem.Item.ID))
            {
                maxAmmount = instanceToAdd.Quantity - CartQuantity(instanceToAdd);
            }
            else
            {
                maxAmmount = instanceToAdd.Quantity;
            }

            return maxAmmount;
        }

        private int CartQuantity(InventoryItemInstance instanceToAdd)
        {
            int cartAmmount = 0;

            if(itemsInCurentOrder.Exists(a => a.Item.ID == changeAmmountTargetItem.Item.ID))
            {
                cartAmmount = itemsInCurentOrder.Find(i => i.Item.ID == instanceToAdd.Item.ID).Quantity;
            }

            return cartAmmount;
        }

        ///<summary>
        /// A button function of an item object, asigned at ItemObject instantiate.
        ///</summary>
        public bool AddItemToCurentOrder(string ammountString)
        {
            int ammount = 1;
            if(!int.TryParse(ammountString, out ammount))
            {
                view.ToggleAmmountPopupWarning(true);
                return false;
            }

            if(ammount > MaxQuantity(changeAmmountTargetItem) || ammount < 1)
            {
                view.ToggleAmmountPopupWarning(true);
                return false;
            }



            if(!itemsInCurentOrder.Exists(a => a.Item.ID == changeAmmountTargetItem.Item.ID))
            {
                InventoryItemInstance newItemInstance = new InventoryItemInstance(changeAmmountTargetItem);
                newItemInstance.Quantity = ammount;
                itemsInCurentOrder.Add(newItemInstance);
            }
            else
            {
                itemsInCurentOrder.Find(i => i.Item.ID == changeAmmountTargetItem.Item.ID).Quantity += ammount;
            }

            CreateItemObjects(Constants.ItemInteraction.RemoveFromCart, currentOrderItemParent, itemsInCurentOrder, spawnedItemObjectsInOrder, true);
            changeAmmountTargetItem = null;
            return true;
        }

        ///<summary>
        /// A button function of an item object, asigned at ItemObject instantiate.
        ///</summary>
        private void RemoveItemFromCurentOrder(InventoryItemInstance instanceToAdd)
        {
            if(itemsInCurentOrder.Contains(instanceToAdd))
            {
                itemsInCurentOrder.Remove(instanceToAdd);
            }
            CreateItemObjects(Constants.ItemInteraction.RemoveFromCart, currentOrderItemParent, itemsInCurentOrder, spawnedItemObjectsInOrder, true);
        }
    #endregion
}
