**2.Merchant Inventory:**

![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/MerchantInvetory.png)

A **Merchant Inventory** allows players to buy and sell items. It’s designed for NPCs like shopkeepers.

It is a ‘Dynamic UI setup’ with ‘FixedSlots’ property disabled in the Inventory. Open the **‘Merchant Inventory Setup’ tool** to create this inventory. Alternatively you can:

1. **Create the Merchant Inventory**:
   
    - Right-click in the Project window, go to Create > Inventory > Inventory, and create a new Inventory Scriptable Object.
    - Add the Inventory script to the merchant GameObject and assign the Scriptable Object.
    - Disable **Fixed Slots** in the inventory settings to allow dynamic slot management.
    - Now, add in a **Purse Inventory,** and fill it with the money item.
      
3. **Set Up the Merchant UI**:
   
    - Add the Merchant Inventory UI Controller script to the merchant GameObject.
    - Configure the UI settings (e.g., Canvas, Slot Parent, Slot UI Prefab) as described in the **Dynamic Inventory UI** section.
    - Ensure the **Slot UI Prefab** includes a **Cost** section to display item prices.
      
5. **Merchant-Specific Settings**:
   
    - **Item Types Accepted**: Specify the types of items the merchant can trade (e.g., weapons, consumables). If left empty, the merchant won’t trade any items.
    - **Merchant Purse**: Assign the merchant’s **Purse Inventory** to handle currency during trades.
    - **Coin Amount Text**: Optionally, assign a UI Text element to display the merchant’s current currency amount.

    ![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/MerchantInvetoryCointText.png)

1. **Player Interaction**:
    - Add a **Collider** near the merchant and set it as a trigger.

      ![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/PlayerRangeCheckCollider.png)

    - Attach the PlayerRangeCheck script to the collider and assign the player’s tag (e.g., "Player") in the ‘InteractionTag’ variable.

      ![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/PlayerInRangeScriptView.png)

    - Attach the PlayerRangeCheck script to the collider and assign the player’s tag (e.g., "Player") in the ‘InteractionTag’ variable.
