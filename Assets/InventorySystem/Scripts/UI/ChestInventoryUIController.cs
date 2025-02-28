using System.Collections;
using UnityEngine;

namespace InventorySystem
{
    public class ChestInventoryUIController : DynamicInventoryUI
    {
        [SerializeField] private PlayerRangeCheck rangeCheck;
        private Inventory playerInventory;

        public delegate void OnChestOpen();
        public event OnChestOpen onChestOpen;
        private BaseInventoryUI playerInventoryUI
        {
            get
            {
                return playerInventory.inventoryUI;
            }
        }

        public void Initialize(GameObject _canvas, GameObject _inventoryUIGO, Inventory _inventory, bool _allowDropItems, Transform _itemDropPoint, Vector3 _itemDropPointOffset, bool _visibleAtStart, Transform _slotParent, GameObject _slotUIPrefab,  PlayerRangeCheck _rangeCheck)
        {
            base.Initialize(_canvas, _inventoryUIGO, _inventory, _allowDropItems, _itemDropPoint, _itemDropPointOffset, _visibleAtStart, _slotParent, _slotUIPrefab);
            rangeCheck = _rangeCheck;
        }

        #region Unity functions
        private void OnEnable()
        {
            base.OnEnable();
            inputManager.onInteractionPressed += InteractionPressed;
            rangeCheck.playerEnteredRange += PlayerEnterRange;
            rangeCheck.playerLeftRange += PlayerLeft;
        }
        private void Start()
        {
            base.Start();
            CloseInventory();
            rangeCheck.playerLeftRange += PlayerLeftRange;
            inventory.LoadInventoryData();
        }
        private void OnDisable()
        {
            base.OnDisable();

            inventory.SaveInentoryData();
            inputManager.onInteractionPressed -= InteractionPressed;
            rangeCheck.playerEnteredRange -= PlayerEnterRange;
            rangeCheck.playerLeftRange -= PlayerLeft;
        }
        #endregion
        private void InteractionPressed()
        {
            if (!rangeCheck.playerInRange) return;

            if (!isInventoryOpen)
            {
                OpenChest();
            }
            else
            {
                CloseChest();
            }
        }
        private void PlayerEnterRange(GameObject player)
        {
            player.TryGetComponent(out playerInventory);
        }
        private void PlayerLeftRange(GameObject player)
        {
            if (isInventoryOpen)
            {
                CloseChest();
            }
        }
        //Have to do this to fix horizontal layout group bug where it does not calculate position correctly without doing this
        IEnumerator WaitToEnableInventory()
        {
            OpenInventory();
            yield return new WaitForEndOfFrame();
            CloseInventory();
            yield return new WaitForEndOfFrame();
            OpenInventory();
        }
        public void OpenChest()
        {
            if (playerInventoryUI != null)
            {
                playerInventoryUI.OpenInventory();
                playerInventoryUI.stopUiFromClosing = true;
            }
            RefreshSlotInformation();
            onChestOpen?.Invoke();
            StartCoroutine(WaitToEnableInventory());
        }
        public void CloseChest()
        {
            if (playerInventoryUI != null)
            {
                playerInventoryUI.CloseInventory();
                playerInventoryUI.stopUiFromClosing = false;
            }

            CloseInventory();
        }
        private void PlayerLeft(GameObject player)
        {
            if (isInventoryOpen) CloseChest();
        }

    }

}

