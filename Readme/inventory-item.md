**Inventory Item:**

Inventory Items are **Scriptable Objects** that act as information containers for items in your game.

![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/InventoryItemExample.png)

They store details about the item but do not represent the actual GameObject in the scene. You will need to link the real-world items (e.g., 3D models or prefabs) to their corresponding Inventory Items manually. (You can look PlayerEquipmentManager and PlayerStatsManager in the Demo scene to get some idea)

**1\. Creating an Inventory Item:**

- Use the **Item Creator Tool** (Tools > Inventory > Item Creator Window) to create items easily. This tool will automatically add the item to the **Item Database**.
- Alternatively, you can create items manually:
    1. Right-click in the Project window, go to Create > Inventory > Inventory Item.
    2. Save the Scriptable Object in a suitable folder.
    3. Open the **Item Database** (located in Resources/InventorySystem), drag the new item into the database, and assign it a unique **Item ID**.

**Important Note:** Each item should have a unique ID. These IDs are used to fetch items in the Inventory.

**2\. Item Settings:**

- **Item Type**: Assign an item type to categorize the item (e.g., weapon, consumable, armor). Item types are also Scriptable Objects and can be created by right-clicking and selecting Create > Inventory > Item Type.
- **Stackable**: Enable this option to allow the item to stack in the inventory. Stackable items can have multiple quantities in a single slot.
  - **Stackable Amount**: The maximum number of items that can be stacked in one slot (only available if the item is stackable).
- **Cost**: The price of the item when trading with merchants or players.
- **Pickup Prefab**: The prefab that will be spawned when the item is dropped from the inventory. If left empty, the item will be removed from the inventory but no prefab will spawn.
  - **Important Note**: The pickup prefab should have an ItemPickup script attached to allow players to pick up the item again.
