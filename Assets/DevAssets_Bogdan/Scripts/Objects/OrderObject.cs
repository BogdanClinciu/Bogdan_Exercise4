using UnityEngine;
using UnityEngine.UI;

public class OrderObject : MonoBehaviour
{
    public string ID
    {
        get;
        private set;
    }

    [SerializeField]
    private Text clientNameText;
    [SerializeField]
    private Text totalCostText;
    [SerializeField]
    private Text totalDiscountText;

    [SerializeField]
    private Button editButton;


    public void UpdateItemObject(bool canEdit, string clientName, float totalCost, float totalDiscount, UnityEngine.Events.UnityAction clickAction)
    {
        editButton.gameObject.SetActive(true);

        ID = clientName.ToLower();
        clientNameText.text = clientName;
        totalCostText.text = Constants.CURENCY_SYMBOL + totalCost.ToString(Constants.CURENCY_FORMAT);
        totalDiscountText.text = Constants.CURENCY_SYMBOL + totalDiscount.ToString(Constants.CURENCY_FORMAT);

        editButton.onClick.AddListener(clickAction);

        gameObject.SetActive(true);
    }

}
