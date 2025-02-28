using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace InventorySystem.Editors
{
    [CustomPropertyDrawer(typeof(InventorySlot))]
    public class InventorySlotDrawer : PropertyDrawer
    {
        #region Variables
        SerializedProperty itemID;
        SerializedProperty amount;
        SerializedProperty itemType;
        List<InventoryItemTypeSO> itemTypes;
        string[] itemTypeNames;
        InventorySlot slot;
        public float singleLineHeight => EditorGUIUtility.singleLineHeight;
        private int? cost;
        #endregion

        //To simulate OnEnable 
        public InventorySlotDrawer() : base()
        {   
            SearchForItemTypes();
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            itemID = property.FindPropertyRelative("itemID");
            amount = property.FindPropertyRelative("amount");
            itemType = property.FindPropertyRelative("itemType");

            var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
            slot = obj as InventorySlot;
            if (obj.GetType() == typeof(List<InventorySlot>))
            {
                var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
                slot = ((List<InventorySlot>)obj)[index];
            }

            string itemName = "Empty Slot";

            if (!string.IsNullOrEmpty(slot.itemID))
            {
                itemName = slot.itemID;
            }

            Rect foldOutBox = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldOutBox, property.isExpanded, itemName);
            if (property.isExpanded)
            {
                float imageWidth = 0;
                cost = null;
                bool itemFound=false;
                if (slot.item != null)
                {
                    itemFound = true;
                    if (slot.item.icon != null)
                    {
                        DrawImage(position, slot.item.icon, out imageWidth);
                    }
                    cost = slot.item.cost;
                }

                DrawProperties(position, imageWidth,itemFound);
            }
            EditorGUI.EndProperty();
        }
        private void DrawImage(Rect position, Sprite image, out float imageWidth)
        {
            if (image == null)
            {
                imageWidth = 0;
                return;
            }
            imageWidth = 100;
            Rect rect = new Rect(position.min.x, position.min.y + singleLineHeight * 2, imageWidth, 100);
            Texture2D itemIcon = AssetPreview.GetAssetPreview(image);
            GUIStyle style = new GUIStyle();
            style.normal.background = itemIcon;
            EditorGUI.LabelField(rect, GUIContent.none, style);
        }
        private void DrawProperties(Rect position, float imageWidth, bool itemFound)
        {
            float xPos = position.min.x + imageWidth + 15;
            float yPos = position.min.y + singleLineHeight * 2;
            float width = 200;
            float height = EditorGUIUtility.singleLineHeight;

            Rect currentRect = new Rect(xPos, yPos, width, height);

            EditorGUIUtility.labelWidth = 60f;

            EditorGUI.BeginChangeCheck();
            string newItemID = EditorGUI.DelayedTextField(currentRect, "ItemID ", itemID.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                itemID.stringValue = newItemID;
                itemID.serializedObject.ApplyModifiedProperties();
            }

            if (itemFound)
            {

                if (slot.item.isStackable)
                {
                    currentRect.y += (EditorGUIUtility.singleLineHeight + 10);
                    EditorGUIUtility.labelWidth = 60f;
                    EditorGUI.PropertyField(currentRect, amount, new GUIContent("Amount "));
                }

                string displayCost = "Not found, refresh inspector.";
                if (cost != null)
                {
                    displayCost = cost.Value.ToString();
                }

                currentRect.y += (EditorGUIUtility.singleLineHeight + 10);
                EditorGUI.LabelField(currentRect, new GUIContent($"Cost : {displayCost}"));
            }

            currentRect.y += (EditorGUIUtility.singleLineHeight + 10);
            DrawItemTypeProperties(currentRect);
        }
        private void DrawItemTypeProperties(Rect currentRect)
        {
            int currentSlotTypeIndex = 0;

            if (itemType.objectReferenceValue != null)
            {
                currentSlotTypeIndex = GeItemTypeIndex(itemType.objectReferenceValue.name);
            }

            EditorGUI.LabelField(currentRect,new GUIContent("Item Type this slot accepts : "));
            currentRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUIUtility.labelWidth = 60f;
            EditorGUI.PropertyField(currentRect, itemType,new GUIContent(""));

            currentRect.y+= (EditorGUIUtility.singleLineHeight+5);
            int newSlotIndex=EditorGUI.Popup(currentRect, currentSlotTypeIndex, itemTypeNames);

            if (newSlotIndex != currentSlotTypeIndex)
            {
                SetItemType(newSlotIndex);
            }

            currentRect.y+= (EditorGUIUtility.singleLineHeight + 5);
            if (GUI.Button(currentRect, "Refresh Item Type"))
            {
                SearchForItemTypes();
            }

            currentRect.y+= (EditorGUIUtility.singleLineHeight + 5);
            if (GUI.Button(currentRect, "Create new Item Type"))
            {
                CreateNewItemType();
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLines = 1;
            if (property.isExpanded)
            {
                var obj = fieldInfo.GetValue(property.serializedObject.targetObject);

                slot = obj as InventorySlot;
                if (obj.GetType() == typeof(List<InventorySlot>))
                {
                    var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
                    slot = ((List<InventorySlot>)obj)[index];
                }

                if (slot.item != null)
                {
                    totalLines += 3;
                }
                totalLines += 10;
            }

            return EditorGUIUtility.singleLineHeight * totalLines;
        }
        private void SearchForItemTypes()
        {
            itemTypes = new List<InventoryItemTypeSO>();

            string[] guids = AssetDatabase.FindAssets("t:InventoryItemTypeSO");
            itemTypes = guids.Select(guid =>
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                return AssetDatabase.LoadAssetAtPath<InventoryItemTypeSO>(path);
            }).ToList();

            itemTypeNames = new string[itemTypes.Count + 1];
            itemTypeNames[0] = "All Items";

            for (int i = 1; i <= itemTypes.Count; i++)
            {
                itemTypeNames[i] = itemTypes[i - 1].name;
            }

            //SetSlotType();
        }
        private void CreateNewItemType()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create a new item type", "ItemType", "asset", "Enter a name for the item type");
            if (string.IsNullOrEmpty(path)) return;

            InventoryItemTypeSO newItemType = ScriptableObject.CreateInstance<InventoryItemTypeSO>();
            AssetDatabase.CreateAsset(newItemType, path);
            AssetDatabase.SaveAssets();

            SearchForItemTypes();
        }
        private int GeItemTypeIndex(string name) { 
            for(int i = 0; i < itemTypeNames.Length; i++)
            {
                if(name == itemTypeNames[i])
                {
                    return i;
                }
            }

            return 0;
        }
        private void SetItemType(int newItemTypeIndex)
        {
            if (newItemTypeIndex == 0)
            {
                itemType.objectReferenceValue = null;
                return;
            }

            itemType.objectReferenceValue = itemTypes[newItemTypeIndex - 1];
        }
    }
}
