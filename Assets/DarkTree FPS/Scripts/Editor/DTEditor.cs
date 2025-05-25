using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace DarkTreeFPS
{
    public class DTEditor : EditorWindow
    {
        private int windowID = 0;

        [MenuItem("DarkTree FPS/Game editor")]
        static void Init()
        {
            DTEditor _editor = (DTEditor)GetWindow(typeof(DTEditor));

            _editor.Show();
            _editor.maxSize = new Vector2(450, 500);
            _editor.minSize = _editor.maxSize;
        }

        private void OnGUI()
        {
            switch(windowID)
            {
                case 0: DrawDefaultMenu();
                    break;
                case 1:
                    DrawPlayerStats();
                    break;
            }
        }

        void DrawPlayerStats()
        {
            PlayerStats stats = FindObjectOfType<PlayerStats>();

            Editor editor = Editor.CreateEditor(stats);
            editor.DrawDefaultInspector();

            if(GUILayout.Button("Back"))
            {
                windowID = 0;
            }
        }

        private void DrawDefaultMenu()
        {
            Texture labelImage = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DarkTree FPS/Scripts/Editor/EditorImages/product_banner.png", typeof(Texture));
            GameObject gamePrefabSource = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/DarkTree FPS/Prefabs/GamePrefab.prefab", typeof(GameObject));

            GUILayout.BeginVertical("HelpBox");
            GUILayout.Label(labelImage);
            GUILayout.EndVertical();

            EditorGUILayout.HelpBox("support : darktreedevelopment@gmail.com", MessageType.None, true);
            GUILayout.BeginVertical("HelpBox");
            Texture newItemTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DarkTree FPS/Scripts/Editor/EditorImages/newItemEditorButton.png", typeof(Texture));
            Texture newNPCTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DarkTree FPS/Scripts/Editor/EditorImages/newNPCEditorButton.png", typeof(Texture));
            Texture newWeaponTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DarkTree FPS/Scripts/Editor/EditorImages/newWeaponEditorButton.png", typeof(Texture));

            if (GUILayout.Button(newItemTexture))
            {
                EditorApplication.ExecuteMenuItem("DarkTree FPS/Item editor");
            }
            if (GUILayout.Button(newWeaponTexture))
            {
                EditorApplication.ExecuteMenuItem("DarkTree FPS/Weapon wizard");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("HelpBox");


            if (GUILayout.Button("1. Edit player health, hunger, thirs parameters"))
            {
                if (FindObjectOfType<PlayerStats>() != null)
                {
                    windowID = 1;
                }
                else
                {
                    Debug.Log("PlayerStats not found");
                    EditorUtility.DisplayDialog("No GamePrefab found", "You need GamePrefab object in your scene to edit settings. Please drag and drop this from prefabs folder!", "Ok");
                }

            }
            if (GUILayout.Button("2. Edit sway controller"))
            {

            }
            if (GUILayout.Button("3. Edit player stats"))
            {

            }

            EditorGUILayout.HelpBox("support : darktreedevelopment@gmail.com", MessageType.None, true);
            GUILayout.EndVertical();
        }
    }

}