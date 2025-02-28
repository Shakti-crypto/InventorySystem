using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    [System.Serializable]
    public class InventorySlotUI
    {
        private GameObject slotUIGO;
        public Image icon;
        public TextMeshProUGUI amount;
        public TextMeshProUGUI cost;
        public EventTrigger eventTrigger;
        private Transform parent;

        public InventorySlotUI(GameObject slotUI, Image icon, TextMeshProUGUI amount)
        {
            slotUIGO = slotUI;
            this.icon = icon;
            this.amount = amount;
            eventTrigger = slotUI.GetComponent<EventTrigger>();
        }
        public InventorySlotUI(GameObject slotUI, Image icon, TextMeshProUGUI amount, TextMeshProUGUI cost) : this(slotUI, icon, amount)
        {
            this.cost = cost;
        }

        public void AddEventToButton(EventTriggerType eventTriggerType, UnityAction<BaseEventData> action)
        {
            if (eventTrigger == null)
            {
                Debug.LogWarning("Event trigger reference null for " + slotUIGO.name);
                return;
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventTriggerType;
            entry.callback.AddListener(action);
            eventTrigger.triggers.Add(entry);
        }
        public void SetParent(Transform parent)
        {
            if (this.parent == parent) return;

            slotUIGO.transform.SetParent(parent, false);
        }
        public void SetGameObjectActive(bool active)
        {
            slotUIGO.SetActive(active);
        }
        public void DestroyGameobject()
        {
            GameObject.Destroy(slotUIGO.gameObject);
        }
    }
}

