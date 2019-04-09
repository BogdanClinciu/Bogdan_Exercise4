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

    [Header("Ammount popup panel")]
    [SerializeField]
    private InputField ammountPopupInputField;


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
        if (model.ConfirmNewItemAdd(view.editPanel.NameInput, view.editPanel.PriceInput, view.editPanel.QuantityInput, view.editPanel.DiscountInput))
        {
            view.editPanel.ClosePanel();
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
    /// Confirms editing is finished on an item.
    ///</summary>
    public void ConfirmItemEdit()
    {
        if(model.ConfirmStockEdit(view.editPanel.PriceInput, view.editPanel.QuantityInput, view.editPanel.DiscountInput))
        {
            view.editPanel.ClosePanel();
        }
        else
        {
            view.editPanel.DisplayErrorMessage(Constants.ERROR_EDIT);
        }
    }

    ///<summary>
    /// Closes the edit panel and ends item editing.
    ///</summary>
    public void CancelItemEdit()
    {
        model.CancelItemEdit();
        view.editPanel.ClosePanel();
    }

}
