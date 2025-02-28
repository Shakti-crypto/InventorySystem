using UnityEngine;

namespace InventorySystem.Demo
{
    public class PlayerEquipmentManager : MonoBehaviour
    {
        [SerializeField] private EquipmentType[] equipmentTypes;
        [SerializeField] private Inventory playerEquipmentInventory;

        private void Start()
        {
            foreach (InventorySlot slot in playerEquipmentInventory.inventorySlots)
            {
                SlotUpdated(slot);
                slot.onSlotUpdated += SlotUpdated;
            }
        }
        private void SlotUpdated(InventorySlot slot)
        {
            foreach (EquipmentType equipmentType in equipmentTypes)
            {
                if (equipmentType.slotType == slot.itemType)
                {
                    SelectRightEquipment(slot, equipmentType);
                    break;
                }
            }
        }
        private void SelectRightEquipment(InventorySlot slot, EquipmentType equipmentType)
        {
            Equipment equippedItem = equipmentType.defaultEquipment;

            foreach (Equipment equipment in equipmentType.equipments)
            {
                if (slot.item == equipment.inventoryItem)
                {
                    equippedItem = equipment;
                    break;
                }
            }

            if (equipmentType.equippedItemGO == equippedItem.gameObject) return;

            if (equipmentType.equippedItemGO != null) //the equippedItem may be null at the start if the player is wearing no equipment
            {
                equipmentType.equippedItemGO.SetActive(false);
            }
            equippedItem.gameObject.SetActive(true);
            equipmentType.equippedItemGO = equippedItem.gameObject;
        }
    }
}


