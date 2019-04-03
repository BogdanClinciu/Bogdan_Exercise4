using UnityEngine;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour
{
    public string ID
    {
        get;
        private set;
    }

    [SerializeField]
    private Text itemNameText;
    [SerializeField]
    private Text stockText;
    [SerializeField]
    private Text priceText;

    [SerializeField]
    private Button editButton;

    [SerializeField]
    private GameObject addToCartImage;
    [SerializeField]
    private GameObject removeCartImage;


    public void UpdateItemObject(Constants.ItemInteraction interactionType, string itemName, int stock, float price, UnityEngine.Events.UnityAction clickAction)
    {
        switch (interactionType)
        {
            case Constants.ItemInteraction.NoInteraction:
            {
                editButton.gameObject.SetActive(false);
                break;
            }
            case Constants.ItemInteraction.AddToCart:
            {
                editButton.gameObject.SetActive(true);
                addToCartImage.SetActive(true);
                removeCartImage.SetActive(false);
                break;
            }
            case Constants.ItemInteraction.RemoveFromCart:
            {
                editButton.gameObject.SetActive(true);
                addToCartImage.SetActive(false);
                removeCartImage.SetActive(true);
                break;
            }
            default:
            {
                break;
            }
        }

        ID = itemName.ToLower();
        itemNameText.text = itemName;
        stockText.text = stock.ToString();
        priceText.text = Constants.CURENCY_SYMBOL + price.ToString(Constants.CURENCY_FORMAT);
        editButton.onClick.AddListener(clickAction);
        gameObject.SetActive(true);
    }

}
