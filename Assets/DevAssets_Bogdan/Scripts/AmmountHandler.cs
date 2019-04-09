using UnityEngine;
using UnityEngine.UI;

public class AmmountHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject ammountParent;
    [SerializeField]
    private GameObject errorText;
    [SerializeField]
    private InputField ammountInputField;
    [SerializeField]
    private Text maxAmmountText;


    internal string AmmountInput { get => ammountInputField.text;}


    internal void OpenAmmountPanel(int maxAmmount, int cartAmmount)
    {
        maxAmmountText.text =
            Constants.AMMOUNT_INSTOCK_PREFIX + Constants.NEWLINE + maxAmmount + Constants.NEWLINE +
            Constants.AMMOUNT_INCART_PREFIX + Constants.NEWLINE + cartAmmount;
        ammountInputField.text = string.Empty;
        ammountParent.SetActive(true);
        ShowErrorMessage(false);
    }

    internal void ShowErrorMessage(bool show)
    {
        errorText.SetActive(show);
    }

    internal void CloseAmmountPanel()
    {
        ammountParent.SetActive(false);
    }
}
