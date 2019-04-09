public class Constants
{
    public const string NEWLINE = "\n";
    public const string PERCENT = "%";
    public const string CURENCY_SYMBOL = "$";
    public const string CURENCY_FORMAT = "#0.00";
    public const string STOCK_LABEL = "Stock: ";
    public const string AMMOUNT_LABEL = "Ammount: ";
    public const string AMMOUNT_INSTOCK_PREFIX = "In stock:";
    public const string AMMOUNT_INCART_PREFIX = "In order:";

    public const string ERROR_ADD = "An item with this name already exists, please edit that item";
    public const string ERROR_EDIT = "Please enter valid ammounts for price and quantity";

    public const string DIDSCOUNT_OLD_BEGIN = "<size=10><color=#800000ff>";
    public const string DIDSCOUNT_OLD_END = "</color></size>";

    public const string DIDSCOUNT_NEW_BEGIN = "<color=#00ff00ff>";
    public const string DIDSCOUNT_NEW_END = "</color>";

    public const string OLD_PRICE_LABEL = "Old price: ";

    public const float DOUBLE_CLICK_TIME = 0.25f;

    public enum ItemInteraction
    {
        NoInteraction,
        AddToCart,
        RemoveFromCart
    }
}
