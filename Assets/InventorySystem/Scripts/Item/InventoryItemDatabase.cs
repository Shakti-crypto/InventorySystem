using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Items Databse", menuName = "Inventory/Items Databse")]
    public class InventoryItemDatabase : ScriptableObject
    {
        public List<InventoryItemData> items;

        public InventoryItemSO GetItemFromID(string id)
        {
            if (items == null || items.Count == 0) return null;

            InventoryItemSO item = null;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id) item = items[i].item;
            }
            return item;
        }
        public string GetIdFromItem(InventoryItemSO item)
        {
            if (items == null || items.Count == 0) return null;

            string id = null;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == item) id = items[i].ID;
            }
            return id;
        }
        public bool CheckIfIdPresent(string ID)
        {
            bool idFound = false;
            foreach (InventoryItemData item in items)
            {
                if (item.ID == ID)
                {
                    idFound = true;
                    break;
                }
            }
            return idFound;
        }
        public void AddItem(InventoryItemSO item, string ID)
        {
            InventoryItemData itemData = new InventoryItemData();
            itemData.ID = ID;
            itemData.item = item;
            if (item == null)
            {
                Debug.LogWarning("Unable to add item to database.");
                return;
            }

            if (items == null) items = new List<InventoryItemData>();

            items.Add(itemData);
        }

    }

    [System.Serializable]
    public class InventoryItemData
    {
        public InventoryItemSO item;
        public string ID;
    }

}