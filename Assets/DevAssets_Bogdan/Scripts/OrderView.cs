using UnityEngine;
using UnityEngine.UI;

public class OrderView : MonoBehaviour
{


    [SerializeField]
    private GameObject addItemWarning;

    [SerializeField]
    private GameObject ammountPromptParent;
    [SerializeField]
    private Text ammountPromptMaxText;
    [SerializeField]
    private GameObject ammountPromptWarningText;

    [SerializeField]
    private GameObject outgoingOrdersPanel;
    [SerializeField]
    private GameObject orderHistoryPanel;


    [SerializeField]
    private GameObject finalizeOrderWarning;


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

    public void ToggleAmmountPrompt(bool show, int maxAmmount, int cartAmmount)
    {
        ammountPromptParent.SetActive(show);
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

}
