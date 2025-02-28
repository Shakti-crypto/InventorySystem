using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        public InventorySO inventorySO;
        public BaseInventoryUI inventoryUI;
        public List<InventorySlot> inventorySlots
        {
            get
            {
                if (inventorySO == null) return null;

                return inventorySO.inventorySlots;
            }
        }
        private void Awake()
        {
            CloneTheInventory();
        }
        private void CloneTheInventory()
        {
            inventorySO = inventorySO.Clone() as InventorySO;
            if (inventorySO.fixedSlots) inventorySO.CreateSlotsForFixedInventory();
            inventorySO.SetInventorySlotItemsAccordingToTheirIDS();
            inventorySO.PopulateDictionary();
        }
        public void SaveInentoryData()
        {
            inventorySO.SaveInventory();
        }
        public void LoadInventoryData()
        {
            inventorySO.LoadInventory();
            if (inventoryUI != null)
            {
                inventoryUI.SetupSlots();
            }
        }
    }
}
