using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace InventorySystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/Inventory")]
    public class InventorySO : ScriptableObject, ISerializationCallbackReceiver, ICloneable
    {
        #region Variables
        public bool fixedSlots; //Fixed slots  mean that the inventory will have max number of slots at start. No slots can be added or deleted at runtime
        public int maxNumberOfSlot;
        public string savePath;
        public List<InventorySlot> inventorySlots = new List<InventorySlot>();
        Dictionary<InventoryItemSO, List<int>> inventory;
        public InventoryItemDatabase database;

        public delegate void OnSlotRemoved(InventorySlot slot);
        public event OnSlotRemoved onSlotRemoved;

        public delegate void OnSlotAdded(InventorySlot slot);
        public event OnSlotAdded onSlotAdded;
        #endregion

        private void OnEnable()
        {
#if UNITY_EDITOR
            database = (InventoryItemDatabase)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/Resources/InventorySystem/Items Database.asset", typeof(InventoryItemDatabase));
#else
        database = Resources.Load<InventoryItemDatabase>("InventorySystem/Items Database");
#endif
        }

        #region Public functions
        public void CreateSlotsForFixedInventory()
        {
            if (!fixedSlots) return;
            for (int i = inventorySlots.Count; i < maxNumberOfSlot; i++)
            {
                InventorySlot slot = new InventorySlot(true, this);
                inventorySlots.Add(slot);
            }
        }
        public void SetInventorySlotItemsAccordingToTheirIDS()
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.RuntimeCreatedSlot) continue;
                InventoryItemSO item = database.GetItemFromID(slot.itemID);
                slot.item = item;
            }
        }
        public void PopulateDictionary()
        {
            inventory = new Dictionary<InventoryItemSO, List<int>>();
            List<InventorySlot> tempInventorySlots = new List<InventorySlot>(inventorySlots);

            foreach (InventorySlot slot in tempInventorySlots)
            {
                if (slot.inventorySO == null) slot.inventorySO = this;

                InventoryItemSO item = slot.item;
                if (item == null)
                {
                    RemoveItem(slot);
                    continue;
                }
                else
                {
                    if (slot.amount == 0)
                    {
                        RemoveItem(slot);
                        continue;
                    }
                }

                if (!inventory.ContainsKey(item))
                {
                    inventory.Add(item, new List<int>());
                }
                int amountPermittedInSlot = item.isStackable ? item.stackableAmount : 1;
                slot.amount = Mathf.Min(slot.amount, amountPermittedInSlot);

                inventory[item].Add(inventorySlots.IndexOf(slot));
            }

            void RemoveItem(InventorySlot slot)
            {
                if (fixedSlots)
                {
                    slot.RemoveItem();
                }
                else
                {
                    onSlotRemoved?.Invoke(slot);
                    inventorySlots.Remove(slot);
                }
            }
        }


        //This function will add the item, in any slot available
        public void AddItem(string itemID, int amount, out int amountAdded)
        {
            amountAdded = 0;

            InventoryItemSO item = database.GetItemFromID(itemID);

            if (item == null)
            {
                Debug.LogWarning("No item found with the given id. Please check database again.");
                return;
            }

            InventorySlot slot;

            if (!inventory.ContainsKey(item) || !item.isStackable)
            {
                if (!TryInsertItemInEmptySlot(item, out _))
                {
                    if (!TryAddNewSlot(item, out _))
                    {
                        return;
                    }
                }
            }

            DistributeItemsAcrossSlots(item, amount, out amountAdded);
        }
        public void AddItemToASlot(InventorySlot slot, InventoryItemSO item, int amount, out int amountAdded)
        {
            amountAdded = 0;
            if (slot.item != null)
            {
                if (slot.item != item)
                {
                    Debug.LogWarning($"Slot already contains a different item. Unable to add {item.name} to it.");
                    return;
                }
            }
            else
            {
                slot.item = item;
                slot.itemID = database.GetIdFromItem(item);
            }

            int amountToAdd = item.isStackable ? Mathf.Min(amount, item.stackableAmount - slot.amount) : 1-slot.amount;

            if (amountToAdd <= 0)
            {
                amountAdded = 0;
                return;
            }

            slot.AddAmount(amountToAdd);
            amountAdded = amountToAdd;
        }
        public void RemoveItem(InventoryItemSO item, int amount, out int amountRemoved)
        {
            amountRemoved = 0;

            if (!inventory.ContainsKey(item)) return;

            List<int> slotIndices = new List<int>(inventory[item]);
            slotIndices.Reverse();

            int currentItemAmount = GetNumberOfItemsInInventory(item);

            foreach (int index in slotIndices)
            {
                InventorySlot slot = inventorySlots[index];
                RemoveItemFromASlot(slot, item, amount, out int amountRemovedFromThisSlot);
                amountRemoved += amountRemovedFromThisSlot;

                if (amountRemovedFromThisSlot == amount)
                {
                    return;
                }

                currentItemAmount -= amountRemovedFromThisSlot;
                amount -= amountRemovedFromThisSlot;

                if (currentItemAmount <= 0)
                {
                    inventory.Remove(item);
                }
            }
        }
        public void RemoveItemFromASlot(InventorySlot slot, InventoryItemSO item, int amount, out int amountRemoved)
        {
            int currentItemSlotAmount = slot.amount;
            if (currentItemSlotAmount <= amount)
            {
                amountRemoved = currentItemSlotAmount;
                RemoveASlot(slot, item, inventory[item], inventorySlots.IndexOf(slot));
            }
            else
            {
                amountRemoved = amount;
                slot.RemoveAmount(amount);
            }
        }
        public void SaveInventory()
        {
            string savedData = JsonUtility.ToJson(this, true);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
            bf.Serialize(file, savedData);
            file.Close();
        }
        public void LoadInventory()
        {
            if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
                JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
                file.Close();
                PopulateDictionary();
            }
        }
        public int GetNumberOfItemsInInventory(InventoryItemSO item)
        {
            int itemNumber = 0;

            List<InventorySlot> allItemSlots = GetSlotsForItem(item);
            foreach (InventorySlot slot in allItemSlots)
            {
                itemNumber += slot.amount;
            }
            return itemNumber;
        }
        public int GetNumberOfItemsInInventory(string itemID)
        {
            int itemNumber = 0;
            InventoryItemSO item = database.GetItemFromID(itemID);

            if (item == null)
            {
                Debug.LogWarning($"Cannot find {itemID} in the inventory database.");
                return 0;
            }

            List<InventorySlot> allItemSlots = GetSlotsForItem(item);
            foreach (InventorySlot slot in allItemSlots)
            {
                itemNumber += slot.amount;
            }
            return itemNumber;
        }
        public List<InventorySlot> GetEmptySlots()
        {
            List<InventorySlot> slots = new List<InventorySlot>();
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.item == null) slots.Add(slot);
            }
            return slots;
        }
        public List<InventorySlot> GetSlotsForItem(InventoryItemSO item)
        {
            List<InventorySlot> slots = new List<InventorySlot>();
            if (inventory.ContainsKey(item))
            {
                foreach (int index in inventory[item])
                {
                    slots.Add(inventorySlots[index]);
                }
            }
            return slots;
        }
        #endregion

        //This function is only for "STACKABLE" items
        private void DistributeItemsAcrossSlots(InventoryItemSO item, int amount, out int amountAdded)
        {
            amountAdded = 0;
            int currentAmount = GetNumberOfItemsInInventory(item);

            foreach (int index in inventory[item])
            {
                InventorySlot slot = inventorySlots[index];
                AddItemToASlot(slot, item, amount, out amountAdded);

                amount -= amountAdded;
                currentAmount += amountAdded;

                if (amount <= 0)
                {
                    return;
                }
            }

            while (amount > 0)
            {
                if (!TryInsertItemInEmptySlot(item, out InventorySlot slot))
                {
                    if (!TryAddNewSlot(item, out slot))
                    {
                        break;
                    }
                }

                AddItemToASlot(slot, item, amount, out int amountAddedInThisSlot);

                if (amountAddedInThisSlot == 0) break;

                amount -= amountAddedInThisSlot;
                currentAmount += amountAddedInThisSlot;
                amountAdded += amountAddedInThisSlot;
            }
        }
        private bool TryInsertItemInEmptySlot(InventoryItemSO item, out InventorySlot slot)
        {
            slot = GetEmptySlots().Where(x => x.itemType == item.itemType || x.itemType == null).FirstOrDefault();
            if (slot != null)
            {
                slot.item = item;
                slot.amount = 0;
                AddSlotToDictionary(slot, item);
                return true;
            }
            return false;
        }
        private bool TryAddNewSlot(InventoryItemSO item, out InventorySlot slot)
        {
            if (inventorySlots.Count >= maxNumberOfSlot)
            {
                Debug.LogWarning("Inventory Full !!!");
                slot = null;
                return false;
            }

            slot = new InventorySlot(database.GetIdFromItem(item), item, this);
            inventorySlots.Add(slot);
            onSlotAdded?.Invoke(slot);
            AddSlotToDictionary(slot, item);
            return true;
        }
        private void AddSlotToDictionary(InventorySlot slot, InventoryItemSO item)
        {
            if (!inventory.ContainsKey(item))
            {
                inventory.Add(item, new List<int>());
            }
            slot.itemID = database.GetIdFromItem(item);
            inventory[item].Add(inventorySlots.IndexOf(slot));
        }
        private void RemoveASlot(InventorySlot slot, InventoryItemSO itemToRemove, List<int> indexListOfItemToBeRemoved, int slotIndex)
        {
            if (fixedSlots)
            {
                slot.RemoveItem();

                if (indexListOfItemToBeRemoved != null) indexListOfItemToBeRemoved.Remove(slotIndex);

                PopulateDictionary(); //To fix the index reference for each item after reshuffling

                return;
            }

            inventorySlots.Remove(slot);
            onSlotRemoved?.Invoke(slot);

            if (indexListOfItemToBeRemoved != null) indexListOfItemToBeRemoved.Remove(slotIndex);

            foreach (var itemsAndTheirIndices in inventory)
            {
                if (itemToRemove != null && itemsAndTheirIndices.Key == itemToRemove) continue;
                List<int> indexList = itemsAndTheirIndices.Value;

                for (int i = 0; i < indexList.Count; i++)
                {
                    if (indexList[i] > slotIndex)
                    {
                        indexList[i]--;
                    }
                }
            }
        }

        #region Interface functions
        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += UpdateItemsAndTheirIds; //This event call is required so that the items are updated only when Unity has finished serializing all the scriptable objects, and the database contains the items and thier list
#else
            UpdateItemsAndTheirIds();
#endif
            //PopulateDictionary();
        }
        private void UpdateItemsAndTheirIds()
        {
            SetInventorySlotItemsAccordingToTheirIDS();
        }
        public object Clone()
        {
            InventorySO clone = CreateInstance<InventorySO>();
            clone.name = name + "Clone";
            clone.inventorySlots = new List<InventorySlot>();
            clone.database = database;
            clone.maxNumberOfSlot = maxNumberOfSlot;
            clone.savePath = savePath;
            clone.fixedSlots = fixedSlots;
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                InventorySlot slot = inventorySlots[i];
                clone.inventorySlots.Add(slot.Clone() as InventorySlot);
                clone.inventorySlots[i].inventorySO = clone;
            }

            return clone;
        }
        public void OnBeforeSerialize() { }
        #endregion
    }

    [System.Serializable]
    public class ItemNumberLimit
    {
        public InventoryItemSO item;
        public int maxNumber;
    }
}

