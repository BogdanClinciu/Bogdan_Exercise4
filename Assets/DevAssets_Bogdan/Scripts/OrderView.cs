using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderView : MonoBehaviour
{


    [SerializeField]
    private GameObject addItemWarning;

    [SerializeField]
    private GameObject ammountPromptParent;
    [SerializeField]
    private Text ammountPromptMaxText;
    [SerializeField]
    private GameObject ammountPromptWarningText;

    [SerializeField]
    private GameObject outgoingOrdersPanel;
    [SerializeField]
    private GameObject orderHistoryPanel;

    [SerializeField]
    private GameObject finalizeOrderWarning;

    [SerializeField]
    private GameObject placeOutgoingOrdersPanel;
    [SerializeField]
    private RectTransform placeOutgoingContentsRect;




    public void OpenOutgoingOrdersPanel()
    {
        outgoingOrdersPanel.SetActive(true);
        orderHistoryPanel.SetActive(false);
    }

    public void OpenOrderHistoryPanel()
    {
        orderHistoryPanel.SetActive(true);
        outgoingOrdersPanel.SetActive(false);
    }

    public void OpenInventoryPanel()
    {
        orderHistoryPanel.SetActive(false);
        outgoingOrdersPanel.SetActive(false);
    }


    public void ToggleAddItemWarning(bool show)
    {
        addItemWarning.SetActive(show);
    }

    public void ToggleAmmountPopup(bool show, int maxAmmount, int cartAmmount)
    {
        ammountPromptParent.SetActive(show);
        ammountPromptMaxText.text =
            Constants.AMMOUNT_INSTOCK_PREFIX + Constants.NEWLINE + maxAmmount + Constants.NEWLINE +
            Constants.AMMOUNT_INCART_PREFIX + Constants.NEWLINE + cartAmmount;
        ToggleAmmountPopupWarning(false);
    }

    public void ToggleAmmountPopupWarning(bool show)
    {
        ammountPromptWarningText.SetActive(show);
    }

    public void FinalizeOrderAction(bool validated)
    {
        if(!validated)
        {
            finalizeOrderWarning.SetActive(true);
            return;
        }

        finalizeOrderWarning.SetActive(false);
        OpenOutgoingOrdersPanel();
    }

    public void TogglePlaceOrdersPanel(bool show)
    {
        placeOutgoingOrdersPanel.SetActive(show);
        LayoutRebuilder.ForceRebuildLayoutImmediate(placeOutgoingContentsRect);
    }


    // #region Update object panel functions
    //     ///<summary>
    //     /// Creates the item objects from the given <paramref name="itemList"/> if they do not already exist in the provided <paramref name="spawnedList"/>, and disables ones that are not present.
    //     /// Objects are instantiated into the apropriate <paramref name="parentTransform"/>, and the <paramref name="interaction"/> enum determines the possible functions of that object.
    //     ///</summary>
    //     private void CreateItemObjects(Constants.ItemInteraction interaction, RectTransform parentTransform, List<InventoryItemInstance> itemList, List<ItemObject> spawnedList, bool destroyObjects)
    //     {
    //         if(spawnedList != null)
    //         {
    //             foreach (ItemObject item in spawnedList)
    //             {
    //                 if(!destroyObjects)
    //                 {
    //                     item.gameObject.SetActive(false);
    //                 }
    //                 else
    //                 {
    //                     Destroy(item.gameObject);
    //                 }
    //             }

    //             if(destroyObjects)
    //             {
    //                 spawnedList.Clear();
    //             }
    //         }

    //         foreach (InventoryItemInstance itemInstance in itemList)
    //         {
    //             if(spawnedList != null)
    //             {
    //                 if(!spawnedList.Exists(i => i.ID.Equals(itemInstance.Item.ID)))
    //                 {
    //                     ItemObject newItemObject = Instantiate(itemObjectPrefab, parentTransform).GetComponent<ItemObject>();
    //                     newItemObject.UpdateItemObject(interaction, itemInstance.Item.Name, itemInstance.Quantity, itemInstance.DiscountedPrice, () => ItemButtonAction(itemInstance, interaction), () => OpenStockEditPanel(itemInstance));
    //                     spawnedList.Add(newItemObject);
    //                 }
    //                 else
    //                 {
    //                     spawnedList.Find(i => i.ID.Equals(itemInstance.Item.ID)).UpdateItemObject(interaction, itemInstance.Item.Name, itemInstance.Quantity, itemInstance.DiscountedPrice, () => ItemButtonAction(itemInstance, interaction), () => OpenStockEditPanel(itemInstance));
    //                 }
    //             }
    //             else
    //             {
    //                 ItemObject newItemObject = Instantiate(itemObjectPrefab, parentTransform).GetComponent<ItemObject>();
    //                 newItemObject.UpdateItemObject(interaction, itemInstance.Item.Name, itemInstance.Quantity, itemInstance.DiscountedPrice, () => ItemButtonAction(itemInstance, interaction), () => OpenStockEditPanel(itemInstance));
    //             }
    //         }
    //     }

    //     private void CreateOrderObjects(bool canEdit, RectTransform parentTransform, List<Order> orderList, List<OrderObject> spawnedList, bool destroyObjects, bool isHistory)
    //     {
    //         foreach (OrderObject orderObject in spawnedList)
    //         {
    //             if(!destroyObjects)
    //             {
    //                 orderObject.gameObject.SetActive(false);
    //             }
    //             else
    //             {
    //                 Destroy(orderObject.gameObject);
    //             }
    //         }

    //         if(destroyObjects)
    //         {
    //             spawnedList.Clear();
    //         }

    //         foreach (Order order in orderList)
    //         {
    //             if(!spawnedList.Exists(i => i.ID.Equals(order.ID)))
    //             {
    //                 OrderObject newOrderObject = Instantiate(orderObjectPrefab, parentTransform).GetComponent<OrderObject>();
    //                 newOrderObject.UpdateOrderObject(canEdit, order.ClientName, order.TotalCost(), order.TotalDiscount(), () => OrderButtonAction(order, true), () => ShowOrderItems(order, isHistory));
    //                 spawnedList.Add(newOrderObject);
    //             }
    //             else
    //             {
    //                 spawnedList.Find(i => i.ID.Equals(order.ID)).UpdateOrderObject(canEdit, order.ClientName, order.TotalCost(), order.TotalDiscount(), () => OrderButtonAction(order, true), () => ShowOrderItems(order, isHistory));
    //             }
    //         }
    //     }
    // #endregion

}
