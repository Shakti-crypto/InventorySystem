**2\. Static Inventory UI:**

In this setup, slot UIs are manually placed in the hierarchy. This is ideal for custom layouts where you need full control over the position and appearance of each slot.

![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/PlayerEquipmentInventoryGameView.png)

**Important Note:** Number of slotUIs referenced in the script should be equal to the slot count in the inventory.

**Example of Static Slot UI**

Add ‘Static Inventory UI’ script to a gameobject. This gameobject need not to be the same one containing the Inventory script.

(The first five settings in Static UI are same as to that of Dynamic UI. Move to the Slot Array to see Static UI specific property)

1. **Inventory UIGO (Inventory UI Gameobject)**:
   
    - This is the GameObject that contains the entire inventory UI (e.g., background, panels, etc.).
    - Ensure all UI elements are children of this GameObject.
    - This GameObject will be enabled/disabled to show or hide the inventory.
      
3. **Inventory**:
   
    - Reference to the **Inventory script** that this UI will display.
    - This links the UI to the actual inventory data.
      
5. **Canvas**:
   
    - Reference to the **Canvas** where the inventory UI is displayed.
    - This is required for displaying item images when they are dragged around the UI.
      
7. **Allow Drop Items**:
   
    - If enabled, players can drag items out of the inventory to drop them.
    - **Important Note**: Items will only be dropped if a **Pickup Prefab** is assigned to the item.
    - **Sub-Settings**:
        - **Item Drop Point**: The Transform where dropped items will appear (e.g., the player's position or a chest's position).
        - **Item Drop Point Offset**: An additional offset applied to the drop point (e.g., to drop items in front of the player).
          
9. **Visible at Start**:
    
    - If enabled, the inventory UI will be visible when the game starts.
    - If disabled, you’ll need to call the OpenInventory() function from a script to show the UI.
      
11. **Slots Array**:
    
    - Manually assign each slot UI GameObject to this array.
    - Ensure the number of slot UIs matches the number of slots in the inventory.
      
      ![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/SlotUIPrefab.png)
      
    - Each slot UI should have the InventorySlotUIReferenceContainer script attached, with references to the UI elements (e.g., item image, amount text, cost text).

- You don’t need to set all of the references in this script. For example, if you don’t want to display item’s cost, then don’t set its reference.
