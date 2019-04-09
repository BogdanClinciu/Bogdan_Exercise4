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

    [Header("User inputs")]
    [SerializeField]
    private InputField nameInput;
    [SerializeField]
    private InputField priceInput;
    [SerializeField]
    private InputField quantityInput;
    [SerializeField]
    private Slider discountSlider;

    [Header("Toggle buttons")]
    [SerializeField]
    private GameObject addNewButton;
    [SerializeField]
    private GameObject confirmEditButton;

    internal string NameInput {get => nameInput.text;}
    internal string PriceInput {get => priceInput.text;}
    internal string QuantityInput {get => quantityInput.text;}
    internal int DiscountInput {get => (int)discountSlider.value;}

    ///<summary>
    /// Updates the slider percent text, asigned to the discount sliderțs onValueChangesd field.
    ///</summary>
    public void UpdateSliderText()
    {
        percentText.text = discountSlider.value.ToString() + Constants.PERCENT;
    }

    ///<summary>
    /// Opens the add/edit panel and updates ui for adding a new item.
    ///</summary>
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

    ///<summary>
    /// Opens the add/edit panel and updates ui for editing the item <paramref name="instanceToEdit"/>.
    ///</summary>
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

    ///<summary>
    /// Displays the given <paramref name="errorString"/>.
    ///</summary>
    public void DisplayErrorMessage(string errorString)
    {
        errorText.text = errorString;
    }

    ///<summary>
    /// Closes the add/edit item panel.
    ///</summary>
    public void ClosePanel()
    {
        editItemParent.SetActive(false);
    }
}
