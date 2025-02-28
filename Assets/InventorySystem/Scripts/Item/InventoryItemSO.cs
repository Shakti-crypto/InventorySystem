using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Inventory Item", menuName = "Inventory/Inventory Item")]
    public class InventoryItemSO : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public InventoryItemTypeSO itemType;
        public bool isStackable;
        public int stackableAmount;
        public int cost;
        public GameObject pickupPrefab;
    }
}
