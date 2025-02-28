**This Inventory System offers some special kind of Inventories as well:**

1. Purse Inventory
2. Merchant Inventory
3. Chest Inventory

These Inventories have been created by extending the Inventory scripts mentioned above. These are designed for specific use cases like currency management, trading, and item storage.

**Comparison of Special Inventories:**

| **Feature** | **Purse Inventory** | **Merchant Inventory** | **Chest Inventory** |
| --- | --- | --- | --- |
| **Purpose** | Holds currency or a single item | Allows buying/selling items. | Stores items persistently |
| **Save/Load** | Not supported(yet) | Manual(you will have to setup) | Automatic |
| **UI Type** | None (no UI by default) | Dynamic UI | Dynamic UI |
| **Fixed Slots** | N/A (holds one item) | Disabled | Enabled |
| **Interaction** | Used for trading | Player interacts to trade | Player interacts to open/close |
