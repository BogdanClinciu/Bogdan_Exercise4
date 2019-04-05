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

}
