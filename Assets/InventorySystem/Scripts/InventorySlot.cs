using System;
using UnityEngine;

public delegate void OnSlotUpdated(InventorySystem.InventorySlot slot);

namespace InventorySystem
{
    [System.Serializable]
    public class InventorySlot : ICloneable
    {
        [HideInInspector] public InventorySO inventorySO; //The inventory scriptable object that this slot is a part of
        [HideInInspector] public InventoryItemSO item;
        [HideInInspector] public InventorySlotUI slotUI;
        public string itemID;
        public int amount;
        public InventoryItemTypeSO itemType;
        private bool runtimeCreatedSlot;
        public bool testing = true;
        public bool RuntimeCreatedSlot { get { return runtimeCreatedSlot; } }

        public event OnSlotUpdated onSlotUpdated;

        #region Constructors
        public InventorySlot(bool _runtimeCreatedSlot, InventorySO inventory)
        {
            runtimeCreatedSlot = _runtimeCreatedSlot;
            this.inventorySO = inventory;
        }
        public InventorySlot(string _ItemID, InventoryItemSO _item, InventorySO inventory)
        {
            itemID = _ItemID;
            item = _item;
            this.inventorySO = inventory;
        }
        public InventorySlot(InventoryItemTypeSO _slotType, InventorySO inventory)
        {
            itemType = _slotType;
            this.inventorySO = inventory;
        }
        #endregion

        public void InvokeSlotUpdatedEvent()
        {
            onSlotUpdated?.Invoke(this);
        }
        public void AddAmount(int _amount)
        {
            amount += _amount;
            onSlotUpdated?.Invoke(this);
        }
        public void RemoveItem()
        {
            item = null;
            itemID = null;
            amount = 0;
            onSlotUpdated?.Invoke(this);
        }
        public void RemoveAmount(int _amount)
        {
            amount -= _amount;
            amount = Mathf.Max(amount, 0);
            onSlotUpdated?.Invoke(this);
        }
        public object Clone()
        {
            return MemberwiseClone(); //We only need to return a softCopy
        }
    }
}



