using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class StaticInventoryUI : BaseInventoryUI
    {
        [SerializeField] private GameObject[] slots;

        public void Initialize(GameObject _canvas, GameObject _inventoryUIGO, Inventory _inventory, bool _allowDropItems, Transform _itemDropPoint, Vector3 _itemDropPointOffset, bool _visibleAtStart, GameObject[] _slots)
        {
            canvas = _canvas;
            inventoryUiBG = _inventoryUIGO;
            inventory = _inventory;
            allowDropItems = _allowDropItems;
            itemDropPoint = _itemDropPoint;
            itemDropPointOffset = _itemDropPointOffset;
            visibleAtStart = _visibleAtStart;
            slots= _slots;
        }

        public override void SetupSlots()
        {
            if (slots.Length != inventory.inventorySlots.Count)
            {
                Debug.LogWarning("Wrong number of slots defined. Please check the UI Slots again.");
                return;
            }

            InventorySlotUI inventoryUI;
            int uiSlotIndex = 0;
            for (int i = 0; i < inventory.inventorySlots.Count; i++)
            {
                InventorySlot slot = inventory.inventorySlots[i];

                GameObject slotGO = slots[uiSlotIndex];
                if (!slotGO.TryGetComponent(out InventorySlotUIReferenceContainer referenceContainer))
                {
                    Debug.LogWarning("Slot Reference Container not found. Cannot set slots' UI information.");
                    return;
                }
                Image image = referenceContainer.itemImage;
                TextMeshProUGUI amount = referenceContainer.amount;

                if (referenceContainer.cost != null)
                {
                    TextMeshProUGUI cost = referenceContainer.cost;
                    inventoryUI = new InventorySlotUI(slotGO, image, amount, cost);
                }
                else
                {
                    inventoryUI = new InventorySlotUI(slotGO, image, amount);
                }

                AddEventsToSlotButton(inventoryUI);
                slot.slotUI = inventoryUI;
                SetSlotInformation(slot);
                slot.onSlotUpdated += SetSlotInformation;
                inventorySlotUI.Add(inventoryUI, slot);
                uiSlotIndex++;
            }
        }
    }

}