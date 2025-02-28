**1.** **Purse Inventory:**

![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/PurseInventory.png)

A **Purse Inventory** is a lightweight and easy-to-setup inventory designed to hold a single item, typically used for currency (e.g., coins, gems). It’s perfect for managing currency or other single-item inventories without the need for complex setup.

**Key Features:**

- Holds only **one item** at a time.
- **No Scriptable Object Required**: The **Inventory Scriptable Object** is automatically created at runtime.
- **Minimal Setup**: Just add the Purse Inventory script to a GameObject—no additional scripts or manual setup needed.

**Setup Instructions:**

1. Add the Purse Inventory script to a GameObject (e.g., the player or a merchant).
2. Specify the **Item ID** of the item (e.g., coins) and the initial amount.
    - **Important Note**: Ensure the **Item ID** is valid and exists in the **Item Database**.
3. That’s it! The Purse Inventory will automatically create the necessary Scriptable Object at runtime.

**Limitations:**

- **No Save/Load Functionality**: Currently, the Purse Inventory does not support saving and loading. This feature will be added in a future update.

**Trading System Requirement:**

- For trading to work, both the player and merchants need a **Purse Inventory** containing the same currency item (e.g., coins).
