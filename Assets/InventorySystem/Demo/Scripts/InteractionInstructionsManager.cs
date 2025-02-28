using TMPro;
using UnityEngine;

namespace InventorySystem.Demo
{
    public class InteractionInstructionsManager : MonoBehaviour
    {
        #region Singleton
        private static InteractionInstructionsManager instance;
        public static InteractionInstructionsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(InteractionInstructionsManager)) as InteractionInstructionsManager;
                }
                return instance;
            }
        }

        #endregion

        [SerializeField] private GameObject instrucitonUIBG;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private MerchantInventoryUIController[] merchants;
        [SerializeField] private PlayerRangeCheck[] merchantRangeCheck;
        [SerializeField] private ChestInventoryUIController[] chests;
        [SerializeField] private PlayerRangeCheck[] chestRangeCheck;
        private bool isChestOpenedOnce;
        private bool isTradingInitiatedOnce;

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
            isChestOpenedOnce=false;
            isTradingInitiatedOnce= false;
            SubscribeToEvents();
        }
        private void OnDisable()
        {
            foreach (ChestInventoryUIController controller in chests)
            {
                controller.onChestOpen -= ChestOpened;
            }

            foreach (MerchantInventoryUIController controller in merchants)
            {
                controller.onTradeIntitiated -= TradingInitiated;
            }

            foreach (PlayerRangeCheck playerRangeCheck in chestRangeCheck)
            {
                playerRangeCheck.playerEnteredRange -= PlayerEnteredNearChest;
                playerRangeCheck.playerLeftRange -= PlayerLeftRange;
            }

            foreach (PlayerRangeCheck playerRangeCheck in merchantRangeCheck)
            {
                playerRangeCheck.playerEnteredRange -= PlayerEnteredNearMerchant;
                playerRangeCheck.playerLeftRange -= PlayerLeftRange;
            }
        }

        private void SubscribeToEvents()
        {
            foreach (ChestInventoryUIController controller in chests)
            {
                controller.onChestOpen += ChestOpened;
            }

            foreach (MerchantInventoryUIController controller in merchants)
            {
                controller.onTradeIntitiated += TradingInitiated;
            }

            foreach(PlayerRangeCheck playerRangeCheck in chestRangeCheck)
            {
                playerRangeCheck.playerEnteredRange += PlayerEnteredNearChest;
                playerRangeCheck.playerLeftRange += PlayerLeftRange;
            }            
            
            foreach(PlayerRangeCheck playerRangeCheck in merchantRangeCheck)
            {
                playerRangeCheck.playerEnteredRange += PlayerEnteredNearMerchant;
                playerRangeCheck.playerLeftRange += PlayerLeftRange;
            }
        }
        private void ChestOpened()
        {
            if (!isChestOpenedOnce)
            {
                isChestOpenedOnce = true;
            }
            DisableInstructions();
        }
        private void TradingInitiated()
        {
            if (!isTradingInitiatedOnce)
            {
                isTradingInitiatedOnce = true;
            }
            DisableInstructions();
        }
        private void PlayerEnteredNearChest(GameObject Player)
        {
            if (isChestOpenedOnce) return;

            SetInstructions("Press E to open Chest");
        }
        private void PlayerEnteredNearMerchant(GameObject Player)
        {
            if (isTradingInitiatedOnce) return;

            SetInstructions("Press E to initiate Trading");
        }
        private void PlayerLeftRange(GameObject player)
        {
            DisableInstructions();
        }
        public void SetInstructions(string instructions)
        {
            instructionText.text = instructions;
            instrucitonUIBG.SetActive(true);
        }
        public void DisableInstructions()
        {
            instrucitonUIBG.SetActive(false);
        }
    }

}
