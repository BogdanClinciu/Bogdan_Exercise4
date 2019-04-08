using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OrderView : MonoBehaviour
{

    [Header("Panel parents")]
    [SerializeField]
    private GameObject outgoingOrdersPanel;
    [SerializeField]
    private GameObject orderHistoryPanel;
    [SerializeField]
    private GameObject addNewItemParent;
    [SerializeField]
    private GameObject editItemParent;
    [SerializeField]
    private GameObject placeOutgoingOrdersPanel;

    [Header("Warning texts")]
    [SerializeField]
    private GameObject addItemWarning;
    [SerializeField]
    private GameObject editItemWarning;
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

    [Header("Discount Slider Texts and sliders")]
    [SerializeField]
    private Text discountSliderText;
    [SerializeField]
    private Text editDiscountSliderText;
    [SerializeField]
    private Slider newItemDiscountSlider;
    [SerializeField]
    private Slider editItemDiscountSlider;

    public void UpdateDiscountLabel(bool isEdit)
    {
        if(isEdit)
        {
            editDiscountSliderText.text = editItemDiscountSlider.value.ToString();
        }
        else
        {
            discountSliderText.text = newItemDiscountSlider.value.ToString();
        }
    }

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
        ammountPpopupParent.SetActive(show);
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

    public void ToggleAddItemPanel(bool show)
    {
        newItemDiscountSlider.value = 0;
        discountSliderText.text = Constants.PERCENT;
        addNewItemParent.SetActive(show);
    }

    public void ToggleEditItemPanel(bool show)
    {
        editItemDiscountSlider.value = 0;
        editDiscountSliderText.text = Constants.PERCENT;
        ToggleEditItemWarning(false);
        editItemParent.SetActive(show);
    }

    public void ToggleEditItemWarning(bool show)
    {
        editItemWarning.SetActive(show);
    }
}
