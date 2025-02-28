using UnityEngine;

namespace InventorySystem.Demo
{
    public class PlayerStatsManager : MonoBehaviour
    {
        [SerializeField] private InventoryItemTypeSO consumableItemType;

        private void Start()
        {
            QuickUseInventoryController.QuickUseItemUsed += QuickUseItemConsumed;
        }
        private bool QuickUseItemConsumed(InventoryItemSO item)
        {
            if (item == null) return false;

            if(item.itemType != consumableItemType) return false;

            Debug.Log($"Consumed {item}");
            return true;
        }
    }
}
