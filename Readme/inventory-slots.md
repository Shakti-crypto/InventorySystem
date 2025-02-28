**Inventory Slots:**

Inventory Slots are containers which actually hold the inventory items.

![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/InventorySlotExample.png)

**1. Adding Items to Slots**

- You can add items to a slot by specifying the **Item ID** in the Inspector.
- If the **Item ID** is valid and the item exists in the **Item Database**, the item's icon and details will appear in the slot.
- **Stackable Items**: If the item is stackable, you can specify the quantity of the item in the slot (up to the stack limit).
- **Non-Stackable Items**: Only one item can be placed in the slot.

**2. Slot Settings**

- **Item Type Restriction**: You can assign an **Item Type** to a slot to restrict it to specific categories (e.g., only weapons can go into weapon slots).
  - If no item type is assigned, the slot can hold any type of item.
- **Cost Display**: The slot displays the cost of the item, but this cannot be modified directly in the slot. To change the cost, edit the **Inventory Item** Scriptable Object.

**3. Slot Behavior**

- **Empty Slots**: If a slot is empty, it will either be removed (if Fixed Slots is disabled) or remain visible (if Fixed Slots is enabled).
- **Invalid Items**: If an item in a slot has an invalid **Item ID**, the slot will be cleared automatically.
- **Exceeding Limits**: If an item exceeds its permitted quantity (e.g., stackable items), the quantity will be reduced to the maximum allowed amount.
