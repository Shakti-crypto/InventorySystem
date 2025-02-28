using System;
using UnityEngine;

namespace InventorySystem.Demo
{
    public class QuickUseInventoryController : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        private InputManager inputManager;
        public static event Func<InventoryItemSO, bool> QuickUseItemUsed;

        private void Awake()
        {
            inputManager = InputManager.Instance;
        }
        private void Start()
        {
            inputManager.onQuickUseInventoryPressed += QuickUsePerformed;
        }
        private void QuickUsePerformed(int slotIndex)
        {
            InventorySlot slot = inventory.inventorySO.inventorySlots[slotIndex];
            InventoryItemSO item = slot.item;
            bool? itemUsed = QuickUseItemUsed?.Invoke(item);
            if(itemUsed.HasValue && itemUsed.Value)
            {
                inventory.inventorySO.RemoveItemFromASlot(slot, item, 1, out _);
            }
        }
        private void OnDisable()
        {
            inputManager.onQuickUseInventoryPressed -= QuickUsePerformed;
        }
    }
}
