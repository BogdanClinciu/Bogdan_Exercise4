using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderController : MonoBehaviour
{
    [SerializeField]
    private OrderModel model;
    [SerializeField]
    private OrderView view;

    [Header("Inventory panel input")]
    [SerializeField]
    private InputField itemSearchField;
    [SerializeField]
    private InputField clientNameField;

    [Header("History panel")]
    [SerializeField]
    private InputField historyClientSearchField;

    [Header("Add new item panel")]
    [SerializeField]
    private InputField newItemNameField;
    [SerializeField]
    private InputField newItemPriceField;
    [SerializeField]
    private InputField newItemQuantityField;
    [SerializeField]
    private Slider newItemDiscountSlider;

    [Header("Ammount popup panel")]
    [SerializeField]
    private InputField ammountPopupInputField;


    [Header("Edit item panel")]
    [SerializeField]
    private Text editItemNameField;
    [SerializeField]
    private InputField editItemPriceField;
    [SerializeField]
    private InputField editItemQuantityField;
    [SerializeField]
    private Slider editItemDiscountSlider;

    ///<summary>
    /// Confirms the ammount to add to the curent order of the selected item.
    /// Triggered from the confirm button of the ammount popup panel
    ///</summary>
    public void ConfirmAmmountToAdd()
    {
        if(model.ConfirmAddItemToCurentOrder(ammountPopupInputField.text))
        {
            view.ToggleAmmountPopup(false,0,0);
            ammountPopupInputField.text = string.Empty;
        }
    }

    ///<summary>
    /// Triggers a search for items matching the item search field's contents.
    /// Triggered from the search button of the inventory item panel's search button
    ///</summary>
    public void SearchInventory()
    {
        //sort request to Model -> update request to View
        model.SearchInventory(itemSearchField.text);
    }

    ///<summary>
    /// Triggers the model to validate the new item and add it to the curent stock with the desired input parameters.
    /// Triggered from the confirm button of the add item panel. Also clears inputs on success.
    ///</summary>
    public void ConfirmNewItem()
    {
        if (model.ConfirmNewItemAdd(newItemNameField.text, newItemPriceField.text, newItemQuantityField.text, (int)newItemDiscountSlider.value))
        {
            view.ToggleAddItemPanel(true);
            newItemNameField.text = string.Empty;
            newItemPriceField.text = string.Empty;
            newItemQuantityField.text = string.Empty;
            newItemDiscountSlider.value = 0;
            Debug.Log("Added item");
        }
    }

    ///<summary>
    /// Triggers the finalize order action on both the view and the model using the model to validate input.
    ///</summary>
    public void FinalizeOrder()
    {
        view.FinalizeOrderAction(model.ConfirmFinalizeOrder(clientNameField.text));
        clientNameField.text = string.Empty;
    }

    ///<summary>
    /// Triggers the model to clear the curent order an return items to the stock
    ///</summary>
    public void ClearCurentOrder()
    {
        model.ClearCurentOrderPanel();
        clientNameField.text = string.Empty;
    }

    ///<summary>
    /// Triggers the first faze of placing all orders
    /// toggles the place orders panel and propms the model to create a sheet object for each outgoing order.
    ///</summary>
    public void PlaceAllOutgoingOrders()
    {
        view.TogglePlaceOrdersPanel(model.BeginPlaceOutgoingOrders());
    }

    ///<summary>
    /// Triggers placing(finishing) of all outgoing orders using the model to validate input.
    ///</summary>
    public void FinishPlaceAllOutgoingOrders()
    {
        model.ConfirmPlaceOutgoingOrders();
        view.TogglePlaceOrdersPanel(false);
    }

    ///<summary>
    /// Triggers a search for orders matching the order history search field's contents.
    /// Triggered from the search button of the history panel's search button
    ///</summary>
    public void SearchOrdersHistory()
    {
        model.SearchOrderHistory(historyClientSearchField.text);
    }


    ///<summary>
    /// Opens the item edit panel.
    ///</summary>
    public void OpenItemEditPanel(InventoryItemInstance instanceToEdit)
    {
        view.ToggleEditItemPanel(true);
        editItemNameField.text = instanceToEdit.Item.Name;
        editItemPriceField.text = Constants.CURENCY_SYMBOL + instanceToEdit.Item.BasePrice.ToString(Constants.CURENCY_FORMAT);
        editItemQuantityField.text = instanceToEdit.Quantity.ToString();
        editItemDiscountSlider.value = instanceToEdit.discount;
    }

    ///<summary>
    /// Opens the item edit panel.
    ///</summary>
    public void ConfirmItemEdit()
    {
        if(model.ConfirmStockEdit(editItemPriceField.text, editItemQuantityField.text, (int)editItemDiscountSlider.value))
        {
            view.ToggleEditItemPanel(false);
        }
        else
        {
            view.ToggleEditItemWarning(true);
        }
    }

    public void CancelItemEdit()
    {
        model.CancelItemEdit();
        view.ToggleEditItemPanel(false);
    }

}
