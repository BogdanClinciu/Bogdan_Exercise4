using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OrderView : MonoBehaviour
{
    [SerializeField]
    internal ItemEditHandler editPanel;

    [Header("Object prefabs")]
    [SerializeField]
    private GameObject itemObjectPrefab;
    [SerializeField]
    private GameObject orderObjectPrefab;
    [SerializeField]
    private GameObject orderSheetObjectPrefab;

    [Header("Panel parents")]
    [SerializeField]
    private GameObject outgoingOrdersPanel;
    [SerializeField]
    private GameObject orderHistoryPanel;
    [SerializeField]
    private GameObject placeOutgoingOrdersPanel;

    [Header("Warning texts")]
    [SerializeField]
    private GameObject finalizeOrderWarning;

    [Header("Ammount panel ui")]
    [SerializeField]
    private GameObject ammountPpopupParent;
    [SerializeField]
    private Text ammountPromptMaxText;
    [SerializeField]
    private GameObject ammountPromptWarningText;

    [Header("UI rebuild rects")]
    [SerializeField]
    private RectTransform placeOutgoingContentsRect;

    ///<summary>
    /// Opens the outgoing orders pannel
    ///</summary>
    public void OpenOutgoingOrdersPanel()
    {
        outgoingOrdersPanel.SetActive(true);
        orderHistoryPanel.SetActive(false);
    }

    ///<summary>
    /// Opens the orders history pannel
    ///</summary>
    public void OpenOrderHistoryPanel()
    {
        orderHistoryPanel.SetActive(true);
        outgoingOrdersPanel.SetActive(false);
    }

    ///<summary>
    /// Opens the inventory and curent order pannel
    ///</summary>
    public void OpenInventoryPanel()
    {
        orderHistoryPanel.SetActive(false);
        outgoingOrdersPanel.SetActive(false);
    }

    ///<summary>
    /// Toggles the ammount pupup to <paramref name="show"/> with the given <paramref name="maxAmmount"/> and <paramref name="cartAmmount"/>.
    ///</summary>
    internal void ToggleAmmountPopup(bool show, int maxAmmount, int cartAmmount)
    {
        ammountPpopupParent.SetActive(show);
        ammountPromptMaxText.text =
            Constants.AMMOUNT_INSTOCK_PREFIX + Constants.NEWLINE + maxAmmount + Constants.NEWLINE +
            Constants.AMMOUNT_INCART_PREFIX + Constants.NEWLINE + cartAmmount;
        ToggleAmmountPopupWarning(false);
    }

    ///<summary>
    /// Toggles the ammount pupup warning to <paramref name="show"/>.
    ///</summary>
    internal void ToggleAmmountPopupWarning(bool show)
    {
        ammountPromptWarningText.SetActive(show);
    }

    ///<summary>
    /// Closes opens the outgoing orders panel if the  <paramref name="validated"/> parameter is true, else it displays a preset error message.
    ///</summary>
    internal void FinalizeOrderAction(bool validated)
    {
        if(!validated)
        {
            finalizeOrderWarning.SetActive(true);
            return;
        }

        finalizeOrderWarning.SetActive(false);
        OpenOutgoingOrdersPanel();
    }

    ///<summary>
    /// Toggles the placed orders (order sheets) panel to <paramref name="show"/>.
    ///</summary>
    internal void TogglePlaceOrdersPanel(bool show)
    {
        placeOutgoingOrdersPanel.SetActive(show);
        LayoutRebuilder.ForceRebuildLayoutImmediate(placeOutgoingContentsRect);
    }

    #region Object Updating/Creation

        ///<summary>
        /// Creates the order sheet objects from the given <paramref name="outgoingOrders"/>, clears the <paramref name="spawnedOrderSheets"/>.
        /// Objects are instantiated into the apropriate <paramref name="orderSheetParent"/>.
        ///</summary>
        internal void UpdateSheetObjects(
            RectTransform orderSheetParent,
            List<Order> outgoingOrders,
            List<OrderSheetObject> spawnedOrderSheets,
            UnityAction<InventoryItemInstance,Constants.ItemInteraction> buttonAction,
            UnityAction<InventoryItemInstance> doubleClickAction
            )
        {
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
                    false,
                    buttonAction,
                    doubleClickAction
                );

                spawnedOrderSheets.Add(newOrderSheet);
            }
        }

        ///<summary>
        /// Creates the item objects from the given <paramref name="itemList"/> if they do not already exist in the provided <paramref name="spawnedList"/>, and disables ones that are not present.
        /// Objects are instantiated into the apropriate <paramref name="parentTransform"/>, and the <paramref name="interaction"/> enum determines the possible functions of the updated objects.
        ///</summary>
        internal void UpdateItemObjects(
            Constants.ItemInteraction interaction,
            RectTransform parentTransform,
            List<InventoryItemInstance> itemList,
            List<ItemObject> spawnedList,
            bool destroyObjects,
            UnityAction<InventoryItemInstance,Constants.ItemInteraction> buttonAction,
            UnityAction<InventoryItemInstance> doubleClickAction
            )
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
                        newItemObject.UpdateItemObject(interaction, itemInstance, () => buttonAction(itemInstance, interaction), () => doubleClickAction(itemInstance));
                        spawnedList.Add(newItemObject);
                    }
                    else
                    {
                        spawnedList.Find(i => i.ID.Equals(itemInstance.Item.ID)).UpdateItemObject(interaction, itemInstance, () => buttonAction(itemInstance, interaction), () => doubleClickAction(itemInstance));
                    }
                }
                else
                {
                    ItemObject newItemObject = Instantiate(itemObjectPrefab, parentTransform).GetComponent<ItemObject>();
                    newItemObject.UpdateItemObject(interaction, itemInstance, () => buttonAction(itemInstance, interaction), () => doubleClickAction(itemInstance));
                }
            }
        }

        ///<summary>
        /// Creates the order objects from the given <paramref name="orderList"/> if they do not already exist in the provided <paramref name="spawnedList"/>, and disables ones that are not present.
        /// Objects are instantiated into the apropriate <paramref name="parentTransform"/>, and the <paramref name="canEdit"/> bool determines the presence of a remove function for the updated objects.
        ///</summary>
        internal void UpdateOrderObjects(
            bool canEdit,
            RectTransform parentTransform,
            List<Order> orderList,
            List<OrderObject> spawnedList,
            bool destroyObjects,
            bool isHistory,
            UnityAction<Order,bool> removeAction,
            UnityAction<Order,bool> viewAction
            )
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
                    newOrderObject.UpdateOrderObject(canEdit, order.ClientName, order.TotalCost(), order.TotalDiscount(), () => removeAction(order, true), () => viewAction(order, isHistory));
                    spawnedList.Add(newOrderObject);
                }
                else
                {
                    spawnedList.Find(i => i.ID.Equals(order.ID)).UpdateOrderObject(canEdit, order.ClientName, order.TotalCost(), order.TotalDiscount(), () => removeAction(order, true), () => viewAction(order, isHistory));
                }
            }
        }

    #endregion
}
