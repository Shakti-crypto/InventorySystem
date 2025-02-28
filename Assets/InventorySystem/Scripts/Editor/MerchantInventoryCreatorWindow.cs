using InventorySystem;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace InventorySytem.Editors
{
    public class MerchantInventoryCreatorWindow : EditorWindow
    {
        #region Variables
        Rect layoutRect;
        GUISkin skin;
        Texture2D bg;
        Color32 buttonBGColor = new Color32(151, 113, 74, 200);
        int currentPanelIndex = 0;
        int currentInventoryUIPropertyIndex = 0;

        Texture2D databaseInfoTex;
        Texture2D merchantInventoryHeirarchyTex;
        Texture2D merchantPurseTex;
        Texture2D slotUIPrefabTex;
        Texture2D slotPrefabUIViewTex;
        Texture2D slotParentGridLayoutTex;
        Texture2D slotParentHeirarchyTex;
        Texture2D merchantInventoryCoinTex;
        Texture2D playerRangeCheckColliderTex;
        Texture2D playerInRangeScriptViewTex;

        InventoryItemDatabase database;
        GameObject merchantGO;
        int maximumCapacity;
        bool? inventoryCreated;
        Inventory inventory;

        PurseInventory purse;
        string purseItemID;
        int purseItemAmount;

        GameObject canvasGO;
        GameObject inventoryUiBG;
        GameObject slotUIPrefab;
        Transform slotParent;
        TextMeshProUGUI coinAmountText;
        PlayerRangeCheck playerRange;
        List<InventoryItemTypeSO> itemTypesAccepted = new List<InventoryItemTypeSO>();
        List<InventoryItemTypeSO> itemTypes;
        string[] itemTypeNames;
        List<int> selectedItemTypeIndex=new List<int>();
        Vector2 itemTypesScrollPos;
        #endregion

        [MenuItem("Tools/Inventory/Merchant Inventory Setup",false,4)]
        public static void ShowWindow()
        {
            MerchantInventoryCreatorWindow window = GetWindow<MerchantInventoryCreatorWindow>("Merchant Inventory Creator");
            window.minSize = new Vector2(600, 600);
            window.maxSize = new Vector2(600, 600);
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
                databaseInfoTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/ItemDatabase.png",typeof(Texture2D));
            }

            if (merchantInventoryHeirarchyTex == null)
            {
                merchantInventoryHeirarchyTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/MerchantInventoryHeirarchy.png", typeof(Texture2D));
            }

            if (merchantPurseTex == null)
            {
                merchantPurseTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/MerchantPurse.png", typeof(Texture2D));
            }

            if (slotUIPrefabTex == null)
            {
                slotUIPrefabTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/MerchantSlotUIPrefab.png", typeof(Texture2D));
            }

            if (slotPrefabUIViewTex == null)
            {
                slotPrefabUIViewTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/MerchantSlotUIPrefabView.png", typeof(Texture2D));
            }

            if (slotParentHeirarchyTex == null)
            {
                slotParentHeirarchyTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/SlotParentHeirarchy.png",typeof(Texture2D));
            }

            if (slotParentGridLayoutTex == null)
            {
                slotParentGridLayoutTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/SlotParentGridLayout.png",typeof(Texture2D));
            }

            if (merchantInventoryCoinTex == null)
            {
                merchantInventoryCoinTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/MerchantInvetoryCointText.png",typeof(Texture2D));
            }

            if (playerRangeCheckColliderTex == null)
            {
                playerRangeCheckColliderTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/PlayerRangeCheckCollider.png",typeof(Texture2D));
            }

            if (playerInRangeScriptViewTex == null)
            {
                playerInRangeScriptViewTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/PlayerRangeCheckScriptView.png",typeof(Texture2D));
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
                    MerchantGOProperty();
                    break;
                case 2:
                    MaximumCapacityProperty();
                    break;
                case 3:
                    PurseProperty();
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
                    DrawItemTypesAccepted();
                    break;
                case 8:
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
            GUILayout.Label("Let's create a Merchant's Inventory!", GetStyle("Header1"));
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
        private void MerchantGOProperty()
        {
            int nextPanelIndex = 2;

            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Drag the Merchant Gameobject for which you want to setup", GetStyle("Header2"));
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
            merchantGO = (GameObject)EditorGUILayout.ObjectField(merchantGO, typeof(GameObject), true, GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (merchantGO != null)
            {
                if (merchantGO.TryGetComponent(out inventory))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
                    customStyle.normal.textColor = GetTextColor();
                    EditorGUILayout.LabelField("This Merchant already has an inventory. Do you want to skip to the next part?", customStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    nextPanelIndex = 3;
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
            }

            GUILayout.EndArea();
        }
        private void MaximumCapacityProperty()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("How many items can this merchant have at max?", GetStyle("Header2"));
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
                currentPanelIndex = 3;
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
        private void PurseProperty()
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
            GUILayout.Label("Now let's add a Purse to our merchant.", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("A purse is a special type of inventory which can hold only one item at a time.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (merchantPurseTex != null)
            {
                Rect imageRect = new Rect(position.width / 2 - 150, 110, 330, merchantPurseTex.height-40);
                GUI.DrawTexture(imageRect, merchantPurseTex);
                GUILayout.Space(merchantPurseTex.height-10);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("To enable trading, both the Player and Merchant must have a Purse", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();            
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("containing the correct Money Item (e.g., a coin).", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.normal.textColor = GetTextColor();
            EditorGUILayout.LabelField("Money can be any type of item, but its reference must be set in the TradingUIManager script", customStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Enter the itemID for the money to add in the purse.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            purseItemID = EditorGUILayout.TextField(purseItemID,GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Enter the amount of money : ", GetStyle("Body"));
            purseItemAmount = EditorGUILayout.IntField(purseItemAmount,GUILayout.Width(60));
            purseItemAmount=Mathf.Abs(purseItemAmount);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            customStyle.normal.textColor = GetTextColor();
            EditorGUILayout.LabelField("You can fill these from the inspector later, if you haven't created any Money Item yet.", customStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Create Purse!!", GUILayout.Width(200), GUILayout.Height(20)))
            {
                CreatePurse();
                currentPanelIndex = 4;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        private void SetupUI()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Purse Created !!!", GetStyle("Header1"));
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
                case 4:
                    DrawCoinAmountText();
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
            EditorGUILayout.LabelField("You can use the same UI Bg Gameobject for all the merchants.", customStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (merchantInventoryHeirarchyTex != null)
            {
                Rect imageRect = new Rect(position.width / 2 - 150, 130, 300, merchantInventoryHeirarchyTex.height - 40);
                GUI.DrawTexture(imageRect, merchantInventoryHeirarchyTex);
                GUILayout.Space(merchantInventoryHeirarchyTex.height - 20);
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
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        private void DrawCoinAmountText()
        {
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("TextMeshPro text for Coin Amount : ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();            
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            coinAmountText = (TextMeshProUGUI)EditorGUILayout.ObjectField(coinAmountText, typeof(TextMeshProUGUI), true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.normal.textColor = GetTextColor();
            EditorGUILayout.LabelField("You can leave it empty, if you don't want to display Merchant Coins.", customStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;

            if (GUILayout.Button("Next!!", GUILayout.Width(200), GUILayout.Height(20)))
            {
                currentInventoryUIPropertyIndex = 5;
                currentPanelIndex = 6;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (merchantInventoryCoinTex != null)
            {
                Rect imageRect = new Rect(position.width / 2-175, 160, 350, 360);
                GUI.DrawTexture(imageRect, merchantInventoryCoinTex);
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
            if (playerRange != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;
                if (GUILayout.Button("Next!!", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    currentPanelIndex = 7;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                imageStartingHeight += 10;
            }

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("PlayerRangeCheck script checks player entry and exit.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();            
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Place a Collider near the merchant, and set its 'isTrigger' on.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if(playerRangeCheckColliderTex != null)
            {
                Rect imageRect = new Rect(position.width / 2 - 150,imageStartingHeight , 300, 225);
                GUI.DrawTexture(imageRect,playerRangeCheckColliderTex);
                GUILayout.Space(245);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Now assign the PlayerRangeCheck script to it.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (playerInRangeScriptViewTex != null)
            {
                imageStartingHeight += 265;
                Rect imageRect = new Rect(position.width / 2 - 180, imageStartingHeight, 360, 100);
                GUI.DrawTexture(imageRect, playerInRangeScriptViewTex);
                GUILayout.Space(120);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("The InteractionTag is the tag assigned to the Player.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        private void DrawItemTypesAccepted()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("We are almost there! Just one more thing to set up.", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Define the Item Types that this merchant can trade in", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            Color defaultColor = GUI.backgroundColor;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Refresh Item Type", GUILayout.Width(200), GUILayout.Height(20)))
            {
                SearchForItemTypes();
            }

            if (GUILayout.Button("Create new Item Type", GUILayout.Width(200), GUILayout.Height(20)))
            {
                CreateNewItemType();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.backgroundColor = defaultColor;
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            itemTypesScrollPos = EditorGUILayout.BeginScrollView(itemTypesScrollPos, GUILayout.Width(400), GUILayout.Height(320));
            for(int i=0;i<itemTypesAccepted.Count;i++)
            {
                DrawIndividualItemType(i);
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Add Item Type", GUILayout.Width(200), GUILayout.Height(20)))
            {
                itemTypesAccepted.Add(null);
                selectedItemTypeIndex.Add(0);
            }

            if (itemTypesAccepted.Count > 0)
            {
                if (GUILayout.Button("Remove Last Item Type", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    itemTypesAccepted.RemoveAt(itemTypesAccepted.Count - 1);
                    selectedItemTypeIndex.RemoveAt(selectedItemTypeIndex.Count - 1);
                }

            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (itemTypesAccepted.Count <= 0)
            {
                GUILayout.EndArea();
                return;
            }

            bool nullGameobjectFound = false;

            for (int i = 0; i < itemTypesAccepted.Count; i++)
            {
                nullGameobjectFound = nullGameobjectFound || itemTypesAccepted[i] == null;
            }

            if (nullGameobjectFound)
            {
                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
                customStyle.normal.textColor = GetTextColor();
                EditorGUILayout.LabelField("Null Item Type present in the array. Please set the references properly.", customStyle);
                //EditorGUILayout.HelpBox("Null Gameobject present in the slots array. Please set the references properly.", MessageType.Warning);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                return;
            }

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Finish!!", GUILayout.Width(200), GUILayout.Height(20)))
            {
                CreateMerchantInventoryUIController();
                currentPanelIndex = 8;
                OnGUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        private void FinalWindow()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Merchant Inventory Setup Successfull !!!", GetStyle("Header1"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Go into the Merchant Inventory Inspector to assign items to sell.", GetStyle("Body"));
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
        private void DrawIndividualItemType(int itemTypeIndex)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            itemTypesAccepted[itemTypeIndex] = (InventoryItemTypeSO)EditorGUILayout.ObjectField(itemTypesAccepted[itemTypeIndex], typeof(InventoryItemTypeSO), true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            InventoryItemTypeSO currentItemType = itemTypesAccepted[itemTypeIndex];
            int currentItemTypeIndex = 0;
            if (currentItemType != null)
            {
                currentItemTypeIndex = GeItemTypeIndex(currentItemType.name);
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int newSlotTypeIndex = EditorGUILayout.Popup(currentItemTypeIndex, itemTypeNames, GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (newSlotTypeIndex != currentItemTypeIndex)
            {
                currentItemTypeIndex = newSlotTypeIndex;

                if (currentItemTypeIndex == 0)
                {
                    itemTypesAccepted[itemTypeIndex] = null;
                }
                else
                {
                    itemTypesAccepted[itemTypeIndex] = itemTypes[currentItemTypeIndex-1];
                }
            }
        }
        private int GeItemTypeIndex(string name)
        {
            for (int i = 0; i < itemTypeNames.Length; i++)
            {
                if (name == itemTypeNames[i])
                {
                    return i;
                }
            }

            return 0;
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
            itemTypeNames[0] = "Null Reference";

            for (int i = 1; i <= itemTypes.Count; i++)
            {
                itemTypeNames[i] = itemTypes[i - 1].name;
            }
        }
        private bool CreateInventory()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Inventory Scriptable Object", "InventorySO", "asset", "Enter a name for the Inventory Scritpable Object");
            if (string.IsNullOrEmpty(path)) return false;

            InventorySO inventorySO = ScriptableObject.CreateInstance<InventorySO>();
            AssetDatabase.CreateAsset(inventorySO, path);
            AssetDatabase.SaveAssets();

            if (inventorySO == null) return false;

            inventorySO.fixedSlots = false;
            inventorySO.maxNumberOfSlot = maximumCapacity;
            inventorySO.database = database;

            inventory = merchantGO.AddComponent<Inventory>();
            inventory.inventorySO = inventorySO;

            if (inventory == null) return false;

            return true;
        }
        private void CreatePurse()
        {
            purse = merchantGO.AddComponent<PurseInventory>();
            purse.Initialze(purseItemID, purseItemAmount);
        }
        private void CreateNewItemType()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create a new item type", "ItemType", "asset", "Enter a name for the item type");
            if (string.IsNullOrEmpty(path)) return;

            InventoryItemTypeSO newItemType = ScriptableObject.CreateInstance<InventoryItemTypeSO>();
            AssetDatabase.CreateAsset(newItemType, path);
            AssetDatabase.SaveAssets();

            SearchForItemTypes();
        }
        private void CreateMerchantInventoryUIController()
        {
            MerchantInventoryUIController merchantInventoryUIController = merchantGO.AddComponent<MerchantInventoryUIController>();

            merchantInventoryUIController.Initialize(canvasGO, inventoryUiBG, inventory, false, null, Vector3.zero, false, slotParent, slotUIPrefab, purse, coinAmountText, playerRange, itemTypesAccepted.ToArray());
            inventory.inventoryUI = merchantInventoryUIController;
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
