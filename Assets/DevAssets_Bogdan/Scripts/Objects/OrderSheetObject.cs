using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderSheetObject : MonoBehaviour
{
    [SerializeField]
    public RectTransform ItemObjectParent;

    [SerializeField]
    private Text clientNameText;
    [SerializeField]
    private Text totalCostText;
    [SerializeField]
    private Text totalDiscountText;


    public RectTransform UpdateOrderSheetItem(Order Order)
    {
        clientNameText.text = Order.ClientName;
        totalCostText.text = Constants.CURENCY_SYMBOL + Order.TotalCost().ToString(Constants.CURENCY_FORMAT);
        totalDiscountText.text = Constants.CURENCY_SYMBOL + Order.TotalDiscount().ToString(Constants.CURENCY_FORMAT);

        return ItemObjectParent;
    }
}
