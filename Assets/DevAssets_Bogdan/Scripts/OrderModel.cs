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
        [SerializeField]
        private RectTransform outgoingOrdersParent;


        [SerializeField]
        private RectTransform outgoingOrdersItemParent;
        [SerializeField]
        private RectTransform orderHistoryItemPanel;


        private List<ItemObject> spawnedItemObjects;
        private List<ItemObject> spawnedItemObjectsInOrder;
        private List<OrderObject> spawnedOutgoingOrderObjects;

        private List<ItemObject> spawnedOutgoingItemObjects;
        private List<ItemObject> spawnedHistoryItemObjects;
    #endregion


    private InventoryItemInstance changeAmmountTargetItem;



    private void Start()
    {
        #region Initializations
            spawnedItemObjectsInOrder = new List<ItemObject>();
            spawnedItemObjects = new List<ItemObject>();
            spawnedOutgoingOrderObjects = new List<OrderObject>();
            spawnedOutgoingItemObjects = new List<ItemObject>();
            spawnedHistoryItemObjects = new List<ItemObject>();

            inventory = new BinaryST<InventoryItemInstance>();
            itemsInCurentOrder = new List<InventoryItemInstance>();
            outgoingOrders = new List<Order>();
        #endregion

        LoadDatabases();
        SearchInventory(null);
    }

    #region Finalize order logic

        public bool ConfirmFinalizeOrder(string clientName)
        {
            if(string.IsNullOrEmpty(clientName))
            {
                //return and toggle warning
                return false;
            }

            if(outgoingOrders.Exists(order => order.ID == clientName.ToLower()))
            {
                Order existingOrder = outgoingOrders.Find(order => order.ID == clientName.ToLower());
                foreach (InventoryItemInstance item in itemsInCurentOrder)
                {
                    //if an item in the curent order mathces one in the existing outgoing orders
                    if(existingOrder.Items.Exists(orderItem => orderItem.Item.ID == item.Item.ID))
                    {
                        //fow now the discount from the initial order is kept for new items
                        existingOrder.Items.Find(orderItem => orderItem.Item.ID == item.Item.ID).Quantity +=
                        item.Quantity;
                    }
                    //else whe add the item from the curent order to the existing order
                    else
                    {
                        existingOrder.Items.Add(new InventoryItemInstance(item));
                    }
                }
                CreateOrderObjects(true, outgoingOrdersParent, outgoingOrders, spawnedOutgoingOrderObjects, true, false);
            }
            else
            {
                outgoingOrders.Add(new Order(clientName.ToLower(), clientName, new List<InventoryItemInstance>(itemsInCurentOrder)));
                CreateOrderObjects(true, outgoingOrdersParent, outgoingOrders, spawnedOutgoingOrderObjects, true, false);
            }

            //remove item quantities from stock
            foreach (InventoryItemInstance item in itemsInCurentOrder)
            {
                inventory.GetNodeAt(item.Item.ID).NodeValue.Quantity -= item.Quantity;
            }
            //update the inventory
            SearchInventory(null);

            //clear curent order
            itemsInCurentOrder.Clear();
            CreateItemObjects(Constants.ItemInteraction.NoInteraction, currentOrderItemParent, itemsInCurentOrder, spawnedItemObjectsInOrder, true);
            return true;
        }

    #endregion

    private void CreateOrderObjects(bool canEdit, RectTransform parentTransform, List<Order> orderList, List<OrderObject> spawnedList, bool destroyObjects, bool isHistory)
    {
        foreach (OrderObject orderObject in spawnedList)
        {
            if(!destroyObjects)
            {
                orderObject.gameObject.SetActive(false);
            }
            else
            {
                Destroy(orderObject.gameObject);
            }
        }

        if(destroyObjects)
        {
            spawnedList.Clear();
        }

        foreach (Order order in orderList)
        {
            if(!spawnedList.Exists(i => i.ID.Equals(order.ClientName)))
            {
                OrderObject newOrderObject = Instantiate(orderObjectPrefab, parentTransform).GetComponent<OrderObject>();
                newOrderObject.UpdateOrderObject(canEdit, order.ClientName, order.TotalCost(), order.TotalDiscount(), () => OrderButtonAction(order, true), () => ShowOrderItems(order, isHistory));
                spawnedList.Add(newOrderObject);
            }
            else
            {
                spawnedList.Find(i => i.ID.Equals(order.ClientName)).UpdateOrderObject(canEdit, order.ClientName, order.TotalCost(), order.TotalDiscount(), () => OrderButtonAction(order, true), () => ShowOrderItems(order, isHistory));
            }
        }
    }


    public bool CompleteNewItemAdd(string name, string basePrice, string quantity, int discount)
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

    #region Order object actions

        ///<summary>
        /// Item object possible click actions.
        ///</summary>
        private void OrderButtonAction(Order order, bool editable)
        {
            if(editable)
            {
                AddOutgoingItemsBackToStock(order);
                SearchInventory(null);
                RemoveFromOutgoingOrders(order);
                //clear outgoing item panel
                foreach (ItemObject item in spawnedOutgoingItemObjects)
                {
                    Destroy(item.gameObject);
                }
                spawnedOutgoingItemObjects.Clear();
            }
        }

        private void RemoveFromOutgoingOrders(Order orderToRemove)
        {
            if(outgoingOrders.Contains(orderToRemove))
            {
                outgoingOrders.Remove(orderToRemove);
            }
            CreateOrderObjects(true, outgoingOrdersParent, outgoingOrders, spawnedOutgoingOrderObjects, true, false);
        }

        private void AddOutgoingItemsBackToStock(Order orderBeingRemoved)
        {
            foreach (InventoryItemInstance item in orderBeingRemoved.Items)
            {
                inventory.GetNodeAt(item.Item.ID).NodeValue.Quantity += item.Quantity;
            }
        }

        private void ShowOrderItems(Order order, bool isHistory)
        {
            if(order != null)
            {
                CreateItemObjects(
                    Constants.ItemInteraction.NoInteraction,
                    (!isHistory) ? outgoingOrdersItemParent : orderHistoryItemPanel,
                    order.Items,
                    (!isHistory) ? spawnedOutgoingItemObjects : spawnedHistoryItemObjects,
                    true
                );
            }
        }

    #endregion


    #region Item object actions

        ///<summary>
        /// A button function of an item object, asigned at ItemObject instantiate.
        ///</summary>
        public bool ConfirmAddItemToCurentOrder(string ammountString)
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

        ///<summary>
        /// Sets the item to add as <paramref name="instanceToAdd"/> and triggers the ammount prompt trough the view.
        ///</summary>
        private void BeginAddItemToCurentOrder(InventoryItemInstance instanceToAdd)
        {
            changeAmmountTargetItem = instanceToAdd;
            view.ToggleAmmountPrompt(true, MaxQuantity(instanceToAdd), CartQuantity(instanceToAdd));
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

    #region QuantityUtils

        ///<summary>
        /// Returns the quantity left in stock for the given <paramref name="instanceToAdd"/>.
        ///</summary>
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

        ///<summary>
        /// Returns the quantity in the curent order cart for the given <paramref name="instanceToAdd"/>.
        ///</summary>
        private int CartQuantity(InventoryItemInstance instanceToAdd)
        {
            int cartAmmount = 0;

            if(itemsInCurentOrder.Exists(a => a.Item.ID == changeAmmountTargetItem.Item.ID))
            {
                cartAmmount = itemsInCurentOrder.Find(i => i.Item.ID == instanceToAdd.Item.ID).Quantity;
            }

            return cartAmmount;
        }

    #endregion

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
            dataIDs.Add(data[i].Item.Name.ToLower());
        }

        inventory = new BinaryST<InventoryItemInstance>(data, dataIDs);
    }

}
