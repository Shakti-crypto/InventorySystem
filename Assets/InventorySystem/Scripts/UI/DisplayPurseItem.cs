using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class DisplayPurseItem : MonoBehaviour
    {
        [SerializeField] private InventoryItemSO item;
        [SerializeField] private PurseInventory purse;
        [SerializeField] private InventorySlotUIReferenceContainer slotInfoContainer;
        private InventorySlotUI slotUI;
        private void Start()
        {
            CreateSlotUI();
            purse.slot.onSlotUpdated += OnSlotUpdated;
            UpdateSlotInformation();
        }

        private void OnSlotUpdated(InventorySlot slot)
        {
            UpdateSlotInformation();
        }

        private void UpdateSlotInformation()
        {
            if (slotUI == null)
            {
                CreateSlotUI();
            }
            slotUI.amount.text = purse.slot.amount.ToString();
        }

        private void CreateSlotUI()
        {
            GameObject slotUIGO = slotInfoContainer.gameObject;
            Image image = slotInfoContainer.itemImage;
            TextMeshProUGUI amount = slotInfoContainer.amount;
            slotUI = new InventorySlotUI(slotUIGO, image, amount);
            slotUI.SetGameObjectActive(true);
        }
    }
}
