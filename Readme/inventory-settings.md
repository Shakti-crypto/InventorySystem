**Inventory References:**

**1\. Inventory UI:**

- This is a reference to the **UI script** that displays the inventory.
- If you don’t want to create a custom UI or are using a separate UI system, you can leave this field empty.
- Assign the UI script reference back in the **Inventory script** to link the inventory with its UI.

**2\. Inventory SO:**

- This is a reference to the **Inventory Scriptable Object** that contains all the inventory settings.
- The **Inventory script** uses this Scriptable Object to display and manage the inventory.
- **Important Note**: Changes made to the inventory during runtime (e.g., adding or removing items) will not reflect back in the Scriptable Object. To persist changes, use the SaveInventory() and LoadInventory() functions.

**Inventory Settings:**

**1\. Fixed Slots:** Enabling this will make the slots undeletable.

| ![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/FixedSlotsDisabled.png)<br><br>**Fixed Slots Disabled**<br><br>Empty Slots will be removed automatically.<br><br>Additional slots will be added when needed<br><br>(Total number of slots can be less than or equal to ‘maxNumberOfSlots’ property) | ![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/FixedSlotsEnabled.png)<br><br>**Fixed Slots Enabled**<br><br>Slot deletion not allowed. They will be shown empty instead.<br><br>No extra slots can be added at runtime. |
| --- | --- |

**2\. MaxNumberOfSlots:**

- Defines the maximum number of slots in the inventory.
- You can add or remove slots from the InventorySlots list through the Inspector.
- If fixed slots is enabled, remaining empty slots will be created at the start.

**3\. Save Path:**

- Specify the file path where the inventory data will be saved (e.g., /PlayerInventory.save).
- Use the **SaveInventory()** and **LoadInventory()** functions in the **InventorySO** script to save and load inventory data.

**4\. Inventory Slots:**

- This is an array that contains all the slots in the inventory.
- Each slot can hold an item, and you can add or remove slots through the Inspector.
- **Important Notes**:
  - If an item exceeds its permitted quantity (e.g., stackable items), it will be reduced to the maximum allowed amount.
  - Slots with invalid item IDs will be removed automatically.
  - If the number of items exceeds the inventory's capacity, extra items will be removed.

**5\. Database:**

- Assign the **Item Database** here. The system will automatically find the database if it’s placed in the Resources/InventorySystem folder.
- **Important Note**: Assigning the database in the inspector allows the system to validate item IDs and display item icons correctly in the editor.
