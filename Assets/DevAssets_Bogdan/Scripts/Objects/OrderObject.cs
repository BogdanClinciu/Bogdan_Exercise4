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
    [SerializeField]
    private Button viewItemsButton;


    private void OnDestroy()
    {
        editButton.onClick.RemoveAllListeners();
        viewItemsButton.onClick.RemoveAllListeners();
    }

    public void UpdateOrderObject(bool canEdit, string clientName, float totalCost, float totalDiscount, UnityEngine.Events.UnityAction removeAction, UnityEngine.Events.UnityAction viewAction)
    {
        editButton.gameObject.SetActive(canEdit);

        ID = clientName.ToLower();
        clientNameText.text = clientName;
        totalCostText.text = Constants.CURENCY_SYMBOL + totalCost.ToString(Constants.CURENCY_FORMAT);
        totalDiscountText.text = Constants.CURENCY_SYMBOL + totalDiscount.ToString(Constants.CURENCY_FORMAT);

        editButton.onClick.AddListener(removeAction);
        viewItemsButton.onClick.AddListener(viewAction);

        gameObject.SetActive(true);
    }
}
