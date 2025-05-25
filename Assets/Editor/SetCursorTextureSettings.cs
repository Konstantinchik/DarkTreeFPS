using UnityEngine;
using UnityEditor;
using System.IO;

public class SetCursorTextureSettings
{
    [MenuItem("Tools/Настроить PNG как курсоры")]
    public static void SetTexturesAsCursor()
    {
        string path = "Assets/Resources/CursorFrames/";
        string[] files = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string assetPath = file.Replace('\\', '/');
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                importer.textureType = TextureImporterType.Cursor;
                importer.isReadable = true;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();

                Debug.Log($"Настроен курсор: {assetPath}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("✅ Завершено: Все PNG в CursorFrames настроены как курсоры.");
    }
}
