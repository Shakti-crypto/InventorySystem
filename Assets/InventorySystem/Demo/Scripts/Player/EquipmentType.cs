using UnityEngine;

namespace InventorySystem.Demo
{
    [System.Serializable]
    public class EquipmentType
    {
        public string typeName;
        public InventoryItemTypeSO slotType;
        [HideInInspector] public GameObject equippedItemGO;
        public Equipment defaultEquipment;
        public Equipment[] equipments;
    }

    [System.Serializable]
    public class Equipment
    {
        public string name;
        public GameObject gameObject;
        public InventoryItemSO inventoryItem;
    }
}
