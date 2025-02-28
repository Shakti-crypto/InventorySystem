using UnityEditor;
using UnityEngine;


namespace InventorySystem.Editors
{
    [CustomEditor(typeof(InventoryItemSO))]
    public class InventoryItemSOEditor : Editor
    {
        SerializedProperty itemName;
        SerializedProperty icon;
        SerializedProperty itemType;
        SerializedProperty isStackable;
        SerializedProperty stackableAmount;
        SerializedProperty pickupPrefab;
        SerializedProperty cost;
        private InventoryItemSO itemSO;

        private void OnEnable()
        {
            itemSO = (InventoryItemSO)target;
            itemName = serializedObject.FindProperty("itemName");
            icon = serializedObject.FindProperty("icon");
            itemType = serializedObject.FindProperty("itemType");
            isStackable = serializedObject.FindProperty("isStackable");
            stackableAmount = serializedObject.FindProperty("stackableAmount");
            pickupPrefab = serializedObject.FindProperty("pickupPrefab");
            cost = serializedObject.FindProperty("cost");
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(itemSO.name.ToUpper(), EditorStyles.boldLabel);
            DrawIcon();
            EditorGUILayout.Space(10);

            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(itemName);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(icon);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(itemType);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(isStackable);
            EditorGUILayout.Space();

            if (isStackable.boolValue)
            {
                EditorGUILayout.PropertyField(stackableAmount);
                itemSO.stackableAmount=Mathf.Max(1,itemSO.stackableAmount);
                EditorGUILayout.Space();

            }
            EditorGUILayout.PropertyField(cost);
            EditorGUILayout.Space();            
            

            EditorGUILayout.PropertyField(pickupPrefab);

            serializedObject.ApplyModifiedProperties();
        }
        private void DrawIcon()
        {
            if (itemSO.icon == null)
            {
                return;
            }
            Texture2D itemIcon = AssetPreview.GetAssetPreview(itemSO.icon);
            GUILayout.Label("", GUILayout.Height(100), GUILayout.Width(100));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), itemIcon);
            EditorGUILayout.Space(15);
        }
    }
}
