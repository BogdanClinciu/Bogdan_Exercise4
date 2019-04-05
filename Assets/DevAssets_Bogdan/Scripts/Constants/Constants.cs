public class Constants
{
    public const string NEWLINE = "\n";
    public const string CURENCY_SYMBOL = "$";
    public const string CURENCY_FORMAT = "#0.00";
    public const string STOCK_LABEL = "Stock: ";
    public const string AMMOUNT_LABEL = "Ammount: ";
    public const string AMMOUNT_INSTOCK_PREFIX = "In stock:";
    public const string AMMOUNT_INCART_PREFIX = "In order:";

    public enum ItemInteraction
    {
        NoInteraction,
        AddToCart,
        RemoveFromCart
    }
}
