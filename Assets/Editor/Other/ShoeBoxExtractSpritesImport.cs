using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// 处理ShoeBox切割的图片到Sprites
/// </summary>
public class ShoeBoxExtractSpritesImport
{
    [MenuItem("Assets/ShoeBox/Process to Sprites")]
    static void ProcessExtractSpritesToSprites()
    {
        TextAsset txt = Selection.activeObject as TextAsset;
        if (txt == null)
        {
            Debug.LogError("Must be .txt file!");
            return;
        }
        string assetPath = AssetDatabase.GetAssetPath(txt);
        if (Path.GetExtension(assetPath) != ".txt")
        {
            Debug.LogError("Must be .txt file!");
            return;
        }

        string rootPath = Path.GetDirectoryName(assetPath);
        string path = rootPath + "/" + Path.GetFileNameWithoutExtension(assetPath);
        TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
        if (texImp == null)
        {
            return;
        }

        object[] args = new object[2] { 0, 0 };
        MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        mi.Invoke(texImp, args);

        var height = (int)args[1];
        List<SpriteMetaData> sprites = GetSpriteMetaDatas(rootPath, txt.text, height);

        texImp.spritesheet = sprites.ToArray();
        texImp.textureType = TextureImporterType.Sprite;
        texImp.spriteImportMode = SpriteImportMode.Multiple;

        AssetDatabase.DeleteAsset(assetPath);
        AssetDatabase.ImportAsset(path);
    }

    static List<SpriteMetaData> GetSpriteMetaDatas(string path, string text, int totalHeight)
    {
        List<SpriteMetaData> sprites = new List<SpriteMetaData>();
        var sliceName = "slice";
        var arrayTxt = text.Split(',');
        var pngIndex = 0;
        foreach (var txt in arrayTxt)
        {
            var arrayPoint = txt.Split(' ');

            int x = int.Parse(arrayPoint[arrayPoint.Length - 2]);
            int y = int.Parse(arrayPoint[arrayPoint.Length - 1].TrimEnd('.'));

            pngIndex++;
            var pngName = sliceName + pngIndex.ToString().PadLeft(2, '0') + ".png";
            var pngPath = path + "/" + pngName;
            TextureImporter importer = AssetImporter.GetAtPath(pngPath) as TextureImporter;

            if (importer != null)
            {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);

                var width = (int)args[0];
                var height = (int)args[1];

                SpriteMetaData smd = new SpriteMetaData();
                smd.rect = new Rect(x, totalHeight - y - height, width, height);
                smd.alignment = 0;
                smd.name = pngName.Remove(pngName.Length - 4);
                smd.pivot = new Vector2(0.5f, 0.5f);
                sprites.Add(smd);

                AssetDatabase.DeleteAsset(pngPath);
            }
        }
        return sprites;
    }
}
