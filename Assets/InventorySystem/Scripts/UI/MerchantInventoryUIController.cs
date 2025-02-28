
using System.Collections;
using TMPro;
using UnityEngine;

namespace InventorySystem
{
    public class MerchantInventoryUIController : DynamicInventoryUI
    {

        #region Variables
        [SerializeField] private PurseInventory merchantPurse;
        [SerializeField] private TextMeshProUGUI coinAmountText;
        [SerializeField] private PlayerRangeCheck rangeCheck;
        public InventoryItemTypeSO[] itemTypesAccepted;
        private Inventory playerInventory;
        private PurseInventory playerPurse;
        private BaseInventoryUI playerInventoryUI
        {
            get
            {
                return playerInventory.inventoryUI;
            }
        }

        public delegate void OnTradeIntitiated();
        public event OnTradeIntitiated onTradeIntitiated;
        #endregion

        public void Initialize(GameObject _canvas, GameObject _inventoryUIGO, Inventory _inventory, bool _allowDropItems, Transform _itemDropPoint, Vector3 _itemDropPointOffset, bool _visibleAtStart, Transform _slotParent, GameObject _slotUIPrefab,PurseInventory _merchantPurse, TextMeshProUGUI _coinAmountText, PlayerRangeCheck _rangeCheck, InventoryItemTypeSO[] _itemTypesAccepted)
        {
            base.Initialize(_canvas,_inventoryUIGO,_inventory,_allowDropItems,_itemDropPoint,_itemDropPointOffset,_visibleAtStart,_slotParent,_slotUIPrefab);
            merchantPurse= _merchantPurse;
            coinAmountText= _coinAmountText;
            rangeCheck= _rangeCheck;
            itemTypesAccepted= _itemTypesAccepted;
        }

        #region Unity functions
        private void OnEnable()
        {
            base.OnEnable();
            inputManager.onInteractionPressed += InteractionPressed;
            rangeCheck.playerEnteredRange += PlayerEnteredRange;
            rangeCheck.playerLeftRange += PlayerLeftRange;
        }
        private void Start()
        {
            base.Start();
            if (merchantPurse.slot == null || merchantPurse.slot.item == null)
            {
                Debug.LogWarning($"No money found in merchant {gameObject.name} purse.");
            }
        }
        private void OnDisable()
        {
            base.OnDisable();
            inputManager.onInteractionPressed -= InteractionPressed;
            rangeCheck.playerEnteredRange -= PlayerEnteredRange;
            rangeCheck.playerLeftRange -= PlayerLeftRange;
        }
        #endregion

        private void MerchantCoinUpdated(InventorySlot slot)
        {
            if (coinAmountText == null) return;

            coinAmountText.text = slot.amount.ToString();
        }
        private void InteractionPressed()
        {
            if (!rangeCheck.playerInRange) return;

            if (playerInventory == null) return;

            if (!isInventoryOpen)
            {
                OpenMerchantInventory();
            }
            else
            {
                CloseMerchantInventory();
            }
        }
        private void OpenMerchantInventory()
        {
            if (playerInventoryUI != null)
            {
                playerInventoryUI.OpenInventory();
                playerInventoryUI.stopUiFromClosing = true;
            }
            MerchantCoinUpdated(merchantPurse.slot);
            merchantPurse.slot.onSlotUpdated += MerchantCoinUpdated;

            SetTradingManagerReferences();
            RefreshSlotInformation();
            onTradeIntitiated?.Invoke();
            StartCoroutine(WaitToEnableInventory());
        }
        private void SetTradingManagerReferences()
        {    
            TradingManager.merchantInventory = inventory.inventorySO;
            TradingManager.merchantPurse = merchantPurse;
            TradingManager.playerInventory = playerInventory.inventorySO;
            TradingManager.playerPurse = playerPurse;
            TradingManager.tradingMode = true;

            if(TradingUIManager.Instance != null)
            {
                TradingUIManager.Instance.currentMerchantUIController = this;
            }
        }
        private void CloseMerchantInventory()
        {
            if (playerInventoryUI != null)
            {
                playerInventoryUI.CloseInventory();
                playerInventoryUI.stopUiFromClosing = false;
            }
            merchantPurse.slot.onSlotUpdated -= MerchantCoinUpdated;

            DraggedItem.currentInventorySlot = null;
            TradingManager.tradingMode = false;
            CloseInventory();
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
        private void PlayerLeftRange(GameObject player)
        {
            if (isInventoryOpen)
            {
                CloseMerchantInventory();
            }
        }
        private void PlayerEnteredRange(GameObject player)
        {
            player.TryGetComponent(out playerInventory);
            player.TryGetComponent(out playerPurse);
        }
        protected override void OnPointerUp(InventorySlotUI slotUI)
        {
            if (TradingUIManager.Instance == null)
            {
                Debug.LogWarning("Trading UI Manager reference is null. Cannot trade items!");
                return;
            }

            if (DraggedItem.currentInventorySlot != null) return;
      
            DraggedItem.targetInventorySlot = null;
            DraggedItem.currentInventorySlot = inventorySlotUI[slotUI];

            TradingUIManager.Instance.InitiateTrading(true);
        }
        public bool CheckIfItemAccepted(InventoryItemSO item)
        {
            if(itemTypesAccepted==null || itemTypesAccepted.Length==0) return false;

            if(item.itemType==null) return false;

            foreach(InventoryItemTypeSO itemType in itemTypesAccepted)
            {
                if (itemType == item.itemType) return true;
            }

            return false;
        }
    }

}