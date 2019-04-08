using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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
    private Text stockLabelText;

    [SerializeField]
    private Button editButton;

    [SerializeField]
    private GameObject addToCartImage;
    [SerializeField]
    private GameObject removeCartImage;

    private bool isDoubleClickEditable = false;
    private float lastClickTime = 0.0f;

    private UnityAction doubleClickAction;


    private void OnDestroy()
    {
        editButton.onClick.RemoveAllListeners();
    }

    ///<summary>
    ///The double click action of this item object, triggered from the gameobject's event trigger component (onClick).
    ///</summary>
    public void DoubleClickAction()
    {
        if(isDoubleClickEditable)
        {
            if(Time.time - lastClickTime > Constants.DOUBLE_CLICK_TIME)
            {
                lastClickTime = Time.time;
            }
            else
            {
                lastClickTime = 0.0f;
                doubleClickAction.Invoke();
            }
        }
    }


    ///<summary>
    ///Updates the item object's interface and assigns its button and double click actions.
    ///</summary>
    public void UpdateItemObject(Constants.ItemInteraction interactionType, InventoryItemInstance instance, UnityAction clickAction, UnityAction doubleAction)
    {
        switch (interactionType)
        {
            case Constants.ItemInteraction.NoInteraction:
            {
                isDoubleClickEditable = false;
                stockLabelText.text = Constants.AMMOUNT_LABEL;
                editButton.gameObject.SetActive(false);
                break;
            }
            case Constants.ItemInteraction.AddToCart:
            {
                isDoubleClickEditable = true;
                stockLabelText.text = Constants.STOCK_LABEL;
                editButton.gameObject.SetActive(true);
                addToCartImage.SetActive(true);
                removeCartImage.SetActive(false);
                break;
            }
            case Constants.ItemInteraction.RemoveFromCart:
            {
                isDoubleClickEditable = false;
                stockLabelText.text = Constants.AMMOUNT_LABEL;
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

        ID = instance.Item.Name.ToLower();
        itemNameText.text = instance.Item.Name;
        stockText.text = instance.Quantity.ToString();

        priceText.text = (instance.discount > 0) ?
            Constants.DIDSCOUNT_OLD_BEGIN + Constants.CURENCY_SYMBOL +  instance.Item.BasePrice + " -" + instance.discount + Constants.PERCENT + Constants.DIDSCOUNT_OLD_END + Constants.NEWLINE +
            Constants.DIDSCOUNT_NEW_BEGIN + Constants.CURENCY_SYMBOL + instance.DiscountedPrice.ToString(Constants.CURENCY_FORMAT) + Constants.DIDSCOUNT_NEW_END :

            Constants.CURENCY_SYMBOL + instance.DiscountedPrice.ToString(Constants.CURENCY_FORMAT);

        editButton.onClick.AddListener(clickAction);
        doubleClickAction = doubleAction;
        gameObject.SetActive(true);
    }
}
