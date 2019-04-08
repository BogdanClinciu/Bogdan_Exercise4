using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderModel : MonoBehaviour
{
    [SerializeField]
    private OrderController controller;
    [SerializeField]
    private OrderView view;

    #region ObjectPrefabs
        [Header("Object prefabs")]

        [SerializeField]
        private GameObject itemObjectPrefab;
        [SerializeField]
        private GameObject orderObjectPrefab;
        [SerializeField]
        private GameObject orderSheetObjectPrefab;
    #endregion

    #region Data sets

        private BinaryST<InventoryItemInstance> inventory;
        private BinaryST<Order> orderHistory;

        private List<InventoryItemInstance> itemsInCurentOrder;
        private List<Order> outgoingOrders;

    #endregion

    #region Spawned item Parents and lists
        [Header("Inventory/CurentOrder spawn parents")]
        #region Invenory and Outgoing
            [SerializeField]
            private RectTransform inventoryItemParent;
            [SerializeField]
            private RectTransform currentOrderItemParent;

            private List<ItemObject> spawnedInventoryItemObjects;
            private List<ItemObject> spawnedItemObjectsInOrder;
        #endregion

        [Header("Order history spawn parents")]
        #region OrderHistory
            [SerializeField]
            private RectTransform orderHistoryParent;
            [SerializeField]
            private RectTransform orderHistoryItemPanel;

            private List<OrderObject> spawnedOrderHistoryObjects;
            private List<ItemObject> spawnedHistoryItemObjects;
        #endregion

        [Header("Outgoing orders spawn parents")]
        #region Outgoing orders
            [SerializeField]
            private RectTransform outgoingOrdersParent;
            [SerializeField]
            private RectTransform outgoingOrdersItemParent;

            private List<OrderObject> spawnedOutgoingOrderObjects;
            private List<ItemObject> spawnedOutgoingItemObjects;
        #endregion

        [Header("OrderSheet spawn parents")]
        #region Order Sheets (placed orders)
            [SerializeField]
            private RectTransform orderSheetParent;

            private List<OrderSheetObject> spawnedOrderSheets;
        #endregion

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

            inventory = new BinaryST<InventoryItemInstance>();
            orderHistory = new BinaryST<Order>();
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

        UpdateItemObjects(Constants.ItemInteraction.RemoveFromCart, currentOrderItemParent, itemsInCurentOrder, spawnedItemObjectsInOrder, true);
        changeAmmountTargetItem = null;
        return true;
    }


    #region Order finalization and placement logic

        ///<summary>
        /// Validates outgoing orders and on success creates sheet objects for each order.
        ///</summary>
        public bool BeginPlaceOutgoingOrders()
    {
        if(outgoingOrders.Count < 1)
        {
            return false;
        }


        //clear placed orders from before
        foreach (OrderSheetObject sheet in spawnedOrderSheets)
        {
            Destroy(sheet.gameObject);
        }
        spawnedOrderSheets.Clear();

        //populate the place orders panel
        foreach (Order order in outgoingOrders)
        {
            OrderSheetObject newOrderSheet = Instantiate(orderSheetObjectPrefab, orderSheetParent).GetComponent<OrderSheetObject>();
            newOrderSheet.UpdateOrderSheetItem(order);
            RectTransform newOrderSheetParent = newOrderSheet.GetComponent<RectTransform>();
            UpdateItemObjects(
                Constants.ItemInteraction.NoInteraction,
                newOrderSheetParent,
                order.Items,
                null,
                false
            );

            spawnedOrderSheets.Add(newOrderSheet);
        }

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
            UpdateOrderObjects(false, outgoingOrdersParent, outgoingOrders, spawnedOutgoingOrderObjects, true, false);

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
                UpdateOrderObjects(true, outgoingOrdersParent, outgoingOrders, spawnedOutgoingOrderObjects, true, false);
            }
            else
            {
                outgoingOrders.Add(new Order(clientName.ToLower(), clientName, new List<InventoryItemInstance>(itemsInCurentOrder)));
                UpdateOrderObjects(true, outgoingOrdersParent, outgoingOrders, spawnedOutgoingOrderObjects, true, false);
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
            UpdateItemObjects(Constants.ItemInteraction.NoInteraction, currentOrderItemParent, itemsInCurentOrder, spawnedItemObjectsInOrder, true);
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
                    searchResults.Add(inventory.GetNodeAt(searchQuery.ToLower()).NodeValue);
                    UpdateItemObjects(Constants.ItemInteraction.AddToCart, inventoryItemParent, searchResults, spawnedInventoryItemObjects, false);
                    return;
                }

                //display items matching
                searchResults = inventory.All(item => item.id.ToLower().StartsWith(searchQuery.ToLower()));
                UpdateItemObjects(Constants.ItemInteraction.AddToCart, inventoryItemParent, searchResults, spawnedInventoryItemObjects, false);
                return;
            }

            //display all items
            searchResults = inventory.ToOrderedList();
            UpdateItemObjects(Constants.ItemInteraction.AddToCart, inventoryItemParent, searchResults, spawnedInventoryItemObjects, false);

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
                    searchResults.Add(orderHistory.GetNodeAt(searchQuery.ToLower()).NodeValue);
                    UpdateOrderObjects(false, orderHistoryParent, searchResults, spawnedOrderHistoryObjects, false, true);
                    return;
                }

                //display items matching
                searchResults = orderHistory.All(item => item.id.ToLower().StartsWith(searchQuery.ToLower()));
                UpdateOrderObjects(false, orderHistoryParent, searchResults, spawnedOrderHistoryObjects, false, true);
                return;
            }

            //display all items
            searchResults = orderHistory.ToOrderedList();
            UpdateOrderObjects(false, orderHistoryParent, searchResults, spawnedOrderHistoryObjects, false, true);
        }

    #endregion

    //Private from here
    #region Object update/creation functions

        ///<summary>
        /// Creates the item objects from the given <paramref name="itemList"/> if they do not already exist in the provided <paramref name="spawnedList"/>, and disables ones that are not present.
        /// Objects are instantiated into the apropriate <paramref name="parentTransform"/>, and the <paramref name="interaction"/> enum determines the possible functions of the updated objects.
        ///</summary>
        private void UpdateItemObjects(
            Constants.ItemInteraction interaction,
            RectTransform parentTransform,
            List<InventoryItemInstance> itemList,
            List<ItemObject> spawnedList,
            bool destroyObjects)
        {
            if(spawnedList != null)
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
            }

            foreach (InventoryItemInstance itemInstance in itemList)
            {
                if(spawnedList != null)
                {
                    if(!spawnedList.Exists(i => i.ID.Equals(itemInstance.Item.ID)))
                    {
                        ItemObject newItemObject = Instantiate(itemObjectPrefab, parentTransform).GetComponent<ItemObject>();
                        newItemObject.UpdateItemObject(interaction, itemInstance.Item.Name, itemInstance.Quantity, itemInstance.DiscountedPrice, () => ItemButtonAction(itemInstance, interaction), () => OpenStockEditPanel(itemInstance));
                        spawnedList.Add(newItemObject);
                    }
                    else
                    {
                        spawnedList.Find(i => i.ID.Equals(itemInstance.Item.ID)).UpdateItemObject(interaction, itemInstance.Item.Name, itemInstance.Quantity, itemInstance.DiscountedPrice, () => ItemButtonAction(itemInstance, interaction), () => OpenStockEditPanel(itemInstance));
                    }
                }
                else
                {
                    ItemObject newItemObject = Instantiate(itemObjectPrefab, parentTransform).GetComponent<ItemObject>();
                    newItemObject.UpdateItemObject(interaction, itemInstance.Item.Name, itemInstance.Quantity, itemInstance.DiscountedPrice, () => ItemButtonAction(itemInstance, interaction), () => OpenStockEditPanel(itemInstance));
                }
            }
        }

        ///<summary>
        /// Creates the order objects from the given <paramref name="orderList"/> if they do not already exist in the provided <paramref name="spawnedList"/>, and disables ones that are not present.
        /// Objects are instantiated into the apropriate <paramref name="parentTransform"/>, and the <paramref name="canEdit"/> bool determines the presence of a remove function for the updated objects.
        ///</summary>
        private void UpdateOrderObjects(
            bool canEdit,
            RectTransform parentTransform,
            List<Order> orderList,
            List<OrderObject> spawnedList,
            bool destroyObjects,
            bool isHistory)
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
                if(!spawnedList.Exists(i => i.ID.Equals(order.ID)))
                {
                    OrderObject newOrderObject = Instantiate(orderObjectPrefab, parentTransform).GetComponent<OrderObject>();
                    newOrderObject.UpdateOrderObject(canEdit, order.ClientName, order.TotalCost(), order.TotalDiscount(), () => OrderButtonAction(order, true), () => ShowOrderItems(order, isHistory));
                    spawnedList.Add(newOrderObject);
                }
                else
                {
                    spawnedList.Find(i => i.ID.Equals(order.ID)).UpdateOrderObject(canEdit, order.ClientName, order.TotalCost(), order.TotalDiscount(), () => OrderButtonAction(order, true), () => ShowOrderItems(order, isHistory));
                }
            }
        }

    #endregion

    #region Order object actions

        ///<summary>
        /// Order object click action.
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

        ///<summary>
        /// Removes the <paramref name="orderToRemove"/> from the outgoing orders list and updates the outgoing order objects
        ///</summary>
        private void RemoveFromOutgoingOrders(Order orderToRemove)
        {
            if(outgoingOrders.Contains(orderToRemove))
            {
                outgoingOrders.Remove(orderToRemove);
            }
            UpdateOrderObjects(true, outgoingOrdersParent, outgoingOrders, spawnedOutgoingOrderObjects, true, false);
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
        /// Shows the <paramref name="order"/>'s items in the apropriate panel defined by the <paramref name="isHistory"/> parameter.
        ///</summary>
        private void ShowOrderItems(Order order, bool isHistory)
        {
            if(order != null)
            {
                UpdateItemObjects(
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
            view.ToggleAmmountPopup(true, MaxStockQuantity(instanceToAdd), CartQuantity(instanceToAdd));
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
            UpdateItemObjects(Constants.ItemInteraction.RemoveFromCart, currentOrderItemParent, itemsInCurentOrder, spawnedItemObjectsInOrder, true);
        }

        ///<summary>
        /// Opens the stock edit menu.
        ///</summary>
        private void OpenStockEditPanel(InventoryItemInstance instanceToEdit)
        {
            changeAmmountTargetItem = instanceToEdit;
            controller.OpenItemEditPanel(instanceToEdit);
        }

    #endregion

    #region Quantity Utils

        ///<summary>
        /// Returns the quantity left in stock for the given <paramref name="instanceToAdd"/>.
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
    /// Loads the item, order and historyDatabases and populates the relevant binary search trees.
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
