using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static InventorySystem.DraggedItem;

namespace InventorySystem
{
    public abstract class BaseInventoryUI : MonoBehaviour
    {
        #region Variables
        public Inventory Inventory { get { return inventory; } }
        protected InputManager inputManager;
        [SerializeField] protected GameObject canvas; //For dragged Item creation
        [SerializeField] protected GameObject inventoryUiBG; //The gameobject that contains all the inventory UI
        [SerializeField] protected Inventory inventory;
        [SerializeField] protected bool allowDropItems;
        [SerializeField] protected Transform itemDropPoint;
        [SerializeField] protected Vector3 itemDropPointOffset;
        [SerializeField] protected bool visibleAtStart;

        protected Dictionary<InventorySlotUI, InventorySlot> inventorySlotUI = new Dictionary<InventorySlotUI, InventorySlot>();
        protected Vector2 mousePosition;

        [HideInInspector] public bool stopUiFromClosing;
        protected bool isInventoryOpen;
        #endregion

        #region Unity functions

        private void Awake()
        {
            inputManager = InputManager.Instance;
        }
        protected void OnEnable()
        {
            inputManager.readMousePosition += ReadMousePosiiton;
        }
        protected void Start()
        {
            if (inventory == null) return;
            AddEventToInventoryBG();
            inventory.inventorySO.onSlotRemoved += OnSlotRemoved;
            inventory.inventorySO.onSlotAdded += OnSlotAdded;
            SetupSlots();
            SetInventoryUIActiveStatus(visibleAtStart);
        }
        protected void OnDisable()
        {
            inputManager.readMousePosition -= ReadMousePosiiton;
            inventory.inventorySO.onSlotRemoved -= OnSlotRemoved;
            inventory.inventorySO.onSlotAdded -= OnSlotAdded;
        }
        #endregion

        #region Overridable functions
        protected virtual void OnSlotRemoved(InventorySlot slot) { }
        protected virtual void OnSlotAdded(InventorySlot slot) { }
        public abstract void SetupSlots();
        #endregion

        private void ReadMousePosiiton(Vector2 mousePosition)
        {
            this.mousePosition = mousePosition;
        }

        #region Inventory Visibility functions
        protected void SetInventoryUIActiveStatus(bool active)
        {
            if (inventoryUiBG == null)
            {
                Debug.LogWarning("Inventory UI Background reference not set. Unable to open or close inventory!");
                return;
            }
            inventoryUiBG.SetActive(active);
        }
        public void OpenInventory()
        {
            SetInventoryUIActiveStatus(true);
            isInventoryOpen = true;
        }
        public void CloseInventory()
        {
            SetInventoryUIActiveStatus(false);
            isInventoryOpen = false;

            if (DraggedItem.dragItemGO != null)
            {
                dragItemGO.SetActive(false);
            }
        }
        #endregion

