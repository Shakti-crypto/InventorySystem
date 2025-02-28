using UnityEditor;

namespace InventorySystem.Editors
{
    [CustomEditor(typeof(ChestInventoryUIController))]
    public class ChestInventoryUIControllerEditor : Editor
    {
        #region Variables
        SerializedProperty inventoryUIGO;
        SerializedProperty inventory;
        SerializedProperty canvas;
        SerializedProperty allowDropItems;
        SerializedProperty pickupPrefabDropPoint;
        SerializedProperty pickupPrefabsOsset;
        bool showReferences;

        SerializedProperty slotParent;
        SerializedProperty slotUIPrefab;

        SerializedProperty rangeCheck;
        #endregion

        private void OnEnable()
        {
            inventoryUIGO = serializedObject.FindProperty("inventoryUiBG");
            inventory = serializedObject.FindProperty("inventory");
            canvas = serializedObject.FindProperty("canvas");
            allowDropItems = serializedObject.FindProperty("allowDropItems");
            pickupPrefabDropPoint = serializedObject.FindProperty("itemDropPoint");
            pickupPrefabsOsset = serializedObject.FindProperty("itemDropPointOffset");

            slotParent = serializedObject.FindProperty("slotParent");
            slotUIPrefab = serializedObject.FindProperty("slotUIPrefab");
            rangeCheck = serializedObject.FindProperty("rangeCheck");
        }

        public override void OnInspectorGUI()
        {
            DrawBaseInventoryEditor();
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(rangeCheck);
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

            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(slotParent);
            EditorGUILayout.PropertyField(slotUIPrefab);
        }
    }
}
