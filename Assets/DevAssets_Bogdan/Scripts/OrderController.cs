using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderController : MonoBehaviour
{
    [SerializeField]
    private OrderModel model;

    [Header("Inventory panel")]

    [SerializeField]
    private InputField itemSearchField;
    [SerializeField]
    private InputField clientNameField;


    [Header("Orders panel")]

    [SerializeField]
    private InputField clientSearchField;

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


    #region Inventory panel actions
        public void SearchInventory()
        {
            //sort request to Model -> update request to View
        }

        public void ConfirmNewItem()
        {
            if(OrderModel.ItemExists(newItemNameField.text.ToLower()))
            {
                // view - toggle add item warning
            }
            else
            {
                model.CompleteItemAdd(newItemNameField.text, newItemPriceField.text, newItemQuantityField.text, (int)newItemDiscountSlider.value);
            }
            // string s = "sdfasd";
            // string i = s.GetHashCode().ToString();

            // if(s.CompareTo(i))
            // {

            // }

            //add request to model -> update request to View
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
