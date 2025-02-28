**Steps to add in an Inventory**

- Use the **Inventory Creator Window** (Tools > Inventory) to create an inventory.
  
- Alternatively, follow these steps:
  
  1\. Right-click in the Project window, go to Create > Inventory > Inventory.
  
  2\. Save the Inventory Scriptable Object in a suitable folder.
  
  3\. Add the Inventory script to a GameObject and assign the created Inventory Scriptable Object to it.
  
  4\. Set up the inventory settings (see Inventory Settings below).
  
  5\. Add a UI setup to display the inventory (see UI Setup below).

**Important Note:** The Inventory Scriptable Object is cloned when the game starts. So no changes made to it, will be saved after the game exits. If you want changes to persist, use Save/Load functions inside InventorySO script instead.
