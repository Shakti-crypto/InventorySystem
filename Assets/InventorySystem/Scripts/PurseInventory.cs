using UnityEngine;

namespace InventorySystem
{
    public class PurseInventory : MonoBehaviour
    {
        [SerializeField] private string itemId;
        [SerializeField] private int amount;
        [HideInInspector]public InventorySO inventorySO;
        private InventoryItemSO item;
        public InventorySlot slot
        {
            get
            {
                if(inventorySO!=null && inventorySO.inventorySlots!=null && inventorySO.inventorySlots.Count > 0)
                {
                    return inventorySO.inventorySlots[0];
                }
                return null;
            }

        }

        private void Awake()
        {
            CreatePurse();
        }
        public void Initialze(string _itemId, int _amount)
        {
            itemId= _itemId;
            amount= _amount;
        }
        public void CreatePurse()
        {
            inventorySO=ScriptableObject.CreateInstance<InventorySO>();

            item=inventorySO.database.GetItemFromID(itemId);

            if (item==null)
            {
                Debug.LogWarning($"Invalid item Id given in {gameObject.name} purse.");
                return;
            }

            inventorySO.fixedSlots = true;
            inventorySO.maxNumberOfSlot = 1;
            InventorySlot slot=new InventorySlot(itemId,item, inventorySO);
            slot.amount = amount;
            inventorySO.inventorySlots.Add(slot);
            inventorySO.PopulateDictionary();
        }
        public int GetItemNumber()
        {
            if (inventorySO == null)
            {
                Debug.LogWarning("No inventory found.");
                return 0;
            }

            return inventorySO.GetNumberOfItemsInInventory(item);
        }
        public InventoryItemSO GetItem()
        {
            return item;
        }
        public void AddItem(int amountToAdd)
        {
            inventorySO.AddItem(itemId, amountToAdd,out _);
        }
        public void RemoveItem(int amountToRemove)
        {
            inventorySO.RemoveItem(item,amountToRemove,out _);
        }
    }

}
