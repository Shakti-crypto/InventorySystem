using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Item Type", menuName = "Inventory/Item Type")]
    public class InventoryItemTypeSO : ScriptableObject
    {
        public string itemType;
    }
}

