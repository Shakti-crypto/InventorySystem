
using static InventorySystem.DraggedItem;
using UnityEngine;
using System;

namespace InventorySystem
{
    public static class TradingManager
    {
        public static InventorySO playerInventory;
        public static PurseInventory playerPurse;
        public static InventorySO merchantInventory;
        public static PurseInventory merchantPurse;
        private static InventorySO buyerInventory;
        private static PurseInventory buyerPurse;
        private static PurseInventory sellerPurse;
        private static InventoryItemSO item { get { return DraggedItem.currentInventorySlot.item; } }
        public static bool tradingMode;

        public static void TryBuyItem(InventoryItemSO money,int singleItemPrice, int itemAmountToTrade,Action onTradeComplete)
        {
            if (playerPurse == null)
            {
                DraggedItem.currentInventorySlot = null;
                Debug.LogWarning("Unable to find player's Purse. Please make sure that the player has a purse inventory. Cannot trade items!");
                onTradeComplete?.Invoke();
                return;
            }
            InventoryItemSO itemInPlayerPurse = playerPurse.GetItem();
            if (itemInPlayerPurse == null || itemInPlayerPurse != money)
            {
                DraggedItem.currentInventorySlot = null;
                Debug.LogWarning("Purse does not contain correct type of money. Cannot trade items!");
                onTradeComplete?.Invoke();
                return;
            }

            int moneyWithThePlayer = playerPurse.GetItemNumber();
            if (singleItemPrice*itemAmountToTrade > moneyWithThePlayer)
            {
                DraggedItem.currentInventorySlot = null;
                Debug.LogWarning("Player does not have enough money.");
                onTradeComplete?.Invoke();
                return;
            }

            buyerInventory = playerInventory;
            buyerPurse = playerPurse;
            sellerPurse = merchantPurse;

            StartTrading(itemAmountToTrade, onTradeComplete);
        }

        public static void TrySellItem(InventoryItemSO money, int singleItemPrice, int itemAmountToTrade, Action onTradeComplete)
        {
            if (merchantPurse == null)
            {
                Debug.LogWarning("Unable to find merchant's Purse. Please make sure that the merchant has a purse inventory. Cannot trade items!");
                DraggedItem.currentInventorySlot = null;
                onTradeComplete?.Invoke();
                return;
            }
            InventoryItemSO itemInmerchantPurse = merchantPurse.GetItem();
            if (itemInmerchantPurse == null || itemInmerchantPurse != money)
            {
                DraggedItem.currentInventorySlot = null;
                Debug.LogWarning("Merchant's Purse does not contain correct type of money. Cannot trade items!");
                onTradeComplete?.Invoke();
                return;
            }

            int moneyWithTheMerchant = merchantPurse.GetItemNumber();
            if (singleItemPrice*itemAmountToTrade > moneyWithTheMerchant)
            {
                DraggedItem.currentInventorySlot = null;
                Debug.LogWarning("Merchant doesn't have enough money.");
                onTradeComplete?.Invoke();
                return;
            }

            buyerInventory = merchantInventory;
            buyerPurse = merchantPurse;
            sellerPurse = playerPurse;
            StartTrading(itemAmountToTrade, onTradeComplete);
        }

        public static void StartTrading(int amountToTrade,Action onTradingComplete)
        {
            buyerInventory.AddItem(currentInventorySlot.itemID, amountToTrade, out int amountAdded);
            int moneyExchanged = item.cost * amountAdded;
            buyerPurse.RemoveItem(moneyExchanged);
            currentInventorySlot.inventorySO.RemoveItem(item, amountAdded, out int amountRemoved);
            
            sellerPurse.AddItem(moneyExchanged);

            currentInventorySlot.inventorySO.PopulateDictionary();
            buyerInventory.PopulateDictionary();

            onTradingComplete?.Invoke();
        }

    }

}