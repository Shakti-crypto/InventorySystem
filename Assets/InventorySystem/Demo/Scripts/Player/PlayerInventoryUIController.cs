using UnityEngine;

namespace InventorySystem.Demo
{
    public class PlayerInventoryUIController : DynamicInventoryUI
    {
        [SerializeField] private BaseInventoryUI equipmentUIController;
        private bool inventoryOpenedForFirstTime;

        #region Unity functions
        private void OnEnable()
        {
            base.OnEnable();
            inputManager.onInventoryPressed += TryOpenInventory;
            inputManager.onInventoryClosed += TryCloseInventory;
        }
        private void Start()
        {
            base.Start();
            inventoryOpenedForFirstTime = false;
            if (InteractionInstructionsManager.Instance != null)
            {
                InteractionInstructionsManager.Instance.SetInstructions("Press TAB to open inventory");
            }
        }
        private void Update()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        private void OnDisable()
        {
            base.OnDisable();
            inputManager.onInventoryPressed -= TryOpenInventory;
            inputManager.onInventoryClosed -= TryCloseInventory;
        }
        #endregion

        private void TryOpenInventory()
        {
            if (isInventoryOpen) return;

            SetInventoryUIActiveStatus(true);
            SetTimeScaleAndCursor(true);
            if (equipmentUIController != null) equipmentUIController.OpenInventory();
            isInventoryOpen = true;

            if (!inventoryOpenedForFirstTime)
            {
                inventoryOpenedForFirstTime = true;
                if (InteractionInstructionsManager.Instance != null)
                {
                    InteractionInstructionsManager.Instance.DisableInstructions();
                }
            }
        }
        private void TryCloseInventory()
        {
            if (!isInventoryOpen) return;

            if (stopUiFromClosing) return;

            SetInventoryUIActiveStatus(false);
            SetTimeScaleAndCursor(false);
            if (equipmentUIController != null) equipmentUIController.CloseInventory();
            isInventoryOpen = false;
        }
        private void SetTimeScaleAndCursor(bool active)
        {
            if (active)
            {
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        protected override void OnPointerUp(InventorySlotUI inventoryUI)
        {
            if (!TradingManager.tradingMode) return;

            if (TradingUIManager.Instance == null)
            {
                Debug.LogWarning("Trading UI Manager reference not found. Unable to initiate trade!");
                return;
            }
            DraggedItem.targetInventorySlot = null;
            DraggedItem.currentInventorySlot = inventorySlotUI[inventoryUI];

            TradingUIManager.Instance.InitiateTrading(false);
        }
    }

}