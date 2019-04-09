using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEditHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject editItemParent;

    [SerializeField]
    private Text errorText;
    [SerializeField]
    private Text percentText;

    [SerializeField]
    private InputField nameInput;
    [SerializeField]
    private InputField priceInput;
    [SerializeField]
    private InputField quantityInput;

    [SerializeField]
    private Slider discountSlider;

    [SerializeField]
    private GameObject addNewButton;
    [SerializeField]
    private GameObject confirmEditButton;

    internal string NameInput {get => nameInput.text;}
    internal string PriceInput {get => priceInput.text;}
    internal string QuantityInput {get => quantityInput.text;}
    internal int DiscountInput {get => (int)discountSlider.value;}

    public void UpdateSliderText()
    {
        percentText.text = discountSlider.value.ToString() + Constants.PERCENT;
    }

    public void OpenPanel()
    {
        nameInput.interactable = true;
        nameInput.text = string.Empty;
        priceInput.text = string.Empty;
        quantityInput.text = string.Empty;
        discountSlider.value = 0;
        editItemParent.SetActive(true);
        errorText.text = string.Empty;
        UpdateSliderText();
        addNewButton.SetActive(true);
        confirmEditButton.SetActive(false);
    }

    public void OpenPanel(InventoryItemInstance instanceToEdit)
    {
        nameInput.interactable = false;
        nameInput.text = instanceToEdit.Item.Name;
        priceInput.text = instanceToEdit.Item.BasePrice.ToString();
        quantityInput.text = instanceToEdit.Quantity.ToString();
        discountSlider.value = instanceToEdit.discount;
        editItemParent.SetActive(true);
        errorText.text = string.Empty;
        UpdateSliderText();
        addNewButton.SetActive(false);
        confirmEditButton.SetActive(true);
    }

    public void DisplayErrorMessage(string errorString)
    {
        errorText.text = errorString;
    }

    public void ClosePanel()
    {
        editItemParent.SetActive(false);
    }
}
