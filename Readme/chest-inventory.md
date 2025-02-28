**2\. Chest Inventory:**

A **Chest Inventory** is used for storing items. It automatically saves and loads inventory data, making it ideal for persistent storage (e.g., treasure chests, storage boxes).

It is a ‘Dynamic UI setup’ with ‘FixedSlots’ property enabled in the Inventory. Open the **‘Chest Inventory Setup’ tool** to create this inventory. Alternatively you can:

1. **Create the Chest Inventory**:
    - Right-click in the Project window, go to Create > Inventory > Inventory, and create a new Inventory Scriptable Object.
    - Add the Inventory script to the chest GameObject and assign the Scriptable Object.
    - Enable **Fixed Slots** in the inventory settings to ensure the chest has a fixed number of slots.
2. **Set Up the Chest UI**:
    - Add the Chest Inventory UI Controller script to the chest GameObject.
    - Configure the UI settings (e.g., Canvas, Slot Parent, Slot UI Prefab) as described in the **Dynamic Inventory UI** section.
3. **Player Interaction**:
    - Add a **Collider** near the chest and set it as a trigger.
      
      ![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/ChestPlayerRangeCheckCollider.png)
   
    - Attach the PlayerRangeCheck script to the collider and assign the player’s tag (e.g., "Player") in the ‘InteractionTag’ variable.
  
      ![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/PlayerInRangeScriptView.png)

    - Link the PlayerRangeCheck reference in the Chest Inventory UI Controller script to enable interaction when the player is in range.

1. **Save Path**:
    - Specify the save path in the **Inventory Settings** (e.g., /ChestInventory.save).
    - The chest will automatically save and load its contents from this path.
