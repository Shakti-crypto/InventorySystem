using UnityEngine;

namespace InventorySystem
{
    public static class DraggedItem
    {
        public static GameObject dragItemGO;
        public static InventorySlot currentInventorySlot;
        public static InventorySlot targetInventorySlot;
        public static BaseInventoryUI currentInventoryUI; //The inventory UI on which the cursor is right now

        public static void TryItemSwap()
        {
            if (TradingManager.tradingMode) return;

            if (!CheckIfSwapPossible())
            {
                ResetDraggedItemProperties();
                return;
            }

            if (targetInventorySlot.item == currentInventorySlot.item)
            {
                AddItemToTargetInventory();
            }
            else
            {
                SwapItemsInSlots();
            }

            UpdateItemsDictionary();
            ResetDraggedItemProperties();
        }
        private static bool CheckIfSwapPossible()
        {
            bool canSwapItems = true;
            if (targetInventorySlot.item)
            {
                if (targetInventorySlot.itemType || currentInventorySlot.itemType)
                {
                    canSwapItems = currentInventorySlot.item.itemType == targetInventorySlot.itemType || currentInventorySlot.item.itemType == targetInventorySlot.item.itemType;
                }
            }
            else
            {
                if (targetInventorySlot.itemType)
                {
                    canSwapItems = currentInventorySlot.item.itemType == targetInventorySlot.itemType;
                }
            }

            return canSwapItems;
        }
        public static void SwapItemsInSlots()
        {
            InventorySlot tempSlot = new InventorySlot(true, null);
            CopySlotInformation(targetInventorySlot, tempSlot);
            CopySlotInformation(currentInventorySlot, targetInventorySlot);
            CopySlotInformation(tempSlot, currentInventorySlot);
        }
        public static void AddItemToTargetInventory()
        {
            targetInventorySlot.inventorySO.AddItemToASlot(targetInventorySlot, currentInventorySlot.item, currentInventorySlot.amount, out int amountAdded);
            currentInventorySlot.inventorySO.RemoveItemFromASlot(currentInventorySlot, currentInventorySlot.item, amountAdded, out _);
        }
        public static void UpdateItemsDictionary()
        {
            currentInventorySlot.InvokeSlotUpdatedEvent();
            targetInventorySlot.InvokeSlotUpdatedEvent();

            currentInventorySlot.inventorySO.PopulateDictionary();
            if (currentInventorySlot.inventorySO != targetInventorySlot.inventorySO)
            {
                targetInventorySlot.inventorySO.PopulateDictionary();
            }
        }
        public static void ResetDraggedItemProperties()
        {   
            dragItemGO.SetActive(false);
            currentInventorySlot = null;
            targetInventorySlot = null;
        }
        private static void CopySlotInformation(InventorySlot baseSlot, InventorySlot targetSlot)
        {
            targetSlot.itemID = baseSlot.itemID;
            targetSlot.item = baseSlot.item;
            targetSlot.amount = baseSlot.amount;
        }
    }

}

