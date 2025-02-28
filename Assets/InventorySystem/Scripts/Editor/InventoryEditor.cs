using UnityEditor;

namespace InventorySystem.Editors
{
    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : Editor
    {
        private Editor _editor;
        private SerializedProperty inventorySO;
        private SerializedProperty inventoryUI;

        private void OnEnable()
        {
            inventorySO = serializedObject.FindProperty("inventorySO");
            inventoryUI = serializedObject.FindProperty("inventoryUI");
        }
        public override void OnInspectorGUI()
        {   
            serializedObject.Update();
            EditorGUILayout.PropertyField(inventoryUI);
            EditorGUILayout.PropertyField(inventorySO);
            EditorGUILayout.Space(10);
            if (inventorySO.objectReferenceValue != null)
            {
                CreateCachedEditor(inventorySO.objectReferenceValue, null, ref _editor);
                _editor.OnInspectorGUI();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
