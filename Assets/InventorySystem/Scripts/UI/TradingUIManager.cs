using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class TradingUIManager : MonoBehaviour
    {
        #region Singleton
        private static TradingUIManager instance;
        public static TradingUIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(TradingUIManager)) as TradingUIManager;
                }
                return instance;
            }
        }
        #endregion

        #region Variables
        [SerializeField] private InventoryItemSO money;
        [Header("Buying Prompt")]
        [SerializeField] private GameObject buyItemPrompt;
        [SerializeField] private GameObject buyMultipleItemsPrompt;
        [SerializeField] private GameObject buySingleItemPrompt;
        [SerializeField] private TextMeshProUGUI buyMultipleItemCostText;
        [SerializeField] private TextMeshProUGUI buyMultipleItemAmount;
        [SerializeField] private TextMeshProUGUI buySingleItemCostText;
        [SerializeField] private Slider buyMultipleItemsSlider;
        [Header("Selling Prompt")]
        [SerializeField] private GameObject sellItemPrompt;
        [SerializeField] private GameObject sellMultipleItemsPrompt;
        [SerializeField] private GameObject sellSingleItemPrompt;
        [SerializeField] private TextMeshProUGUI sellMultipleItemCostText;
        [SerializeField] private TextMeshProUGUI sellMultipleItemAmount;
        [SerializeField] private TextMeshProUGUI sellSingleItemCostText;
        [SerializeField] private Slider sellMultipleItemsSlider;
        [HideInInspector] public MerchantInventoryUIController currentMerchantUIController;
        private bool buying;
        private int singleItemPrice;
        private int itemAmountInSlot;
        private int itemAmountToTrade;
        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                if (instance != this)
                {
                    Destroy(this);
                }
            }
        }
        private void Start()
        {
            buyMultipleItemsPrompt.SetActive(false);
            buySingleItemPrompt.SetActive(false);
            buyItemPrompt.SetActive(false);
            sellMultipleItemsPrompt.SetActive(false);
            sellSingleItemPrompt.SetActive(false);
            sellItemPrompt.SetActive(false);
        }
        public void InitiateTrading(bool buying)
        {
            this.buying= buying;

            if (DraggedItem.currentInventorySlot.amount <= 0)
            {
                Debug.LogWarning("Item ammount is 0. Cannot sell item.");
                DraggedItem.currentInventorySlot = null;
                return;
            }

            if (buying)
            {
                OpenBuyingPrompt();
            }
            else
            {
                if (currentMerchantUIController == null || !currentMerchantUIController.CheckIfItemAccepted(DraggedItem.currentInventorySlot.item))
                {
                    Debug.LogWarning("Merchant does not accept this item.");
                    return;
                }
                OpenSellingPrompt();
            }
        }
        private void OpenBuyingPrompt()
        {   
            singleItemPrice = DraggedItem.currentInventorySlot.item.cost;
            itemAmountInSlot = DraggedItem.currentInventorySlot.amount;
            if (itemAmountInSlot > 1)
            {
                
                SetupBuyMultipleItemPropmt();
            }
            else
            {
                SetupBuySingleItemPropmt();
            }

            Time.timeScale = 0f;
        }
        private void SetupBuyMultipleItemPropmt()
        {
            buyMultipleItemsSlider.minValue = 1;
            buyMultipleItemsSlider.maxValue = itemAmountInSlot;
            BuySlidingValueChanged(buyMultipleItemsSlider.value);

            buyMultipleItemsSlider.onValueChanged.RemoveAllListeners();
            buyMultipleItemsSlider.onValueChanged.AddListener(BuySlidingValueChanged);
            buyMultipleItemsSlider.value = itemAmountInSlot;
            buyMultipleItemsPrompt.SetActive(true);
            buyItemPrompt.SetActive(true);
        }
        private void SetupBuySingleItemPropmt()
        {
            itemAmountToTrade = 1;
            buySingleItemCostText.text = singleItemPrice.ToString();
            buySingleItemPrompt.SetActive(true);
            buyItemPrompt.SetActive(true);
        }
        private void BuySlidingValueChanged(float slideVale)
        {
            itemAmountToTrade = Mathf.RoundToInt(slideVale);
            buyMultipleItemAmount.text = itemAmountToTrade.ToString();
            buyMultipleItemCostText.text = (singleItemPrice* itemAmountToTrade).ToString();
        }
        private void OpenSellingPrompt()
        {
            singleItemPrice = DraggedItem.currentInventorySlot.item.cost;
            itemAmountInSlot = DraggedItem.currentInventorySlot.amount;
            if (itemAmountInSlot > 1)
            {
                SetupSellMultipleItemPropmt();
            }
            else
            {
                SetupSellSingleItemPropmt();
            }

            Time.timeScale = 0f;
        }
        private void SetupSellMultipleItemPropmt()
        {
            int moneyWithTheMerchant = TradingManager.merchantPurse.GetItemNumber();
            int maxNumberOfItemsThatMerchantCanBuy = moneyWithTheMerchant / singleItemPrice;
            
            if (maxNumberOfItemsThatMerchantCanBuy <= 0)
            {
                Debug.LogWarning("Merchant doesn't have enough money to buy this item.");
                ClosePrompt();
                return;
            }
            else if (maxNumberOfItemsThatMerchantCanBuy == 1)
            {
                SetupSellSingleItemPropmt();
                return;
            }

            sellMultipleItemsSlider.minValue = 1;
            sellMultipleItemsSlider.maxValue = Mathf.Min(itemAmountInSlot,maxNumberOfItemsThatMerchantCanBuy);
            SellSlidingValueChanged(sellMultipleItemsSlider.value);

            sellMultipleItemsSlider.onValueChanged.RemoveAllListeners();
            sellMultipleItemsSlider.onValueChanged.AddListener(SellSlidingValueChanged);
            sellMultipleItemsSlider.value = itemAmountInSlot;
            sellMultipleItemsPrompt.SetActive(true);
            sellItemPrompt.SetActive(true);
        }
        private void SellSlidingValueChanged(float slideVale)
        {
            itemAmountToTrade = Mathf.RoundToInt(slideVale);
            sellMultipleItemAmount.text = itemAmountToTrade.ToString();
            sellMultipleItemCostText.text = (singleItemPrice * itemAmountToTrade).ToString();
        }
        private void SetupSellSingleItemPropmt()
        {
            itemAmountToTrade = 1;
            sellSingleItemCostText.text = singleItemPrice.ToString();
            sellSingleItemPrompt.SetActive(true);
            sellItemPrompt.SetActive(true);
        }
        public void StartTrading()
        {
            if (buying)
            {
                TradingManager.TryBuyItem(money, singleItemPrice, itemAmountToTrade, () => { ClosePrompt(); });
            }
            else
            {
                TradingManager.TrySellItem(money, singleItemPrice, itemAmountToTrade, () => { ClosePrompt(); });
            }
        }
        private void ClosePrompt()
        {
            DraggedItem.currentInventorySlot = null;
            buyItemPrompt.SetActive(false);
            buyMultipleItemsPrompt.SetActive(false);
            buySingleItemPrompt.SetActive(false);
            sellItemPrompt.SetActive(false);
            sellMultipleItemsPrompt.SetActive(false);
            sellSingleItemPrompt.SetActive(false);
            Time.timeScale = 1f;
        }
        public void CancelTrading()
        {
            ClosePrompt();
        }
    }
}
