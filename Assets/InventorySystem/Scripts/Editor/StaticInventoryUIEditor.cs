using UnityEditor;

namespace InventorySystem.Editors
{
    [CustomEditor(typeof(StaticInventoryUI))]
    public class StaticInventoryUIEditor : Editor
    {
        SerializedProperty inventoryUIGO;
        SerializedProperty inventory;
        SerializedProperty canvas;
        SerializedProperty allowDropItems;
        SerializedProperty itemDropPoint;
        SerializedProperty itemDropPointOffset;
        SerializedProperty visibleAtStart;
        bool showReferences;
        SerializedProperty slots;
        private void OnEnable()
        {
            inventoryUIGO = serializedObject.FindProperty("inventoryUiBG");
            inventory = serializedObject.FindProperty("inventory");
            canvas = serializedObject.FindProperty("canvas");
            allowDropItems = serializedObject.FindProperty("allowDropItems");
            itemDropPoint = serializedObject.FindProperty("itemDropPoint");
            itemDropPointOffset = serializedObject.FindProperty("itemDropPointOffset");
            visibleAtStart = serializedObject.FindProperty("visibleAtStart");
            slots = serializedObject.FindProperty("slots");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            DrawBaseInventoryEditor();
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(slots);
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
