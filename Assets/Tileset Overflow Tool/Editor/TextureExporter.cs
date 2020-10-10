using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class TextureExporter
{
    public static string mainPath = "/Resources/";

    public static void ExportTexture(Texture2D texture, string dirPath, string fileName, int ppu = 32)
    {
        //Save To Disk as PNG
        byte[] bytes = texture.EncodeToPNG();
        if (!Directory.Exists(Application.dataPath + mainPath + dirPath))
        {
            Directory.CreateDirectory(mainPath + dirPath);
        }
        File.WriteAllBytes(Application.dataPath + mainPath + dirPath + fileName + ".png", bytes);
        AssetDatabase.Refresh();       
        
        string path = "Assets/Resources/" + dirPath + fileName + ".png";
        TextureImporter texImporter = AssetImporter.GetAtPath(path) as TextureImporter;        
        texImporter.isReadable = true;
        texImporter.filterMode = texture.filterMode;
        texImporter.spritePixelsPerUnit = ppu;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        Texture2D tex = AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;

        AssetDatabase.Refresh();
    }
}
