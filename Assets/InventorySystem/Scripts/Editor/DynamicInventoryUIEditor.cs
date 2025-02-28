using UnityEditor;

namespace InventorySystem.Editors
{
    [CustomEditor(typeof(DynamicInventoryUI))]
    public class DynamicInventoryUIEditor : Editor
    {
        #region Variables
        SerializedProperty inventoryUIGO;
        SerializedProperty inventory;
        SerializedProperty canvas;
        SerializedProperty allowDropItems;
        SerializedProperty itemDropPoint;
        SerializedProperty itemDropPointOffset;
        SerializedProperty visibleAtStart;
        bool showReferences;

        SerializedProperty slotParent;
        SerializedProperty slotUIPrefab;
        #endregion

        private void OnEnable()
        {
            inventoryUIGO = serializedObject.FindProperty("inventoryUiBG");
            inventory = serializedObject.FindProperty("inventory");
            canvas = serializedObject.FindProperty("canvas");
            allowDropItems = serializedObject.FindProperty("allowDropItems");
            itemDropPoint = serializedObject.FindProperty("itemDropPoint");
            itemDropPointOffset = serializedObject.FindProperty("itemDropPointOffset");
            visibleAtStart = serializedObject.FindProperty("visibleAtStart");

            slotParent = serializedObject.FindProperty("slotParent");
            slotUIPrefab = serializedObject.FindProperty("slotUIPrefab");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            DrawBaseInventoryEditor();
            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(slotParent);
            EditorGUILayout.PropertyField(slotUIPrefab);
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawBaseInventoryEditor()
        {
            showReferences = EditorGUILayout.BeginFoldoutHeaderGroup(showReferences, " References");

            if (showReferences)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.ObjectField(inventoryUIGO);
                EditorGUILayout.PropertyField(inventory);
                EditorGUILayout.ObjectField(canvas);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.PropertyField(allowDropItems);

            if (allowDropItems.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(itemDropPoint);
                EditorGUILayout.PropertyField(itemDropPointOffset);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(visibleAtStart);
        }
    }
}
