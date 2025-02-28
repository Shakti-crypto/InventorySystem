
**1\. Dynamic Inventory UI:**

In this setup, slot UIs are generated at runtime. This is ideal for layouts where the number of slots can change dynamically, or you can control their position using Canvas Layout components.

![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/DynamicInventoryExample.png)

**Slot UIs are created at runtime in Dynamic Inventory UI**

Add ‘Dynamic Inventory UI’ script to a gameobject. This gameobject need not to be the same one containing the Inventory script.

1. **Inventory UIGO (Inventory UI Gameobject)**:
    - This is the GameObject that contains the entire inventory UI (e.g., background, panels, etc.).
    - Ensure all UI elements are children of this GameObject.
    - This GameObject will be enabled/disabled to show or hide the inventory.
2. **Inventory**:
    - Reference to the **Inventory script** that this UI will display.
    - This links the UI to the actual inventory data.
3. **Canvas**:
    - Reference to the **Canvas** where the inventory UI is displayed.
    - This is required for displaying item images when they are dragged around the UI.
4. **Allow Drop Items**:
    - If enabled, players can drag items out of the inventory to drop them.
    - **Important Note**: Items will only be dropped if a **Pickup Prefab** is assigned to the item.
    - **Sub-Settings**:
        - **Item Drop Point**: The Transform where dropped items will appear (e.g., the player's position or a chest's position).
        - **Item Drop Point Offset**: An additional offset applied to the drop point (e.g., to drop items in front of the player).
5. **Visible at Start**:
    - If enabled, the inventory UI will be visible when the game starts.
    - If disabled, you’ll need to call the OpenInventory() function from a script to show the UI.
6. **Slot Parent**:
    - The GameObject in the Canvas hierarchy where slot UIs will be instantiated.
    - Ensure this GameObject has a **Layout Group** component (e.g., Grid Layout, Horizontal Layout) to automatically position the slots.
      ![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/SlotParentGridLayout.png)
    - Test placing slots manually first to verify they appear as expected when instantiated.

7. **Slot UI Prefab**:
   
    ![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/SlotUIPrefabView.png)

- The prefab used for each slot UI.
- Setup this prefab on how you want each slot to look. Namely there can be an item image, amount, and cost. You can add more things, but then you will have to modify ‘BaseInventoryUI’ script to set them accordingly for each slot UI.
- Attach the InventorySlotUIReferenceContainer script to the prefab and assign references to the UI elements you want to display.
  
![image](https://github.com/Shakti-crypto/InventorySystem/blob/main/Readme/Images/SlotUIPrefab.png)

- Only assign the references for UI elements you want to display. For example, if you don’t want to display the cost of item, don’t set its reference.

