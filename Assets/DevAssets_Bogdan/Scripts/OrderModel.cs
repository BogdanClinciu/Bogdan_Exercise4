using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderModel : MonoBehaviour
{
    [SerializeField]
    private OrderView view;

    #region Data sets

        private BinaryST<InventoryItemInstance> inventory;
        private BinaryST<Order> orderHistory;

        private List<InventoryItemInstance> itemsInCurentOrder;
        private List<Order> outgoingOrders;

    #endregion

    #region Spawned item Parents and lists

        [Header("Inventory/CurentOrder spawn parents")]
        [SerializeField]
        private RectTransform inventoryItemParent;
        [SerializeField]
        private RectTransform currentOrderItemParent;

        [Header("Order history spawn parents")]
        [SerializeField]
        private RectTransform orderHistoryParent;
        [SerializeField]
        private RectTransform orderHistoryItemPanel;

        [Header("Outgoing orders spawn parents")]
        [SerializeField]
        private RectTransform outgoingOrdersParent;
        [SerializeField]
        private RectTransform outgoingOrdersItemParent;

        [Header("Order sheet spawn parent")]
        [SerializeField]
        private RectTransform orderSheetParent;

        private List<ItemObject> spawnedInventoryItemObjects;
        private List<ItemObject> spawnedItemObjectsInOrder;
        private List<OrderObject> spawnedOrderHistoryObjects;
        private List<ItemObject> spawnedHistoryItemObjects;
        private List<OrderObject> spawnedOutgoingOrderObjects;
        private List<ItemObject> spawnedOutgoingItemObjects;
        private List<OrderSheetObject> spawnedOrderSheets;

    #endregion

    private InventoryItemInstance changeAmmountTargetItem;



    private void Start()
    {
        #region Data structure initializations
            spawnedItemObjectsInOrder = new List<ItemObject>();
            spawnedInventoryItemObjects = new List<ItemObject>();
            spawnedOutgoingOrderObjects = new List<OrderObject>();
            spawnedOutgoingItemObjects = new List<ItemObject>();
            spawnedHistoryItemObjects = new List<ItemObject>();
            spawnedOrderSheets = new List<OrderSheetObject>();
            spawnedOrderHistoryObjects = new List<OrderObject>();
            itemsInCurentOrder = new List<InventoryItemInstance>();
            outgoingOrders = new List<Order>();
        #endregion

        LoadDatabases();
        SearchInventory(null);
        SearchOrderHistory(null);
    }

    ///<summary>
    /// Clears all items from the curent order.
    ///</summary>
    public void ClearCurentOrderPanel()
    {
        foreach (ItemObject item in spawnedItemObjectsInOrder)
        {
            Destroy(item.gameObject);
        }
        spawnedItemObjectsInOrder.Clear();
    }

    ///<summary>
    /// Returns item to add validity. On a succesfull validation the item will be added to the inventory, and the item database will be saved.
    ///</summary>
    public bool ConfirmNewItemAdd(string name, string basePrice, string quantity, int discount)
    {
        if(inventory.ContainsId(name))
        {
            view.editPanel.DisplayErrorMessage(Constants.ERROR_ADD);
            return false;
        }

        int quant = 0;
        int.TryParse(quantity, out quant);
        float price = 1.0f;
        float.TryParse(basePrice, out price);

        if(quant < 0 || price < 0)
        {
            view.editPanel.DisplayErrorMessage(Constants.ERROR_EDIT);
            return false;
        }

        InventoryItem itemBase = new InventoryItem(name, price);
        InventoryItemInstance itemInstance = new InventoryItemInstance(quant, discount, itemBase);

        inventory.Add(new Node<InventoryItemInstance>(itemInstance, itemBase.ID));
        // view -> update inventory items
        SearchInventory(null);

        SavedDatabaseHandler.SaveDatabase<InventoryItemInstance>(inventory.ToOrderedList());
        return true;
    }

    ///<summary>
    /// Validates the stock modification and saves the modified item. Returns false on invalid entry for price or quantity.
    ///</summary>
    public bool ConfirmStockEdit(string priceString, string ammountString, int discount)
    {
        float price = 0f;
        float.TryParse(priceString, out price);
        int ammount = -1;
        int.TryParse(ammountString, out ammount);

        if (price < 1 || ammount < 0)
        {
            return false;
        }

        InventoryItemInstance item = inventory.GetNodeAt(changeAmmountTargetItem.Item.ID).NodeValue;

        item.Item.BasePrice = price;
        item.Quantity = ammount;
        item.discount = discount;

        //save the database
        SavedDatabaseHandler.SaveDatabase<InventoryItemInstance>(inventory.ToOrderedList());
        SearchInventory(null);
        return true;
    }

    ///<summary>
    /// Cleares the change ammount target
    ///</summary>
    public void CancelItemEdit()
    {
        changeAmmountTargetItem = null;
    }

    ///<summary>
    /// Tries to parse the <paramref name="ammountString"/>, validates the parsed ammount and if valid adds the item to the curent order.
    /// Returns false when parsing failed or the value is invalid.
    ///</summary>
    public bool ConfirmAddItemToCurentOrder(string ammountString)
    {
        int ammount = 1;
        if(!int.TryParse(ammountString, out ammount))
        {
            view.ToggleAmmountPopupWarning(true);
            return false;
        }

        if(ammount > MaxStockQuantity(changeAmmountTargetItem) || ammount < 1)
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

        view.UpdateItemObjects(
            Constants.ItemInteraction.RemoveFromCart,
            currentOrderItemParent,
            itemsInCurentOrder,
            spawnedItemObjectsInOrder,
            true,
            ItemButtonAction,
            OpenStockEditPanel
            );
        changeAmmountTargetItem = null;
        return true;
    }


    #region Order finalization and placement logic

        ///<summary>
        /// Validates outgoing orders and on success promps view to creates sheet objects for each order.
        ///</summary>
        public bool BeginPlaceOutgoingOrders()
    {
        if(outgoingOrders.Count < 1)
        {
            return false;
        }

        view.UpdateSheetObjects(orderSheetParent, outgoingOrders, spawnedOrderSheets, ItemButtonAction, OpenStockEditPanel);

        return true;
    }

        ///<summary>
        /// Adds the finalized outgoing orders to history and deletes the sheet objects creared to show them.
        // Also triggeres save request for both stock values and order history.
        ///</summary>
        public void ConfirmPlaceOutgoingOrders()
        {
            foreach (Order order in outgoingOrders)
            {
                orderHistory.Add(new Node<Order>(order, order.ClientName.ToLower()));
            }

            //add outgoing to history
            //clear the outgoing list
            outgoingOrders.Clear();
            view.UpdateOrderObjects(
                false,
                outgoingOrdersParent,
                outgoingOrders,
                spawnedOutgoingOrderObjects,
                true,
                false,
                OrderRemoveAction,
                ShowOrderItems
                );

            //destroy placed orders from placed panel
            foreach (OrderSheetObject sheet in spawnedOrderSheets)
            {
                Destroy(sheet.gameObject);
            }
            spawnedOrderSheets.Clear();

            //Save the updated inventory database and order history
            SavedDatabaseHandler.SaveDatabase<InventoryItemInstance>(inventory.ToOrderedList());
            SavedDatabaseHandler.SaveDatabase<Order>(orderHistory.ToOrderedList());
            SearchOrderHistory(null);
        }

        ///<summary>
        /// Returns validity of the given <paramref name= "clientName"/> and if valid adds the curent order to the outgoing orders list.
        ///</summary>
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
                view.UpdateOrderObjects(
                    true,
                    outgoingOrdersParent,
                    outgoingOrders,
                    spawnedOutgoingOrderObjects,
                    true,
                    false,
                    OrderRemoveAction,
                    ShowOrderItems
                    );
            }
            else
            {
                outgoingOrders.Add(new Order(clientName.ToLower(), clientName, new List<InventoryItemInstance>(itemsInCurentOrder)));
                view.UpdateOrderObjects(
                    true,
                    outgoingOrdersParent,
                    outgoingOrders,
                    spawnedOutgoingOrderObjects,
                    true,
                    false,
                    OrderRemoveAction,
                    ShowOrderItems
                    );
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
            view.UpdateItemObjects(
                Constants.ItemInteraction.NoInteraction,
                currentOrderItemParent,
                itemsInCurentOrder,
                spawnedItemObjectsInOrder,
                true,
                ItemButtonAction,
                OpenStockEditPanel
                );
            return true;
        }

    #endregion

    #region Search functions

        ///<summary>
        /// Searches the item inventory for the requested item query, if the query string matches an id exactly it will insead return the item matching that id.
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
                    //display single matching item
                    searchResults.Add(inventory.GetNodeAt(searchQuery.ToLower()).NodeValue);
                }
                else
                {
                    //display items matching
                    searchResults = inventory.All(item => item.id.ToLower().StartsWith(searchQuery.ToLower()));
                }

            }
            else
            {
                //display all items
                searchResults = inventory.ToOrderedList();
            }

            //display all items
            view.UpdateItemObjects(
                Constants.ItemInteraction.AddToCart,
                inventoryItemParent,
                searchResults,
                spawnedInventoryItemObjects,
                false,
                ItemButtonAction,
                OpenStockEditPanel
                );
        }

        ///<summary>
        /// Searches the order history for the requested client query, if the query string matches an id exactly it will insead return the order matching that id.
        ///</summary>
        public void SearchOrderHistory(string searchQuery)
        {
            List<Order> searchResults = new List<Order>();
            //get results from bst -> request update in view

            //find all items with matching name
            if(!string.IsNullOrEmpty(searchQuery))
            {
                if(orderHistory.ContainsId(searchQuery.ToLower()))
                {
                    //display single matching item
                    searchResults.Add(orderHistory.GetNodeAt(searchQuery.ToLower()).NodeValue);
                }
                else
                {
                    //display items matching
                    searchResults = orderHistory.All(item => item.id.ToLower().StartsWith(searchQuery.ToLower()));
                }


            }
            else
            {
                //display all items
                searchResults = orderHistory.ToOrderedList();
            }

            view.UpdateOrderObjects(
                false,
                orderHistoryParent,
                searchResults,
                spawnedOrderHistoryObjects,
                false,
                true,
                OrderRemoveAction,
                ShowOrderItems
                );
        }

    #endregion

    #region Order object actions

        ///<summary>
        /// Order object remove action. Assigned to the order object on instantiation
        ///</summary>
        private void OrderRemoveAction(Order order, bool editable)
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

        ///<summary>
        /// Removes the <paramref name="orderToRemove"/> from the outgoing orders list and updates promps the view to update the outgoing order objects
        ///</summary>
        private void RemoveFromOutgoingOrders(Order orderToRemove)
        {
            if(outgoingOrders.Contains(orderToRemove))
            {
                outgoingOrders.Remove(orderToRemove);
            }
            view.UpdateOrderObjects(
                true, outgoingOrdersParent,
                outgoingOrders,
                spawnedOutgoingOrderObjects,
                true,
                false,
                OrderRemoveAction,
                ShowOrderItems
                );
        }

        ///<summary>
        /// Returns the <paramref name="orderBeingRemoved"/>'s item ammounts back to the apropriate stock items.
        ///</summary>
        private void AddOutgoingItemsBackToStock(Order orderBeingRemoved)
        {
            foreach (InventoryItemInstance item in orderBeingRemoved.Items)
            {
                inventory.GetNodeAt(item.Item.ID).NodeValue.Quantity += item.Quantity;
            }
        }

        ///<summary>
        /// Promps the view to show the <paramref name="order"/>'s items in the apropriate panel defined by the <paramref name="isHistory"/> parameter.
        ///</summary>
        private void ShowOrderItems(Order order, bool isHistory)
        {
            if(order != null)
            {
                view.UpdateItemObjects(
                    Constants.ItemInteraction.NoInteraction,
                    (!isHistory) ? outgoingOrdersItemParent : orderHistoryItemPanel,
                    order.Items,
                    (!isHistory) ? spawnedOutgoingItemObjects : spawnedHistoryItemObjects,
                    true,
                    ItemButtonAction,
                    OpenStockEditPanel
                );
            }
        }

    #endregion

    #region Item object actions

        ///<summary>
        /// Item object possible click actions switch.
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
        /// ItemObject function, Sets the item to add as <paramref name="instanceToAdd"/> and triggers the ammount prompt trough the view.
        ///</summary>
        private void BeginAddItemToCurentOrder(InventoryItemInstance instanceToAdd)
        {
            changeAmmountTargetItem = instanceToAdd;
            view.ToggleAmmountPopup(true, MaxStockQuantity(instanceToAdd), CartQuantity(instanceToAdd));
        }

        ///<summary>
        /// ItemObject function, asigned at ItemObject instantiate.
        ///</summary>
        private void RemoveItemFromCurentOrder(InventoryItemInstance instanceToAdd)
        {
            if(itemsInCurentOrder.Contains(instanceToAdd))
            {
                itemsInCurentOrder.Remove(instanceToAdd);
            }
            view.UpdateItemObjects(
                Constants.ItemInteraction.RemoveFromCart,
                currentOrderItemParent, itemsInCurentOrder,
                spawnedItemObjectsInOrder,
                true,
                ItemButtonAction,
                OpenStockEditPanel
                );
        }

        ///<summary>
        /// ItemObject function, promps the view to opens the stock edit menu with the <paramref name="instanceToEdit"/>.
        ///</summary>
        private void OpenStockEditPanel(InventoryItemInstance instanceToEdit)
        {
            changeAmmountTargetItem = instanceToEdit;
            view.editPanel.OpenPanel(instanceToEdit);
        }

    #endregion

    #region Quantity Utils

        ///<summary>
        /// Returns the quantity left in stock for the given <paramref name="instanceToAdd"/>, considering the ammount of items already in the cart.
        ///</summary>
        private int MaxStockQuantity(InventoryItemInstance instanceToAdd)
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
    /// Loads the inventory and historyDatabases and populates the relevant binary search trees.
    ///</summary>
    private void LoadDatabases()
    {
        List<InventoryItemInstance> itemData;
        List<string> itemDataIDs = new List<string>();

        SavedDatabaseHandler.LoadDatabase<InventoryItemInstance>(out itemData);

        for (int i = 0; i < itemData.Count; i++)
        {
            itemDataIDs.Add(itemData[i].Item.Name.ToLower());
        }

        inventory = new BinaryST<InventoryItemInstance>(itemData, itemDataIDs);


        List<Order> orderData;
        List<string> orderDataIDs = new List<string>();

        SavedDatabaseHandler.LoadDatabase<Order>(out orderData);

        for (int i = 0; i < orderData.Count; i++)
        {
            orderDataIDs.Add(orderData[i].ClientName.ToLower());
        }

        orderHistory = new BinaryST<Order>(orderData, orderDataIDs);
    }

}
