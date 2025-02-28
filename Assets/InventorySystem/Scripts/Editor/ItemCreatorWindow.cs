using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace InventorySystem.Editors
{
    public class ItemCreatorWindow : EditorWindow
    {
        #region Variables
        Texture2D bg;
        GUISkin skin;
        Sprite inventoryUIBG;
        Color32 buttonBGColor = new Color32(151, 113, 74, 200);
        Color32 defaultBGColor = new Color32(151, 113, 74, 100);

        Rect leftSideRect;
        Rect rightSideRect;
        Rect footer;

        string itemName;
        string ID;
        string previousID;
        Sprite icon;
        List<InventoryItemTypeSO> itemTypes;
        string[] itemTypeNames;
        int selecteditemTypeIndex;
        InventoryItemTypeSO selecteditemType;
        bool isStackable;
        int stackableAmount;
        int cost;
        GameObject pickupPrefab;

        InventoryItemDatabase database;
        #endregion

        [MenuItem("Tools/Inventory/Item Creator",false,2)]
        public static void ShowWindow()
        {
            ItemCreatorWindow window = GetWindow<ItemCreatorWindow>("Inventory Item Creator");
            window.minSize = new Vector2(450, 500);
            window.maxSize = new Vector2(500, 550);
        }
        private void OnEnable()
        {
            LoadAssets();
            SearchForItemTypes();
        }
        private void LoadAssets()
        {
            if (database == null)
            {
                database = Resources.Load<InventoryItemDatabase>("InventorySystem/Items Database");
            }

            if (skin == null)
            {
                skin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/EditorWindowSkin.guiskin", typeof(GUISkin));
            }

            if (inventoryUIBG == null)
            {
                inventoryUIBG = Resources.Load<Sprite>("InventorySystem/InventorySlotBG");
            }

            if (bg == null)
            {
                bg = Resources.Load<Texture2D>("InventorySystem/InventoryUIBG");
            }
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
            itemTypeNames[0] = "No item type";

            for (int i = 1; i <= itemTypes.Count; i++)
            {
                itemTypeNames[i] = itemTypes[i - 1].name;
            }

            SetItemType();
        }
        private void OnGUI()
        {
            DefineLayout();
            DrawBackground();
            DrawLeftSide();
            DrawRightSide();
            DrawFooter();
        }
        private void DefineLayout()
        {
            leftSideRect = new Rect(20, 10, position.width / 2, position.height * 0.75f);
            rightSideRect = new Rect(position.width / 2 + 20, 10, position.width / 2, position.height * 0.8f);
            footer = new Rect(0, position.height * 0.8f, position.width, 30);
        }
        private void DrawBackground()
        {
            if (bg == null) return;

            Rect bgRect = new Rect(0, 0, position.width, position.height);
            GUI.DrawTexture(bgRect, bg);
        }
        private void DrawLeftSide()
        {
            GUI.backgroundColor = defaultBGColor;

            GUILayout.BeginArea(leftSideRect);
            if (inventoryUIBG != null)
            {
                Rect bgRect = new Rect(10, 30, 150, 150);
                DrawTexturePreview(bgRect, inventoryUIBG);

                if (icon != null)
                {
                    float iconWidth = bgRect.width * 0.8f;
                    float iconHeight = bgRect.height * 0.8f;
                    float iconX = bgRect.x + (bgRect.width - iconWidth) / 2f;
                    float iconY = bgRect.y + (bgRect.height - iconHeight) / 2f;

                    Rect iconRect = new Rect(iconX, iconY, iconWidth, iconHeight);
                    DrawTexturePreview(iconRect, icon);
                }
                EditorGUILayout.Space(190);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Slot UI BG:", GetStyle("Body"), GUILayout.Width(80));
            inventoryUIBG = (Sprite)EditorGUILayout.ObjectField(inventoryUIBG, typeof(Sprite), false, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);

            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Refresh item types", GUILayout.Width(200), GUILayout.Height(30)))
            {
                SearchForItemTypes();
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Create a new item type", GUILayout.Width(200), GUILayout.Height(30)))
            {
                CreateNewItemType();
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Reset properties", GUILayout.Width(200), GUILayout.Height(30)))
            {
                ResetProperties();
            }
            GUILayout.EndArea();
        }
        private void DrawRightSide()
        {
            GUI.backgroundColor = defaultBGColor;

            GUILayout.BeginArea(rightSideRect);
            EditorGUILayout.Space(20);

            GUILayout.Label("Database:", GetStyle("Body"), GUILayout.Width(70));
            database = (InventoryItemDatabase)EditorGUILayout.ObjectField(database, typeof(InventoryItemDatabase), false, GUILayout.Width(180));

            EditorGUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GetStyle("Body"), GUILayout.Width(50));
            itemName = EditorGUILayout.TextField(itemName, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("ID:", GetStyle("Body"), GUILayout.Width(50));
            ID = EditorGUILayout.TextField(ID, GUILayout.Width(100));

            if (ID != previousID)
            {
                if (database == null)
                {
                    Debug.LogError("Database not found. Please assign a valid Item Database to continue.");
                    EditorGUILayout.HelpBox("Database not found. Please assign a valid Item Database to continue.", MessageType.Error);
                    ID = null;
                }
                else
                {
                    if (CheckIfIdAlreadyPresent())
                    {
                        Debug.LogError("This ID already exists in the databse. Make sure that each item has unique ID.");
                        EditorGUILayout.HelpBox("This ID already exists in the databse. Make sure that each item has unique ID.", MessageType.Error);
                        ID = null;
                    }
                }
                previousID = ID;
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Icon:", GetStyle("Body"), GUILayout.Width(50)); // Label with fixed width
            icon = (Sprite)EditorGUILayout.ObjectField(icon, typeof(Sprite), false, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Item Type:", GetStyle("Body"), GUILayout.Width(70)); // Label with fixed width
            int newSlotTypeIndex = EditorGUILayout.Popup(selecteditemTypeIndex, itemTypeNames, GUILayout.Width(100));
            if (newSlotTypeIndex != selecteditemTypeIndex)
            {
                selecteditemTypeIndex = newSlotTypeIndex;

                SetItemType();
            }
            EditorGUILayout.EndHorizontal();

            selecteditemType = (InventoryItemTypeSO)EditorGUILayout.ObjectField(selecteditemType, typeof(InventoryItemTypeSO), false, GUILayout.Width(180));

            EditorGUILayout.Space(30);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("isStackable :", GetStyle("Body"), GUILayout.Width(90));
            isStackable = EditorGUILayout.Toggle(isStackable);
            EditorGUILayout.EndHorizontal();

            if (isStackable)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Stackable amount :", GetStyle("Body"), GUILayout.Width(140));
                stackableAmount = EditorGUILayout.IntField(stackableAmount, GUILayout.Width(40));
                stackableAmount=Mathf.Max(stackableAmount, 1);
                EditorGUILayout.EndHorizontal();

            }

            stackableAmount = Mathf.Abs(stackableAmount);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Cost :", GetStyle("Body"), GUILayout.Width(50));
            cost = EditorGUILayout.IntField(cost, GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();
            cost = Mathf.Abs(cost);
            EditorGUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.normal.textColor = Color.black;
            EditorGUILayout.LabelField("If this is disabled, the item will deleted from the inventory if it's quantity is 0.", customStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            GUILayout.Label("Pickup Prefab :", GetStyle("Body"), GUILayout.Width(90));
            pickupPrefab = (GameObject)EditorGUILayout.ObjectField(pickupPrefab, typeof(GameObject), false, GUILayout.Width(150));
            GUILayout.EndArea();
        }
        private void DrawFooter()
        {
            GUILayout.BeginArea(footer);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Create Item", GUILayout.Width(200), GUILayout.Height(30)))
            {
                CreateItem();
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        private void SetItemType()
        {
            if (selecteditemTypeIndex == 0)
            {
                selecteditemType = null;
                return;
            }

            selecteditemType = itemTypes[selecteditemTypeIndex - 1];
        }
        private GUIStyle GetStyle(string styleName)
        {
            if (skin == null || skin.GetStyle(styleName) == null)
            {
                GUI.skin.label.normal.textColor = Color.black;
                return GUI.skin.label;
            }

            return skin.GetStyle(styleName);
        }

        //Function to draw sprites(GuiLayout.DrawTexture draws entire spritesheet)
        private void DrawTexturePreview(Rect position, Sprite sprite)
        {
            Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            Rect coords = sprite.textureRect;
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            Vector2 center = position.center;
            position.width = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }
        private bool CheckIfIdAlreadyPresent()
        {
            if (database == null)
            {
                Debug.LogWarning("Item Database not found. Create an item database first.");
                return false;
            }

            return database.CheckIfIdPresent(ID);
        }
        private void ResetProperties()
        {
            itemName = null;
            icon = null;
            ID = null;
            if (itemTypes.Count > 0)
            {
                selecteditemTypeIndex = 0;
                SetItemType();
            }

            isStackable = false;
            stackableAmount = 1;
            cost = 0;
            pickupPrefab = null;
        }
        private void CreateItem()
        {
            if (database == null)
            {
                Debug.LogWarning("Unable to create item because no database found.");
                return;
            }

            if (string.IsNullOrEmpty(ID))
            {
                Debug.LogError("Make sure all the entries are properly set. Unable to create item.");
                return;
            }

            if (CheckIfIdAlreadyPresent())
            {
                return;
            }

            string path = EditorUtility.SaveFilePanelInProject("Create new item", itemName, "asset", "Select the final name ");
            if (string.IsNullOrEmpty(path)) return;

            InventoryItemSO item = ScriptableObject.CreateInstance<InventoryItemSO>();

            item.itemName = itemName;
            item.icon = icon;
            item.itemType = selecteditemType;
            item.isStackable = isStackable;
            item.stackableAmount = stackableAmount;
            item.cost = cost;
            item.pickupPrefab = pickupPrefab;

            AssetDatabase.CreateAsset(item, path);

            database.AddItem(item, ID);
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log("Item Created!!!");

            ResetProperties();
        }
        private void CreateNewItemType()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create a new item type", "ItemType", "asset", "Enter a name for the item type");
            if (string.IsNullOrEmpty(path)) return;

            InventoryItemTypeSO newSlotType = ScriptableObject.CreateInstance<InventoryItemTypeSO>();
            AssetDatabase.CreateAsset(newSlotType, path);
            AssetDatabase.SaveAssets();

            SearchForItemTypes();
            selecteditemType = newSlotType;
            SetItemType();

        }
    }

}
