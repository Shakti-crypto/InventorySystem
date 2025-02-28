using UnityEngine;

namespace InventorySystem.Demo
{
    public class ItemPickup : MonoBehaviour
    {
        private InputManager inputManager;
        private Inventory inventory;
        [SerializeField] private PlayerRangeCheck rangeController;
        [SerializeField] private string itemID;
        [SerializeField] private int amountToAdd;

        #region Unity functions
        private void Awake()
        {
            inputManager=InputManager.Instance;
        }
        private void OnEnable()
        {
            inputManager.onInteractionPressed += InteractionButtonPressed;
            rangeController.playerEnteredRange += PlayerEnteredRange;
        }

        private void Start()
        {
            rangeController.playerInRange = false;
        }

        private void OnDisable()
        {
            inputManager.onInteractionPressed -= InteractionButtonPressed;
            rangeController.playerEnteredRange -= PlayerEnteredRange;
        }
        #endregion

        private void InteractionButtonPressed()
        {
            if (!rangeController.playerInRange) return;

            PickupObject();
        }
        private void PlayerEnteredRange(GameObject player)
        {
            player.TryGetComponent(out inventory);
        }
        private void PickupObject()
        {
            if (inventory == null)
            {
                Debug.LogWarning("Player Inventory reference not found.");
                return;
            }

            if (itemID == null)
            {
                Debug.LogWarning("No itemID found to add to inventory.");
                return;
            }

            inventory.inventorySO.AddItem(itemID, amountToAdd, out int amountAdded);

            if (amountAdded == amountToAdd)
            {
                gameObject.SetActive(false);
            }
        }
    }

}