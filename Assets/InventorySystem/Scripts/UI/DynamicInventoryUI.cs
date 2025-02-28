using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class DynamicInventoryUI : BaseInventoryUI
    {
        [SerializeField] private Transform slotParent;
        [SerializeField] private GameObject slotUIPrefab;
        private List<InventorySlotUI> slotsUI = new List<InventorySlotUI>();

        public void Initialize(GameObject _canvas, GameObject _inventoryUIGO, Inventory _inventory, bool _allowDropItems, Transform _itemDropPoint,Vector3 _itemDropPointOffset,bool _visibleAtStart,Transform _slotParent, GameObject _slotUIPrefab)
        {
            canvas = _canvas;
            inventoryUiBG = _inventoryUIGO;
            inventory = _inventory;
            allowDropItems = _allowDropItems;
            itemDropPoint = _itemDropPoint;
            itemDropPointOffset=_itemDropPointOffset;
            visibleAtStart = _visibleAtStart;
            slotParent = _slotParent;
            slotUIPrefab =_slotUIPrefab;
        }

        public override void SetupSlots()
        {   
            //This is required for when we load the inventory
            if (slotsUI.Count > 0)
            {
                DestroyExistingSlots();
            }

            slotsUI = new List<InventorySlotUI>();
            inventorySlotUI=new Dictionary<InventorySlotUI, InventorySlot> ();
            int slotUIIndex = 0;

            foreach (InventorySlot slot in inventory.inventorySlots)
            {
                InventorySlotUI slotUI = CreateASingleSlotUI();
                if (slotUI != null)
                {
                    slot.slotUI = slotUI;
                    SetSlotInformation(slot);
                    slot.onSlotUpdated += SetSlotInformation;
                    inventorySlotUI.Add(slotUI, slot);
                    slotUI.SetGameObjectActive(true);
                    slotUIIndex++;
                }
            }
        }
        public void RefreshSlotInformation()
        {
            //This is needed when multiple inventories share the same slot parent
            for(int i=0;i< slotParent.childCount; i++)
            {
                slotParent.GetChild(i).gameObject.SetActive(false);
            }

            foreach(InventorySlot slot in inventorySlotUI.Values)
            {
                SetSlotInformation(slot);
            }
        }
        private void DestroyExistingSlots()
        {
            foreach (InventorySlotUI slot in slotsUI)
            {
                slot.DestroyGameobject();
            }
        }
        protected InventorySlotUI CreateASingleSlotUI()
        {
            InventorySlotUI inventoryUI;
            GameObject slotUI = Instantiate(slotUIPrefab, Vector3.zero, Quaternion.identity);

            if (!slotUI.TryGetComponent(out InventorySlotUIReferenceContainer referenceContainer))
            {
                Debug.LogWarning("Slot Reference Container not found. Cannot create slots' UI.");
                return null;
            }

            Image image = referenceContainer.itemImage;
            TextMeshProUGUI amount = referenceContainer.amount; ;
            if (referenceContainer.cost != null)
            {
                TextMeshProUGUI cost = referenceContainer.cost;
                inventoryUI = new InventorySlotUI(slotUI, image, amount, cost);
            }
            else
            {
                inventoryUI = new InventorySlotUI(slotUI, image, amount);
            }

            AddEventsToSlotButton(inventoryUI);
            slotsUI.Add(inventoryUI);
            inventoryUI.SetParent(slotParent);

            return inventoryUI;
        }
        protected override void OnSlotRemoved(InventorySlot slot)
        {
            InventorySlotUI slotUI = slot.slotUI;
            inventorySlotUI.Remove(slotUI);
            slotUI.DestroyGameobject();
        }
        protected override void OnSlotAdded(InventorySlot slot)
        {
            SetupSlots();
        }
    }

}