        protected void AddEventToInventoryBG()
        {
            if (inventoryUiBG == null)
            {
                Debug.LogWarning("Inventory UI Background reference not set. Make sure it is set for the inventory to function properly!");
                return;
            }

            if (!inventoryUiBG.TryGetComponent(out Button button))
            {
                button = inventoryUiBG.AddComponent<Button>();
                button.interactable=true;
                Color buttonColor = Color.white; // Change this to your desired color
                ColorBlock colorBlock = button.colors;
                colorBlock.normalColor = buttonColor;
                colorBlock.highlightedColor = buttonColor;
                colorBlock.pressedColor = buttonColor;
                colorBlock.selectedColor = buttonColor;
                colorBlock.disabledColor = buttonColor;
                button.colors = colorBlock;
            }

            if(!inventoryUiBG.TryGetComponent(out EventTrigger eventTrigger))
            {
                eventTrigger=inventoryUiBG.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;

            bool eventFound=false;
            foreach(EventTrigger.Entry entryEvents in eventTrigger.triggers)
            {
                if(entryEvents.eventID == EventTriggerType.PointerEnter)
                {
                    eventFound = true;
                }
            }

            if (!eventFound)
            {
                entry.callback.AddListener(delegate { OnPointerEnterInventoryBG(); });
                eventTrigger.triggers.Add(entry);
            }

            eventFound = false;
            EventTrigger.Entry exit = new EventTrigger.Entry();
            foreach (EventTrigger.Entry exitEvents in eventTrigger.triggers)
            {
                if (exitEvents.eventID == EventTriggerType.PointerExit)
                {
                    eventFound = true;
                }
            }

            if (!eventFound)
            {
                exit.eventID = EventTriggerType.PointerExit;
                exit.callback.AddListener(delegate { OnPointerExitInventoryBG(); });
                eventTrigger.triggers.Add(exit);
            }
        }

        #region Slot Info functions
        protected void SetSlotInformation(InventorySlot slot)
        {
            InventorySlotUI slotUI = slot.slotUI;
            InventoryItemSO item = slot.item;
            slotUI.SetGameObjectActive(true);
            if (item == null)
            {
                SetImage(false, slotUI);
                SetItemAmount(false, slotUI);
                SetItemCost(false, slotUI);
                return;
            }

            SetImage(true, slotUI, item.icon);

            bool displayItemNumber = item.isStackable || slot.amount==0;

            SetItemAmount(displayItemNumber, slotUI, slot.amount);
            SetItemCost(true, slotUI, item.cost);
            slotUI.icon.gameObject.SetActive(true);
        }

        //These 3 functions are used to set slot properties, because in some cases they might be null, so we cannot set them directly
        protected void SetItemCost(bool active, InventorySlotUI slotUI, int cost = 0)
        {
            TextMeshProUGUI costText = slotUI.cost;

            if (costText == null) return;

            if (active)
            {
                costText.text = cost.ToString();
            }
            costText.gameObject.SetActive(active);
        }
        protected void SetItemAmount(bool active, InventorySlotUI slotUI, float amount = 0)
        {
            TextMeshProUGUI amountText = slotUI.amount;

            if (amountText == null) return;

            if (active)
            {
                amountText.text = amount.ToString();
            }
            amountText.gameObject.SetActive(active);
        }
        protected void SetImage(bool active, InventorySlotUI slotUI, Sprite itemImage = null)
        {
            Image image = slotUI.icon;

            if (image == null) return;

            if (active && itemImage != null)
            {
                image.sprite = itemImage;
            }
            image.gameObject.SetActive(active);
        }
        protected void AddEventsToSlotButton(InventorySlotUI inventoryUI)
        {
            inventoryUI.AddEventToButton(EventTriggerType.PointerUp, delegate { OnPointerUp(inventoryUI); });
            inventoryUI.AddEventToButton(EventTriggerType.BeginDrag, delegate { OnBeginDragSlot(inventoryUI); });
            inventoryUI.AddEventToButton(EventTriggerType.Drag, delegate { OnDragSlot(); });
            inventoryUI.AddEventToButton(EventTriggerType.EndDrag, delegate { OnDragEndSlot(); });
            inventoryUI.AddEventToButton(EventTriggerType.PointerEnter, delegate { OnPointerEnter(inventoryUI); });
            inventoryUI.AddEventToButton(EventTriggerType.PointerExit, delegate { OnPointerExit(); });
        }
        #endregion

        #region Button Events

        //We have this virtual OnPointerUp function so that we can use it in other inventory(in this project merchant's inventory)
        protected virtual void OnPointerUp(InventorySlotUI inventoryUI) { }
        protected void OnBeginDragSlot(InventorySlotUI slotUI)
        {
            if (TradingManager.tradingMode) return;

            if (inventorySlotUI[slotUI].item == null) return;

            if (DraggedItem.dragItemGO == null)
            {
                CreateItemDragIcon();
            }

            SetDraggedItemCurrentItem(slotUI);
        }
        protected void SetDraggedItemCurrentItem(InventorySlotUI slotUI)
        {
            GameObject dragItemGO = DraggedItem.dragItemGO;
            Image image = dragItemGO.GetComponent<Image>();
            image.sprite = slotUI.icon.sprite;
            dragItemGO.SetActive(true);
            currentInventorySlot = inventorySlotUI[slotUI];
        }
        protected void OnPointerEnter(InventorySlotUI slotUI)
        {
            if (dragItemGO == null || !dragItemGO.activeInHierarchy) return;

            targetInventorySlot = inventorySlotUI[slotUI];
        }        
        protected void CreateItemDragIcon()
        {
            if (canvas == null)
            {
                Debug.LogWarning("Canvas reference not set. Unable to create dragged item gameobject.");
                return;
            }
            GameObject dragItemGO = new GameObject("DragItem");
            Image image = dragItemGO.AddComponent<Image>();
            image.raycastTarget = false;
            dragItemGO.transform.SetParent(canvas.transform, false);
            dragItemGO.SetActive(false);
            DraggedItem.dragItemGO = dragItemGO;
        }
        private void OnPointerEnterInventoryBG()
        {
            DraggedItem.currentInventoryUI = this;
        }
        private void OnPointerExitInventoryBG()
        {
            DraggedItem.currentInventoryUI = null;
        }        
        private void OnDragSlot()
        {
            if (dragItemGO == null || !dragItemGO.activeInHierarchy) return;

            dragItemGO.transform.position = mousePosition;
        }
        private void OnDragEndSlot()
        {
            if (dragItemGO == null || !dragItemGO.activeInHierarchy) return;

            if (targetInventorySlot != null || !dragItemGO.activeInHierarchy)
            {
                DraggedItem.TryItemSwap();
                dragItemGO.gameObject.SetActive(false);
                return;
            }
            else if (allowDropItems && !TradingManager.tradingMode && DraggedItem.currentInventoryUI==null)
            {
                DropCurrentItem();
            }

            ResetDraggedItemProperties();
        }
        private void DropCurrentItem()
        {   
            int itemAmount=currentInventorySlot.amount;
            InventoryItemSO item = currentInventorySlot.item;

            currentInventorySlot.inventorySO.RemoveItemFromASlot(currentInventorySlot, item, currentInventorySlot.amount, out _);

            if (item.pickupPrefab == null || itemAmount==0)
            {
                Debug.LogWarning("No pickup prefab provided.");
                return;
            }

            GameObject pickupPrefab = Instantiate(item.pickupPrefab);
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle;
            Vector3 dropOffset = new Vector3(randomPoint.x, 0, randomPoint.y) + itemDropPointOffset;
            pickupPrefab.transform.position = itemDropPoint.position + dropOffset;
        }
        private void OnPointerExit()
        {
            if (dragItemGO == null || !dragItemGO.activeInHierarchy) return;
            targetInventorySlot = null;
        }
        #endregion
    }
}



