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

    #region Inventory panel inputs

        [Header("Inventory panel")]

        [SerializeField]
        private InputField itemSearchField;
        [SerializeField]
        private InputField clientNameField;

    #endregion

    #region Orfer panel inputs

        [Header("Orders panel")]

        [SerializeField]
        private InputField clientSearchField;

    #endregion

    #region History panel inputs

        [Header("History panel")]

        [SerializeField]
        private InputField historyClientSearchField;

    #endregion

    #region New item inputs

        [Header("Add new item panel")]

        [SerializeField]
        private GameObject addNewItemParent;

        [SerializeField]
        private InputField newItemNameField;
        [SerializeField]
        private InputField newItemPriceField;
        [SerializeField]
        private InputField newItemQuantityField;
        [SerializeField]
        private Slider newItemDiscountSlider;

    #endregion

    #region Ammount popup inputs

        [SerializeField]
        private InputField ammountPopupInputField;

    #endregion


    #region AmmountPopupLogic

        public void ConfirmAmmountChange()
        {
            if(model.AddItemToCurentOrder(ammountPopupInputField.text))
            {
                view.ToggleAmmountPrompt(false,0,0);
                ammountPopupInputField.text = string.Empty;
            }
        }

    #endregion

    #region Inventory panel actions
        public void SearchInventory()
        {
            //sort request to Model -> update request to View
            model.SearchInventory(itemSearchField.text);
        }

        public void ConfirmNewItem()
        {
            if (model.CompleteItemAdd(newItemNameField.text, newItemPriceField.text, newItemQuantityField.text, (int)newItemDiscountSlider.value))
            {
                addNewItemParent.SetActive(false); //<----- move to view
                newItemNameField.text = string.Empty;
                newItemPriceField.text = string.Empty;
                newItemQuantityField.text = string.Empty;
                newItemDiscountSlider.value = 0;
                Debug.Log("Added item");
            }
        }

        public void FinalizeOrder()
        {
            //send client name and list to Model -> view: update and show outgoing orders
        }

        public void ClearCurentOrder()
        {
            //send empty client name and list to Model -> view: update curent order panel
        }

        /*button gameobject functions
            Add new item - opens add new item panel
            Outgoing order panel toggle
            Order history panel toggle
        */
    #endregion


    #region Orders panel actions

        public void SearchOutgoingOrders()
        {
            //send client name to Model -> view: update outgoing orders
        }

    #endregion


    #region History panel actions

        public void SearchOrdersHistory()
        {
            //send client name to Model -> view: update outgoing orders
        }

    #endregion



}
