using InventorySystem;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace InventorySytem.Editors
{
    public class ChestInventoryCreatorWindow : EditorWindow
    {
        #region Variables
        Rect layoutRect;
        GUISkin skin;
        Texture2D bg;
        Color32 buttonBGColor = new Color32(151, 113, 74, 200);
        int currentPanelIndex = 0;
        int currentInventoryUIPropertyIndex = 0;

        Texture2D databaseInfoTex;
        Texture2D chestInventoryHeirarchyTex;
        Texture2D slotUIPrefabTex;
        Texture2D slotPrefabUIViewTex;
        Texture2D slotParentGridLayoutTex;
        Texture2D slotParentHeirarchyTex;
        Texture2D playerRangeCheckColliderTex;
        Texture2D playerInRangeScriptViewTex;

        InventoryItemDatabase database;
        GameObject chestGO;
        int maximumCapacity;
        string savePath;
        bool? inventoryCreated;
        Inventory inventory;

        GameObject canvasGO;
        GameObject inventoryUiBG;
        GameObject slotUIPrefab;
        Transform slotParent;
        PlayerRangeCheck playerRange;
        #endregion

        [MenuItem("Tools/Inventory/Chest Inventory Setup",false,5)]
        public static void ShowWindow()
        {
            ChestInventoryCreatorWindow window = GetWindow<ChestInventoryCreatorWindow>("Chest Inventory Creator");
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
                bg = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/InventoryWizardGradient.png", typeof(Texture2D));
            }

            if (skin == null)
            {
                skin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/EditorWindowSkin.guiskin", typeof(GUISkin));
            }

            if (databaseInfoTex == null)
            {
                databaseInfoTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/ItemDatabase.png", typeof(Texture2D));
            }

            if (chestInventoryHeirarchyTex == null)
            {
                chestInventoryHeirarchyTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/ChestInventoryHeirarchy.png", typeof(Texture2D));
            }


            if (slotUIPrefabTex == null)
            {
                slotUIPrefabTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/SlotUIPrefab.png", typeof(Texture2D));
            }

            if (slotPrefabUIViewTex == null)
            {
                slotPrefabUIViewTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/SlotUIPrefabView.png", typeof(Texture2D));
            }

            if (slotParentHeirarchyTex == null)
            {
                slotParentHeirarchyTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/SlotParentHeirarchy.png", typeof(Texture2D));
            }

            if (slotParentGridLayoutTex == null)
            {
                slotParentGridLayoutTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/SlotParentGridLayout.png", typeof(Texture2D));
            }

            if (playerRangeCheckColliderTex == null)
            {
                playerRangeCheckColliderTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/ChestPlayerRangeCheckCollider.png", typeof(Texture2D));
            }

            if (playerInRangeScriptViewTex == null)
            {
                playerInRangeScriptViewTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/PlayerRangeCheckScriptView.png", typeof(Texture2D));
            }
        }
        private void OnGUI()
        {
            layoutRect = new Rect(0, 30, position.width, position.height);
            DrawBG();

            switch (currentPanelIndex)
            {
                case 0:
                    DatabasePanel();
                    break;
                case 1:
                    ChestGOProperty();
                    break;
                case 2:
                    MaximumCapacityProperty();
                    break;
                case 3:
                    SavePathProperty();
                    break;
                case 4:
                    SetupUI();
                    break;
                case 5:
                    DrawUIProperties();
                    break;
                case 6:
                    DrawRangeCheckProperty();
                    break;
                case 7:
                    FinalWindow();
                    break;
            }
        }
        private void DrawBG()
        {
            if (bg == null) return;

            Rect bgRect = new Rect(0, 0, position.width, position.height);
            GUI.DrawTexture(bgRect, bg);
        }
        private void DatabasePanel()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Let's create a Chest Inventory!", GetStyle("Header1"));
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
        private void ChestGOProperty()
        {
            int nextPanelIndex = 2;

            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Drag the Chest Gameobject for which you want to setup", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("an inventory.", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            chestGO = (GameObject)EditorGUILayout.ObjectField(chestGO, typeof(GameObject), true, GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (chestGO != null)
            {
                bool inventoryFound = false;
                if (chestGO.TryGetComponent(out inventory))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
                    customStyle.normal.textColor = GetTextColor();
                    EditorGUILayout.LabelField("This Chest already has an inventory. Do you want to skip to the UI Setup?", customStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    nextPanelIndex = 5;
                    inventoryFound = true;
                }

                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;

                if (GUILayout.Button("Next!!", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    currentPanelIndex = nextPanelIndex;
                    OnGUI();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (inventoryFound)
                {
                    GUILayout.Space(15);

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUI.backgroundColor = buttonBGColor;

                    if (GUILayout.Button("Close", GUILayout.Width(200), GUILayout.Height(20)))
                    {
                        this.Close();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndArea();
        }
        private void MaximumCapacityProperty()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("How many items can this chest have at max?", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            maximumCapacity = EditorGUILayout.IntField(maximumCapacity, GUILayout.Width(200));
            maximumCapacity = Mathf.Abs(maximumCapacity);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (maximumCapacity > 0)
            {
                GUILayout.Space(20);

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

                GUILayout.Space(20);
            }

            
            GUILayout.EndArea();
        }
        private void SavePathProperty()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Save Path : ", GetStyle("Body"));
            savePath = EditorGUILayout.TextField(savePath, "", GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

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
            GUILayout.Label("Example save path : \"/ChestInventory.save\"", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            if (string.IsNullOrEmpty(savePath))
            {
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
                GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
                customStyle.normal.textColor = GetTextColor();
                EditorGUILayout.LabelField("You will be prompted to save the Inventory Scriptable Object when clicking this button.", customStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            if (inventoryCreated != null && inventoryCreated.Value)
            {
                currentPanelIndex = 4;
            }
            else if (inventoryCreated != null && !inventoryCreated.Value)
            {
                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
                customStyle.normal.textColor = GetTextColor();
                EditorGUILayout.LabelField("Unable to create Inventory! Make sure that the specified path is valid and please try again", customStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndArea();
        }
        private void SetupUI()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Inventory Created !!!", GetStyle("Header1"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Now let's setup the UI elements", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Setup UI!!", GUILayout.Width(200), GUILayout.Height(20)))
            {
                currentPanelIndex = 5;
                currentInventoryUIPropertyIndex = 0;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        private void DrawUIProperties()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Set Inventory UI settings.", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            switch (currentInventoryUIPropertyIndex)
            {
                case 0:
                    DrawCanvasReference();
                    break;
                case 1:
                    DrawInventoryUiBG();
                    break;
                case 2:
                    DrawSlotUIPrefabInfo();
                    break;
                case 3:
                    DrawSlotParentInfo();
                    break;
            }

            GUILayout.EndArea();
        }
        private void DrawCanvasReference()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Canvas : ", GetStyle("Body"));
            canvasGO = (GameObject)EditorGUILayout.ObjectField(canvasGO, typeof(GameObject), true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("The Canvas that contains the Inventory UI", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Required for displaying item images when dragging items.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (canvasGO != null)
            {
                if (canvasGO.TryGetComponent(out Canvas canvas))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUI.backgroundColor = buttonBGColor;
                    if (GUILayout.Button("Next", GUILayout.Width(200), GUILayout.Height(20)))
                    {
                        currentInventoryUIPropertyIndex = 1;
                        OnGUI();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace(); GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
                    customStyle.normal.textColor = GetTextColor();
                    EditorGUILayout.LabelField("No Canvas found in the selected GameObject. Ensure you’ve selected the correct object!", customStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

            }

        }
        private void DrawInventoryUiBG()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Inventory UiBG : ", GetStyle("Body"));
            inventoryUiBG = (GameObject)EditorGUILayout.ObjectField(inventoryUiBG, typeof(GameObject), true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("The GameObject containing the Inventory UI background.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Make sure that all Inventory UI elements are child of this gameobject.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("This object will be enabled/disabled to open or close the inventory.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.normal.textColor = GetTextColor();
            EditorGUILayout.LabelField("You can use the same UI Bg Gameobject for all the chests.", customStyle);
            //EditorGUILayout.HelpBox("You will be prompted to save the Inventory Scriptable Object when clicking this button.", MessageType.Info);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (chestInventoryHeirarchyTex != null)
            {
                Rect imageRect = new Rect(position.width / 2 - 150, 130, 300, chestInventoryHeirarchyTex.height - 40);
                GUI.DrawTexture(imageRect, chestInventoryHeirarchyTex);
                GUILayout.Space(chestInventoryHeirarchyTex.height - 20);
            }

            if (inventoryUiBG != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;
                if (GUILayout.Button("Next", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    currentInventoryUIPropertyIndex = 2;
                    OnGUI();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndArea();
        }
        private void DrawSlotUIPrefabInfo()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("SlotUI Prefab : ", GetStyle("Body"));
            slotUIPrefab = (GameObject)EditorGUILayout.ObjectField(slotUIPrefab, typeof(GameObject), true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("The prefab representing an inventory slot.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (slotPrefabUIViewTex != null)
            {
                float imageWidth = slotPrefabUIViewTex.width - 90;
                Rect rect = new Rect(position.width / 2 - imageWidth / 2, 105, imageWidth, slotPrefabUIViewTex.height - 20);

                GUI.DrawTexture(rect, slotPrefabUIViewTex);
                GUILayout.Space(slotPrefabUIViewTex.height);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Ensure this prefab has the InventorySlotUIReferenceContainer script", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("which holds references needed by the Dynamic UI system.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (slotUIPrefabTex != null)
            {
                Rect slotUIPrefabRect = GUILayoutUtility.GetLastRect();
                slotUIPrefabRect.y += 20;
                slotUIPrefabRect.width = position.width;
                slotUIPrefabRect.height = slotUIPrefabTex.height - 20;

                GUI.DrawTexture(slotUIPrefabRect, slotUIPrefabTex);
                GUILayout.Space(slotUIPrefabRect.height + 20);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Only assign references for UI elements you want to display", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (slotUIPrefab != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;

                if (GUILayout.Button("Next", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    currentInventoryUIPropertyIndex = 3;
                    OnGUI();

                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        private void DrawSlotParentInfo()
        {
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Slot Parent : ", GetStyle("Body"));
            slotParent = (Transform)EditorGUILayout.ObjectField(slotParent, typeof(Transform), true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            float yHeight = 130;
            Rect infoTextSideRect = new Rect(10, yHeight, position.width / 2, position.height / 3 - 30);
            Rect imageSideRect = new Rect(position.width / 2, yHeight, position.width / 2, position.height / 3);

            if (slotParentGridLayoutTex != null && slotParentHeirarchyTex != null)
            {
                GUILayout.BeginArea(imageSideRect);
                Rect slotParentHeirarchyRect = new Rect(10, 0, position.width / 2 - 20, slotParentHeirarchyTex.height);
                GUI.DrawTexture(slotParentHeirarchyRect, slotParentHeirarchyTex);

                Rect slotParentGridLayoutRect = slotParentHeirarchyRect;
                slotParentGridLayoutRect.y += slotParentHeirarchyTex.height + 25;
                GUI.DrawTexture(slotParentGridLayoutRect, slotParentGridLayoutTex);

                GUILayout.EndArea();
            }
            else
            {
                infoTextSideRect.width = position.width;
            }

            GUILayout.BeginArea(infoTextSideRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("The GameObject in the Canvas hierarchy", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" where slots will be instantiated", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(30);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Ensure it has a Layout Group", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("(Grid Layout, Horizontal Layout, etc.)", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("to position slots correctly. ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            GUILayout.Space(infoTextSideRect.height + 60);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" Test placing slots manually first to verify", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("they appear as expected when instantiated.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (slotParent != null)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;

                if (GUILayout.Button("Next!!", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    currentInventoryUIPropertyIndex = 4;
                    currentPanelIndex = 6;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        private void DrawRangeCheckProperty()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Set PlayerRangeCheck script reference : ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            playerRange = (PlayerRangeCheck)EditorGUILayout.ObjectField(playerRange, typeof(PlayerRangeCheck), true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            int imageStartingHeight = 110;

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("PlayerRangeCheck script checks player entry and exit.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Place a Collider near the chest, and set its 'isTrigger' on.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (playerRangeCheckColliderTex != null)
            {
                Rect imageRect = new Rect(position.width / 2 - 150, imageStartingHeight, 300, 200);
                GUI.DrawTexture(imageRect, playerRangeCheckColliderTex);
                GUILayout.Space(225);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Now assign the PlayerRangeCheck script to it.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (playerInRangeScriptViewTex != null)
            {
                imageStartingHeight += 245;
                Rect imageRect = new Rect(position.width / 2 - 180, imageStartingHeight, 360, 100);
                GUI.DrawTexture(imageRect, playerInRangeScriptViewTex);
                GUILayout.Space(130);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("The InteractionTag is the tag assigned to the Player.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (playerRange != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;
                if (GUILayout.Button("Finish!!", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    CreateChestInventoryControllerUI();
                    currentPanelIndex = 7;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndArea();
        }
        private void FinalWindow()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Chest Inventory Setup Successfull !!!", GetStyle("Header1"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("You can now store items in the chest, and they will remain where they are!", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" Visit the GitHub page to learn more about features and upcoming updates!", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Close Window", GUILayout.Width(200), GUILayout.Height(20)))
            {
                this.Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        private bool CreateInventory()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Inventory Scriptable Object", "InventorySO", "asset", "Enter a name for the Inventory Scritpable Object");
            if (string.IsNullOrEmpty(path)) return false;

            InventorySO inventorySO = ScriptableObject.CreateInstance<InventorySO>();
            AssetDatabase.CreateAsset(inventorySO, path);
            AssetDatabase.SaveAssets();

            if (inventorySO == null) return false;

            inventorySO.fixedSlots = true;
            inventorySO.maxNumberOfSlot = maximumCapacity;
            inventorySO.database = database;
            inventorySO.savePath = savePath;
            inventory = chestGO.AddComponent<Inventory>();
            inventory.inventorySO = inventorySO;

            if (inventory == null) return false;

            return true;
        }
        private void CreateChestInventoryControllerUI()
        {
            ChestInventoryUIController chestInventoryUIController = chestGO.AddComponent<ChestInventoryUIController>();
            chestInventoryUIController.Initialize(canvasGO, inventoryUiBG, inventory, false, null, Vector3.zero, false, slotParent, slotUIPrefab, playerRange);
            inventory.inventoryUI=chestInventoryUIController;
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
