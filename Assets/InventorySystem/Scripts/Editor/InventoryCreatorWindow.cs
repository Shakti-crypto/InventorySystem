using InventorySytem.Editors;
using UnityEditor;
using UnityEngine;

namespace InventorySystem.Editors
{
    public class InventoryCreatorWindow : EditorWindow
    {
        #region Variables
        Rect layoutRect;
        GUISkin skin;
        Texture2D bg;
        Color32 buttonBGColor = new Color32(151, 113, 74, 200);
        Color32 normalBGColor;

        Texture2D databaseInfoTex;
        Texture2D inventoryInspectorTex;
        Texture2D fixedSlotsEnabledTex;
        Texture2D fixedSlotsDisabledTex;

        int currentPanelIndex;

        InventoryItemDatabase database;
        GameObject objectToAttachInventory;
        bool fixedSlots;
        int maxInventorySlots;
        string savePath;
        bool? inventoryCreated;
        Inventory inventory;
        #endregion

        [MenuItem("Tools/Inventory/Inventory Creator",false,1)]
        public static void ShowWindow()
        {
            InventoryCreatorWindow window = GetWindow<InventoryCreatorWindow>("Inventory Wizard");
            window.minSize = new Vector2(600, 600);
            window.maxSize = new Vector2(600, 600);
        }
        private void OnEnable()
        {
            LoadAssets();
        }
        private void LoadAssets()
        {
            if (database == null)
            {
                database = Resources.Load<InventoryItemDatabase>("InventorySystem/Items Database");
            }

            if (bg == null)
            {
                bg = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/InventoryWizardGradient.png",typeof(Texture2D));
            }

            if (skin == null)
            {
                skin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/EditorWindowSkin.guiskin", typeof(GUISkin));
            }

            if (databaseInfoTex == null)
            {
                databaseInfoTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/ItemDatabase.png",typeof(Texture2D));
            }

            if (inventoryInspectorTex == null)
            {
                inventoryInspectorTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/InventoryInspector.png",typeof(Texture2D));
            }

            if (fixedSlotsEnabledTex == null)
            {
                fixedSlotsEnabledTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/FixedSlotsEnabled.png",typeof(Texture2D));
            }

            if (fixedSlotsDisabledTex == null)
            {
                fixedSlotsDisabledTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/FixedSlotsDisabled.png",typeof(Texture2D));
            }
        }
        private void OnGUI()
        {
            layoutRect = new Rect(0, 30, position.width, position.height);
            normalBGColor = GUI.backgroundColor;
            DrawBG();
            switch (currentPanelIndex)
            {
                case 0:
                    DatabasePanel();
                    break;
                case 1:
                    MainGameObjectProperty();
                    break;
                case 2:
                    FixedSlotsBoolProperty();
                    break;
                case 3:
                    MaximumInventorySlotsProperty();
                    break;
                case 4:
                    SavePathProperty();
                    break;
                case 5:
                    InventoryCreated();
                    break;
            }
        }
        private void DrawBG()
        {
            if (bg == null) return;

            Rect bgRect = new Rect(0, 0, position.width, position.height);
            GUI.DrawTexture(bgRect, bg);
        }
        void DatabasePanel()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Welcome to the Inventory Creator!",GetStyle("Header1"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(30);

