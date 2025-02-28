using UnityEditor;

namespace InventorySystem.Demo.Editors
{
    [CustomEditor(typeof(PlayerInventoryUIController))]
    public class PlayerInventoryUIControllerEditor : Editor
    {
        SerializedProperty inventoryUIGO;
        SerializedProperty inventory;
        SerializedProperty canvas;
        SerializedProperty allowDropItems;
        SerializedProperty pickupPrefabDropPoint;
        SerializedProperty pickupPrefabsOsset;
        bool showReferences;
        SerializedProperty equipmentUIController;

        private void OnEnable()
        {
            inventoryUIGO = serializedObject.FindProperty("inventoryUiBG");
            inventory = serializedObject.FindProperty("inventory");
            canvas = serializedObject.FindProperty("canvas");
            allowDropItems = serializedObject.FindProperty("allowDropItems");
            pickupPrefabDropPoint = serializedObject.FindProperty("itemDropPoint");
            pickupPrefabsOsset = serializedObject.FindProperty("itemDropPointOffset");
            equipmentUIController = serializedObject.FindProperty("equipmentUIController");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            DrawBaseInventoryEditor();
            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(equipmentUIController);
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
                EditorGUILayout.PropertyField(pickupPrefabDropPoint);
                EditorGUILayout.PropertyField(pickupPrefabsOsset);
                EditorGUI.indentLevel--;
            }

        }
    }
}
