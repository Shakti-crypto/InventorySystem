**Trading:**

The trading system allows players to buy and sell items with merchants. It relies on two main scripts: **TradingManager** for the logic and **TradingUIManager** for the UI.

**1. TradingManager**

- Handles the core trading logic, including buying and selling items.
- Key Functions:
  - **TryBuyItem()**: Attempts to buy an item from the merchant.
  - **TrySellItem()**: Attempts to sell an item to the merchant.
- Both functions require the **Money Item** (e.g., coins) to be passed as a parameter.

**2. TradingUIManager**

- Manages the trading UI, including displaying items, prices, and trade buttons.
- Calls the appropriate functions in the **TradingManager** when the player initiates a trade.
- Requires the **Money Item** to be assigned in the inspector.
- **Important Note**: The **TradingUIManager** is included only in the **Demo Scene** and not in the main package, as its implementation may vary depending on the project. You can use the demo version as a reference or create your own custom UI manager.

**3. Requirements for Trading**

- Both the player and merchant must have a **Purse Inventory** containing the same **Money Item**.
- The **Money Item** must be defined in the **Item Database** and assigned in the **TradingUIManager**.