            if (database == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Item Database not found-You need to create one before proceeding.", GetStyle("Header2"));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;
                if (GUILayout.Button("Create database", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    CreateDatabase();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
                customStyle.normal.textColor = GetTextColor();
                EditorGUILayout.LabelField("Important: Create the database inside \"Resources/InventorySystem\" folder.", customStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Database Found! You can now continue.", GetStyle("Header2"));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.ObjectField(database, typeof(InventoryItemDatabase), false, GUILayout.Width(200));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;
                if (GUILayout.Button("Next!!", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    currentPanelIndex = 1;
                    OnGUI();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            DrawDatabaseInfo(GUILayoutUtility.GetLastRect());

            GUILayout.EndArea();
        }
        private void MainGameObjectProperty()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Drag the gameobject to which you want to add an inventory.", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            objectToAttachInventory = (GameObject)EditorGUILayout.ObjectField(objectToAttachInventory, typeof(GameObject), true, GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (objectToAttachInventory != null)
            {
                if (objectToAttachInventory.TryGetComponent(out Inventory inventory))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
                    customStyle.normal.textColor = GetTextColor();
                    EditorGUILayout.LabelField("This Gameobject already has an inventory. Click Next if you want to add a new one.", customStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                }

                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;

                if (GUILayout.Button("Next!!", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    currentPanelIndex = 2;
                    OnGUI();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            if (inventoryInspectorTex != null)
            {
                Rect imageRect = GUILayoutUtility.GetLastRect();
                imageRect.x = position.width / 3 - 50;
                imageRect.y += 40;
                imageRect.width = position.width / 2;
                imageRect.height = position.height / 2 + 20;

                GUI.DrawTexture(imageRect, inventoryInspectorTex);
            }

            GUILayout.EndArea();
        }
        private void FixedSlotsBoolProperty()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Choose inventory settings.", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Fixed Slots : ",GetStyle("Body"));
            fixedSlots = EditorGUILayout.Toggle(fixedSlots, GUILayout.Width(40));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.normal.textColor = GetTextColor();
            EditorGUILayout.LabelField("Enabling this will make the slots undeletable.", customStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;

            if (GUILayout.Button("Next!!", GUILayout.Width(200), GUILayout.Height(20)))
            {
                currentPanelIndex = 3;
                OnGUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            DrawFixedSlotsImages(GUILayoutUtility.GetLastRect());

            GUILayout.EndArea();
        }
        private void MaximumInventorySlotsProperty()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Choose inventory settings.", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Maximum Inventory Slots : ",GetStyle("Body"));
            maxInventorySlots = EditorGUILayout.IntField(maxInventorySlots, GUILayout.Width(40));
            maxInventorySlots = Mathf.Abs(maxInventorySlots);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(30);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Slots will be created automatically at the start.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("If Fixed Slots is disabled, only slots containing items", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("will be created. You cannot exceed the ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("maximum slot count set in the inventory.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (maxInventorySlots > 0)
            {
                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;

                if (GUILayout.Button("Next!!", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    currentPanelIndex = 4;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


            }

            GUILayout.EndArea();
        }
        private void SavePathProperty()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Choose inventory settings.", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Save Path : ", GetStyle("Body"));
            savePath = EditorGUILayout.TextField(savePath,"", GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();            
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("File path where you want to save inventory data.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();            
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("It will be concated after the Application.PersistentPath.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();            
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Example save path : \"/PlayerInventory.save\"", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.normal.textColor = GetTextColor();
            EditorGUILayout.LabelField("You will have to call Save/Load functions manually. They are not called by default", customStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("You can save and load inventory data by calling", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();            
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("SaveInventory() and LoadInventory() in InventorySO script.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;

            if (GUILayout.Button("Create Inventory!!", GUILayout.Width(200), GUILayout.Height(20)))
            {
                inventoryCreated = CreateInventory();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            customStyle.normal.textColor = GetTextColor();
            EditorGUILayout.LabelField("You will be prompted to save the Inventory Scriptable Object when clicking this button.", customStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (inventoryCreated != null && inventoryCreated.Value)
            {
                currentPanelIndex = 5;
            }
            else if (inventoryCreated != null && !inventoryCreated.Value)
            {
                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                customStyle.normal.textColor = GetTextColor();
                EditorGUILayout.LabelField("Unable to create Inventory! Make sure that the specified path is valid and please try again", customStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }



            GUILayout.EndArea();
        }
        private void InventoryCreated()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Inventory Created!!!", GetStyle("Header1"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Do you want to setup the UI elements as well?", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Visit the GitHub page for detailed UI setup instructions.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;

            if (GUILayout.Button("Setup UI", GUILayout.Width(200), GUILayout.Height(20)))
            {
                InventoryUISetupWindow.inventory = inventory;
                InventoryUISetupWindow.objectToAttachInventory = objectToAttachInventory;
                InventoryUISetupWindow.ShowWindow();
                this.Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Open Item Creator", GUILayout.Width(200), GUILayout.Height(20)))
            {
                ItemCreatorWindow.ShowWindow();
                this.Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;

            if (GUILayout.Button("Close", GUILayout.Width(200), GUILayout.Height(20)))
            {
                this.Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.normal.textColor = GetTextColor();
            EditorGUILayout.LabelField("You can now go the gameObject inspector and add items to inventory slots.", customStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        private void DrawDatabaseInfo(Rect currentRect)
        {
            Rect imageRect = currentRect;
            Rect textRect = currentRect;
            if (databaseInfoTex != null)
            {
                imageRect.x = 20;
                imageRect.y = currentRect.y + 80;
                imageRect.width = position.width / 2 - 20;
                imageRect.height = position.height / 2 - 20;

                GUI.DrawTexture(imageRect, databaseInfoTex);

                textRect.x = imageRect.x + imageRect.width;
                textRect.y = imageRect.y + 150;
                textRect.width = position.width / 2 - 20;
                textRect.height = position.height / 2 - 20;
            }
            else
            {
                textRect.x = 0;
                textRect.y += 180;
                textRect.width = position.width;
                textRect.height = position.height / 2;
            }

            GUILayout.BeginArea(textRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("The database stores unique item IDs.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Each item must have a unique ID.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Always add new items to the database ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("after creating them.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("If you use the \"Item Creator Window\",", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("it will automatically add created items ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("to the database. ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        private void DrawFixedSlotsImages(Rect currentRect)
        {
            Rect leftImage = currentRect;
            Rect rightImage = currentRect;

            leftImage.width = position.width / 2 - 20;
            rightImage.width = position.width / 2 - 20;
            rightImage.x += leftImage.width + 15;

            if (fixedSlotsDisabledTex != null && fixedSlotsEnabledTex != null)
            {
                leftImage.x = 15;
                leftImage.y += 50;
                leftImage.height = position.height / 2 - 20;
                GUI.DrawTexture(leftImage, fixedSlotsDisabledTex);
                GUILayout.Space(leftImage.height + 20);

                rightImage = leftImage;
                rightImage.x += leftImage.width + 15;
                GUI.DrawTexture(rightImage, fixedSlotsEnabledTex);
                GUILayout.Space(leftImage.height + 20);
            }
            else
            {
                leftImage.y += 20;
                rightImage.y += 20;
            }

            Rect leftSideInfoRect = leftImage;
            leftSideInfoRect.y += leftImage.height + 130;
            leftSideInfoRect.height = 300;

            GUILayout.BeginArea(leftSideInfoRect);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Fixed Slots Disabled. ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Empty slots will be removed automatically. ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Additional slots will be added when needed.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Total number of slots can only be less than", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();            
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("or equal to maxNumberOfSlot property", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();

            Rect rightSideInfoRect = rightImage;
            rightSideInfoRect.y += rightImage.height + 130;
            rightSideInfoRect.height = 300;

            GUILayout.BeginArea(rightSideInfoRect);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Fixed Slots Enabled. ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Slot deletion not allowed. They will", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("be shown empty instead.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("No extra slots can be added at runtime.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        private void CreateDatabase()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create new database", "ItemDatabase", "asset", "Select the final name ");
            if (string.IsNullOrEmpty(path)) return;

            InventoryItemDatabase newDatabase = ScriptableObject.CreateInstance<InventoryItemDatabase>();
            AssetDatabase.CreateAsset(newDatabase, path);
            AssetDatabase.SaveAssets();

            database = newDatabase;
            OnGUI();
        }
        private bool CreateInventory()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Inventory Scriptable Object", "InventorySO", "asset", "Enter a name for the Inventory Scritpable Object");
            if (string.IsNullOrEmpty(path)) return false;

            InventorySO inventorySO = ScriptableObject.CreateInstance<InventorySO>();
            AssetDatabase.CreateAsset(inventorySO, path);
            AssetDatabase.SaveAssets();

            if (inventorySO == null) return false;

            inventorySO.fixedSlots = fixedSlots;
            inventorySO.maxNumberOfSlot = maxInventorySlots;
            inventorySO.database = database;
            inventorySO.savePath = savePath;

            inventory = objectToAttachInventory.AddComponent<Inventory>();
            inventory.inventorySO = inventorySO;

            if (inventory == null) return false;

            return true;
        }
        private GUIStyle GetStyle(string styleName)
        {
            if (skin == null || skin.GetStyle(styleName) == null)
            {
                GUI.skin.label.normal.textColor = GetTextColor();
                return GUI.skin.label;
            }

            skin.GetStyle(styleName).normal.textColor = GetTextColor();
            return skin.GetStyle(styleName);
        }
        private Color GetTextColor()
        {
            if (bg == null) return Color.white;

            return Color.black;
        }
    }

}