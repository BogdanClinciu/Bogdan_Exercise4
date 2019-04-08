# Bogdan_Exercise4
# Description:
            	
Create an order management application. Using the UI, a user should be able to add new items to the inventory and place orders for customers.
Adding items:
·         When making the selection to add an item, the user should be presented with a screen asking for an item name and quantity
·         If the item already exists, the new quantity is added to the existing one
·         If the item does not exist, a new entry is made for the item and its quantity
Creating order:
·         When making the selection to create an order, the user should be presented with a window asking for the customer’s name
·         If a customer already has an order placed, the new order is added on top of the old one
·         If a customer does not have an order placed, a new order is created for them
·         The user can see a list of the inventory with names and quantities
·         Double clicking on an item will prompt the user to enter a quantity
o   If the quantity is greater than the available stock, the user should be warned and asked to insert a different quantity
·         After the item is added to the order, the order details are refreshed
Finalizing orders:
·         Clicking on finalize orders will open a window showing all the orders, grouped by the client’s name
·         If the list looks ok, clicking “Proceed” should send the orders (print to pdf) and the order list should be emptied
 
Use Binary Search Trees for managing the inventory and the orders.

EXTRA 1 : Structure the app around MVC pattern ( Model-View-Controller )
EXTRA 2 : Order History. In the history, every order should remain the same, even if things have changed. For example : an item is priced at $100 in the order. If the item changes it’s value to $125, the order should still display $100.
EXTRA 3 : Ability to easily create sales for items. ( Requires some restructuring or planning ahead )



# Order manager:
The order manager allows the user to keep and edit stock items and outgoing orders containing said items. Data is saved in User\AppData\LocalLow\Exercises\Bogdan_Exercise_4

# Design
The application implements the MVC design structure:
1. The model: OrderModel.cs manages data operations (searching, editing, addition, removal) and creates the objects that represent items,  orders and order sheets.
2. The controller: OrderController.cs handles most of the user inputs.
3. The view: OrderView.cs toggles and updates UI elements.

# Data type:
Permanent data is saved as json but when loaded into the application it is organised into a binary search tree for quick access. This data includes item data and order history.
Temporary data such as outgoing orders is stored only at runtime within the application.

# Additional notes
- SaveDatabaseHandler.cs is used to load/save and assign the main saved data sets (inventory items and order history).
- SaveDatabase.cs type has been created to neatly save a binary search tree into a serialized list.
- Objects: ItemObject.cs, OrderObject.cs and OrderSheetObject.cs handle their own UI and interactions as they are instantiated by the model.
- BinaryST.cs contains the binary search tree with associated lookup, addition, removal etc. functions. The search tree has been made with a flexible type and virtually any class be used as the node value.
- InventoryItem.cs and InventoryItemInstance.cs have been created in order to keep base item statistics and instance values such as quantity and discount value separate from their instances.
- Constants.cs contains commonly used values
