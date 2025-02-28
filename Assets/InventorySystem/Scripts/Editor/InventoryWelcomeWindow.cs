using InventorySystem.Editors;
using UnityEditor;
using UnityEngine;

namespace InventorySytem.Editors
{
    [InitializeOnLoad]
    public class InventoryWelcomeWindow : EditorWindow
    {
        Rect layoutRect;
        GUISkin skin;
        Texture2D bg;
        Color32 buttonBGColor = new Color32(151, 113, 74, 200);

        private const string PrefKey = "InventorySystem_WelcomeShown";

        static InventoryWelcomeWindow()
        {
            if(!EditorPrefs.GetBool(PrefKey, false))
            {
                EditorPrefs.SetBool(PrefKey, true);
                EditorApplication.delayCall += ShowWindow;
            }
        }
        public static void ShowWindow()
        {
            InventoryWelcomeWindow window = GetWindow<InventoryWelcomeWindow>(true, "Welcome to Inventory System", true);
            window.minSize = new Vector2(500, 300);
            window.maxSize = new Vector2(500, 300);
        }
        private void OnEnable()
        {
            LoadAssets();
        }
        private void LoadAssets()
        {
            if (bg == null)
            {
                bg = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/Icons/InventoryWizardGradient.png", typeof(Texture2D));
            }
            if (skin == null)
            {
                skin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/InventorySystem/EditorData/InventoryWizardSkin.guiskin", typeof(GUISkin));
            }
        }
        private void OnGUI()
        {
            layoutRect = new Rect(0, 30, position.width, position.height);
            DrawBG();
            ShowInfo();
        }
        private void ShowInfo()
        {
            GUILayout.BeginArea(layoutRect);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Thanks for downloading the Inventory System!", GetStyle("Header2"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Here are some tools to get you started", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Inventory Creator", GUILayout.Width(200), GUILayout.Height(20)))
            {
                InventoryCreatorWindow.ShowWindow();
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Item Creator", GUILayout.Width(200), GUILayout.Height(20)))
            {
                ItemCreatorWindow.ShowWindow();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = buttonBGColor;
            if (GUILayout.Button("Merchant Inventory Creator", GUILayout.Width(200), GUILayout.Height(20)))
            {
                MerchantInventoryCreatorWindow.ShowWindow();
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Chest Inventory Creator", GUILayout.Width(200), GUILayout.Height(20)))
            {
                ChestInventoryCreatorWindow.ShowWindow();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("For any queries or suggestions, please visit the Github page.", GetStyle("Body"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

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
        private void DrawBG()
        {
            if (bg == null) return;

            Rect bgRect = new Rect(0, 0, position.width, position.height);
            GUI.DrawTexture(bgRect, bg);
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
