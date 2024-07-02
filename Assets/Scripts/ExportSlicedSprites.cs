using UnityEngine;
using UnityEditor;
using System.IO;

public class ExportSlicedSprites : EditorWindow
{
    private string exportPath = "Assets/ExportedSprites";

    [MenuItem("Tools/Export Sliced Sprites")]
    public static void ShowWindow()
    {
        GetWindow<ExportSlicedSprites>("Export Sliced Sprites");
    }

    private void OnGUI()
    {
        GUILayout.Label("Export Sliced Sprites", EditorStyles.boldLabel);

        GUILayout.Label("Export Path:", EditorStyles.label);
        exportPath = GUILayout.TextField(exportPath);

        if (GUILayout.Button("Export"))
        {
            ExportSprites();
        }
    }

    private void ExportSprites()
    {
        if (!Directory.Exists(exportPath))
        {
            Directory.CreateDirectory(exportPath);
        }

        // Get the selected sprite in the Project window
        Object selectedObject = Selection.activeObject;
        if (selectedObject == null || !(selectedObject is Texture2D))
        {
            Debug.LogError("Please select a sprite texture in the Project window.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(selectedObject);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

        // Load all sub-assets (sliced sprites)
        Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

        // Export each sliced sprite
        foreach (Object subAsset in subAssets)
        {
            if (subAsset is Sprite)
            {
                Sprite sprite = subAsset as Sprite;
                ExportSprite(sprite, texture, exportPath);
            }
        }

        Debug.Log("Sliced sprites exported successfully!");
        AssetDatabase.Refresh();
    }

    private void ExportSprite(Sprite sprite, Texture2D texture, string path)
    {
        // Create a new texture for the sprite
        Texture2D newTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] pixels = texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        // Encode texture to PNG
        byte[] pngData = newTexture.EncodeToPNG();
        if (pngData != null)
        {
            File.WriteAllBytes(Path.Combine(path, sprite.name + ".png"), pngData);
        }

        // Clean up
        DestroyImmediate(newTexture);
    }
}
