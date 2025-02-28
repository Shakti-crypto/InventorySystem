using UnityEditor;

namespace InventorySystem.Editors
{

    [CustomEditor(typeof(MerchantInventoryUIController))]
    public class MerchantInventoryUIControllerEditor : Editor
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

        SerializedProperty itemTypesAccepted;
        SerializedProperty merchantPurse;
        SerializedProperty coinAmountText;
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

            itemTypesAccepted = serializedObject.FindProperty("itemTypesAccepted");
            merchantPurse = serializedObject.FindProperty("merchantPurse");
            coinAmountText = serializedObject.FindProperty("coinAmountText");
            rangeCheck = serializedObject.FindProperty("rangeCheck");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            DrawBaseInventoryEditor();

            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(itemTypesAccepted);
            EditorGUILayout.PropertyField(merchantPurse);
            EditorGUILayout.PropertyField(coinAmountText);
            EditorGUILayout.PropertyField(rangeCheck);

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

            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(slotParent);
            EditorGUILayout.PropertyField(slotUIPrefab);
        }
    }
}
