using InventorySystem;
using InventorySystem.Editors;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InventorySytem.Editors
{
    public class InventoryUISetupWindow : EditorWindow
    {
        #region Variables
        Texture2D dynamicUITex;
        Texture2D staticUITex;
        Texture2D slotUIPrefabTex;
        Texture2D slotParentHeirarchyTex;
        Texture2D slotParentGridLayoutTex;
        Texture2D merchantInventoryHeirarchyTex;
        Texture2D mainCanvasHeirarchyTex;
        Texture2D slotPrefabUIViewTex;
        Texture2D equipmentInventoryGameViewTex;
        Texture2D equipmentInventoryHeirarViewTex;


        Rect layoutRect;
        GUISkin skin;
        Texture2D bg;
        Color32 buttonBGColor = new Color32(151, 113, 74, 200);
        Vector2 staticSlotPrefabScrollRect;

        private GameObject inventoryUiBG;
        private GameObject canvasGO;
        private bool allowDropItems;
        private Transform itemDropPoint;
        private Vector3 itemDropPointOfsset;
        private bool visibleAtStart;
        private GameObject slotUIPrefab;
        private Transform slotParent;
        private List<GameObject> slots = new List<GameObject>();

        int currentPanelIndex = 0;
        private int currentInventoryUIPropertyIndex = 0;
        public static GameObject objectToAttachInventory;
        public static Inventory inventory;
        #endregion

        [MenuItem("Tools/Inventory/Inventory UI Setup",false,3)]
        public static void ShowWindow()
        {
            InventoryUISetupWindow window = GetWindow<InventoryUISetupWindow>("Inventory UI Editor");
            window.minSize = new Vector2(600, 600);
            window.maxSize = new Vector2(600, 600);
        }
        private void OnEnable()
        {
            LoadAssets();
        }
        private void LoadAssets()
        {
            if (bg == null)
            {
                bg = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/InventoryWizardGradient.png",typeof(Texture2D));
            }

            if (skin == null)
            {
                skin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/EditorWindowSkin.guiskin", typeof(GUISkin));
            }

            if (dynamicUITex == null)
            {
                dynamicUITex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/DynamicUIExample.png",typeof(Texture2D));
            }

            if (staticUITex == null)
            {
                staticUITex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/StaticUIExample.png",typeof(Texture2D));
            }

            if (slotUIPrefabTex == null)
            {
                slotUIPrefabTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/SlotUIPrefab.png",typeof(Texture2D));
            }

            if (slotParentHeirarchyTex == null)
            {
                slotParentHeirarchyTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/SlotParentHeirarchy.png",typeof(Texture2D));
            }

            if (slotParentGridLayoutTex == null)
            {
                slotParentGridLayoutTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/SlotParentGridLayout.png",typeof(Texture2D));
            }

            if (merchantInventoryHeirarchyTex == null)
            {
                merchantInventoryHeirarchyTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/MerchantInventoryHeirarchy.png",typeof(Texture2D));
            }

            if (mainCanvasHeirarchyTex == null)
            {
                mainCanvasHeirarchyTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/MainCanvasHeirarchy.png",typeof(Texture2D));
            }

            if (slotPrefabUIViewTex == null)
            {
                slotPrefabUIViewTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/SlotUIPrefabView.png",typeof(Texture2D));
            }

            if (equipmentInventoryGameViewTex == null)
            {
                equipmentInventoryGameViewTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/PlayerEquipmentInventoryGameView.png",typeof(Texture2D));
            }

            if (equipmentInventoryHeirarViewTex == null)
            {
                equipmentInventoryHeirarViewTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/PlayerEquipmentInventoryHeirarchyView.png",typeof(Texture2D));
            }
        }
        private void OnGUI()
        {
            layoutRect = new Rect(0, 30, position.width, position.height);
            DrawBG();
            switch (currentPanelIndex)
            {
                case 0:
                    UISetup();
                    break;
                case 1:
                    DynamicPanel();
                    break;
                case 2:
                    StaticPanel();
                    break;
                case 3:
                    FinalPanel();
                    break;
            }
        }
        private void DrawBG()
        {
            if (bg == null) return;

            Rect bgRect = new Rect(0, 0, position.width, position.height);
            GUI.DrawTexture(bgRect, bg);
        }
        private void UISetup()
        { 
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("This Inventory System supports two types of UI setups.",GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            float yHeightForButtons =240 ;
            if (DrawTwoTypesOfUiSetups(GUILayoutUtility.GetLastRect()))
            {
                yHeightForButtons = position.height - 160;
            }

            Rect buttonRects = new Rect(0, yHeightForButtons, position.width, 200);

            GUILayout.BeginArea(buttonRects);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Go DYNAMIC!!!", GUILayout.Width(200), GUILayout.Height(35)))
            {
                currentPanelIndex = 1;
                OnGUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Go STATIC!!!", GUILayout.Width(200), GUILayout.Height(35)))
            {
                currentPanelIndex = 2;
                OnGUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.normal.textColor = GetTextColor();
            EditorGUILayout.LabelField("These are just ways of representing inventory information. They will have no effect on the actual inventroy system.", customStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
            GUILayout.EndArea();
        }
        private void DynamicPanel()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Dynamic UI", GetStyle("Header1"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Choose Inventory UI settings.",GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            DrawUIProperties();

            GUILayout.EndArea();
        }
        private void StaticPanel()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Static UI",GetStyle("Header1"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Choose Inventory UI settings.", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            DrawUIProperties();

            GUILayout.EndArea();
        }
        private bool DrawTwoTypesOfUiSetups(Rect currentRect)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Dynamic Inventory UI", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Static Inventory UI", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            Rect dynamciUIImage = currentRect;
            Rect dynamicInfoRect = currentRect;

            bool imagesDrawn = false;

            if (dynamicUITex != null && staticUITex != null)
            {
                dynamciUIImage.x = 15;
                dynamciUIImage.y += 60;
                dynamciUIImage.width = position.width / 2 - 20;
                dynamciUIImage.height = position.height / 3 - 50;

                Rect staticUIImage = dynamciUIImage;
                staticUIImage.x= position.width/2+15;
                staticUIImage.width = position.width / 2 - 20;
                staticUIImage.height = position.height / 3;

                GUI.DrawTexture(dynamciUIImage, dynamicUITex);
                GUILayout.Space(dynamciUIImage.height + 20);

                GUI.DrawTexture(staticUIImage, staticUITex);
                GUILayout.Space(staticUIImage.height + 20);

                imagesDrawn = true;
                dynamicInfoRect.y += staticUIImage.height + 40;
            }

            dynamicInfoRect.x = 15;
            dynamicInfoRect.y += 50;
            dynamicInfoRect.height = position.height - 200;
            dynamicInfoRect.width = position.width / 2 - 20;

            GUILayout.BeginArea(dynamicInfoRect);
            GUILayout.BeginVertical();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Slot UIs are generated at runtime.",GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Recommended for layouts that can be", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("managed using Canvas Layout components", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("like Grid Layout Group, Horizontal Layout,", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Group, Vertical Layout Group, etc.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();

            Rect staticInfoRect = dynamicInfoRect;
            staticInfoRect.x = position.width / 2 + 15;

            GUILayout.BeginArea(staticInfoRect);

            GUILayout.BeginVertical();
            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Slot UIs need to be manually placed in the", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("hierarchy. Recommended if you need full ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" control over slot positions ensuring the", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("slot count matches the inventory size.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("You must manually assign slots to the script", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ensuring the slot count matches the", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();            
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("the inventory size.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();

            return imagesDrawn;

        }
        private void DrawUIProperties()
        {
            switch (currentInventoryUIPropertyIndex)
            {
                case 0:
                    DrawInventoryScriptReference();
                    break;
                case 1:
                    DrawCanvasReference();
                    break;
                case 2:
                    DrawInventoryUiBG();
                    break;
                case 3:
                    DrawAllowDropItemsProperty();
                    break;
                case 4:
                    DrawVisibleAtStart();
                    break;
                case 5:
                    if (currentPanelIndex == 1)
                    {
                        DrawSlotUIPrefabInfo();
                    }
                    else if (currentPanelIndex == 2)
                    {
                        DrawStaticSlotUIInfo();
                    }
                    break;
                case 6:
                    if (currentPanelIndex == 1)
                    {
                        DrawSlotParentInfo();
                    }
                    else if (currentPanelIndex == 2)
                    {
                        DrawSlotsArrayForStaticUI();
                    }

                    break;
            }
        }
        private void DrawInventoryScriptReference()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Inventory Script Reference : ", GetStyle("Body"));
            inventory = (Inventory)EditorGUILayout.ObjectField(inventory, typeof(Inventory), true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Select the inventory this UI will be linked to.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (inventory != null)
            {
                objectToAttachInventory = inventory.gameObject;
                GUILayout.Space(10);

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
                        currentInventoryUIPropertyIndex = 2;
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

            if (merchantInventoryHeirarchyTex != null)
            {
                Rect imageRect = new Rect(position.width / 2 - 150, 180, 300, merchantInventoryHeirarchyTex.height - 40);
                GUI.DrawTexture(imageRect, merchantInventoryHeirarchyTex);
                GUILayout.Space(merchantInventoryHeirarchyTex.height - 20);
            }

            if (inventoryUiBG != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor= buttonBGColor;
                if (GUILayout.Button("Next", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    currentInventoryUIPropertyIndex = 3;
                    OnGUI();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        private void DrawAllowDropItemsProperty()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Allow Drop Items: ",GetStyle("Body"));
            allowDropItems = EditorGUILayout.Toggle(allowDropItems, GUILayout.Width(30));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("If enabled, items can be removed by dragging them out of the inventory.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            bool nextButtonAllowed;
            if (allowDropItems)
            {
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Item Drop Point : ", GetStyle("Body"));
                itemDropPoint = (Transform)EditorGUILayout.ObjectField(itemDropPoint, typeof(Transform), true);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (itemDropPoint == null)
                {
                    nextButtonAllowed = false;
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace(); 
                    GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
                    customStyle.normal.textColor = GetTextColor();
                    EditorGUILayout.LabelField("Item Drop Point is required when AllowDropItems is set to true.", customStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else
                {
                    nextButtonAllowed = true;
                }

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Specify the Transform where dropped items should appear.", GetStyle("Body"));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Example: The player's Transform for a player inventory", GetStyle("Body"));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("or a point above a chest for a chest inventory.", GetStyle("Body"));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Item Drop Point : ", GetStyle("Body"));
                itemDropPointOfsset = EditorGUILayout.Vector3Field(GUIContent.none, itemDropPointOfsset);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Offset to adjust the drop position without needing extra empty GameObjects.", GetStyle("Body"));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                nextButtonAllowed = true;
            }

            if (nextButtonAllowed)
            {
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = buttonBGColor;
                if (GUILayout.Button("Next", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    currentInventoryUIPropertyIndex = 4;
                    OnGUI();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

        }
        private void DrawVisibleAtStart()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Visible At Start : ", GetStyle("Body"));
            visibleAtStart = EditorGUILayout.Toggle(visibleAtStart, GUILayout.Width(30));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("If enabled, the inventory will be visible when the game starts.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("If disabled, you must open it manually through a script.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Next", GUILayout.Width(200), GUILayout.Height(20)))
            {
                currentInventoryUIPropertyIndex = 5;
                OnGUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
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

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("The prefab representing an inventory slot.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (slotPrefabUIViewTex != null)
            {
                float imageWidth = slotPrefabUIViewTex.width - 90;
                Rect rect = new Rect(position.width / 2 - imageWidth / 2, 130, imageWidth, slotPrefabUIViewTex.height - 20);

                GUI.DrawTexture(rect, slotPrefabUIViewTex);
                GUILayout.Space(140);
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
                    currentInventoryUIPropertyIndex = 6;
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

            GUILayout.Space(infoTextSideRect.height+20);

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

                if (GUILayout.Button("Create Inventory UI !!!", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    CreateDynamicInventoryUI();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        private void DrawStaticSlotUIInfo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Now, place the slot UIs in the hierarchy", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (equipmentInventoryGameViewTex != null && equipmentInventoryHeirarViewTex != null)
            {
                Rect gameViewRect = new Rect(-40, 0, 200, position.height / 2 - 20);
                GUI.DrawTexture(gameViewRect, equipmentInventoryGameViewTex);

                Rect heirarchyViewRect = new Rect(220, 140, equipmentInventoryHeirarViewTex.width - 200, equipmentInventoryHeirarViewTex.height - 30);
                GUI.DrawTexture(heirarchyViewRect, equipmentInventoryHeirarViewTex);

                GUILayout.Space(equipmentInventoryHeirarViewTex.height + 60);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Attach the InventorySlotUIReferenceContainer", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("script to each slot and assign necessary references.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

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
            GUILayout.Label("Only set references for elements you want to display.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;

            if (GUILayout.Button("Next", GUILayout.Width(200), GUILayout.Height(20)))
            {
                currentInventoryUIPropertyIndex = 6;
                OnGUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); 
            GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
            customStyle.normal.textColor = GetTextColor();
            EditorGUILayout.LabelField("Ensure slots are correctly positioned before proceeding..", customStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void DrawSlotsArrayForStaticUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Set all the slots UI gameobject : ", GetStyle("Body"));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Slot Gameobjects : ", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            staticSlotPrefabScrollRect = EditorGUILayout.BeginScrollView(staticSlotPrefabScrollRect, GUILayout.Width(400), GUILayout.Height(250));
            for (int i = 0; i < slots.Count; i++)
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                slots[i] = (GameObject)EditorGUILayout.ObjectField(slots[i], typeof(GameObject), true, GUILayout.Width(200));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Add Slot", GUILayout.Width(200), GUILayout.Height(20)))
            {
                slots.Add(null);
            }

            if (slots.Count > 0)
            {
                if (GUILayout.Button("Remove Last Slot", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    slots.RemoveAt(slots.Count - 1);
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (slots.Count <= 0)
            {
                return;
            }

            bool nullGameobjectFound = false;

            for (int i = 0; i < slots.Count; i++)
            {
                nullGameobjectFound = nullGameobjectFound || slots[i] == null;
            }

            if (nullGameobjectFound)
            {
                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle customStyle = new GUIStyle(EditorStyles.helpBox);
                customStyle.normal.textColor = GetTextColor();
                EditorGUILayout.LabelField("Null Gameobject present in the slots array. Please set the references properly.", customStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Create Inventory UI!!!", GUILayout.Width(200), GUILayout.Height(20)))
            {
                CreateStaticInventoryUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void CreateDynamicInventoryUI()
        {
            DynamicInventoryUI dynamicInventory = objectToAttachInventory.AddComponent<DynamicInventoryUI>();
            dynamicInventory.Initialize(canvasGO, inventoryUiBG, inventory, allowDropItems, itemDropPoint, itemDropPointOfsset, visibleAtStart, slotParent, slotUIPrefab);
            inventory.inventoryUI = dynamicInventory;
            currentPanelIndex = 3;
            OnGUI();
        }
        private void CreateStaticInventoryUI()
        {
            StaticInventoryUI staticInventoryUI = objectToAttachInventory.AddComponent<StaticInventoryUI>();
            staticInventoryUI.Initialize(canvasGO, inventoryUiBG, inventory, allowDropItems, itemDropPoint, itemDropPointOfsset, visibleAtStart, slots.ToArray());
            inventory.inventoryUI = staticInventoryUI;
            currentPanelIndex = 3;
            OnGUI();
        }
        private void FinalPanel()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Inventory UI setup successfull !!!", GetStyle("Header1"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" Visit the GitHub page to learn more about features and upcoming updates!", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Open Item Creator Window", GUILayout.Width(200), GUILayout.Height(20)))
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

            GUILayout.EndArea();
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
