using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using Celf;

public class AtlasPacker : EditorWindow
{
    [MenuItem("Tools/Pack All Atlas")]
    public static void PackAllAtlas()
    {
        for (int i = 0; i < UIAtlasUtility.ConstAtlasPath.Length; ++i)
        {
            PackAtlasUnderPath("Assets/" + UIAtlasUtility.ConstAtlasPath[i]);
        }
    }

    [MenuItem("Tools/Pack Selected Atlas")]
    public static void PackAtlas()
    {
        // 检查是否选中了一个目录
        string basePath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(basePath))
        {
            Debug.LogError("Please select the sprite folder to pack!!!");
            return;
        }

        PackAtlasUnderPath(basePath);
    }

    public static void PackAtlasUnderPath(string basePath)
    {
        // 先检查resource下atlas目录是否存在，不存在就创建
        DirectoryInfo atlasDirInfo = new DirectoryInfo(Application.dataPath + "/" + UIAtlasUtility.atlasRootPath);
        if (!atlasDirInfo.Exists)
        {
            atlasDirInfo.Create();
        }

        string prefix = "Assets/";
        // 去掉Assets前缀
        basePath = basePath.Substring(prefix.Length);
        // 是否在需要打包的sprite的根目录下
        if (!basePath.StartsWith(UIAtlasUtility.spriteRootPath))
        {
            Debug.LogError("The source folder should be under: " + UIAtlasUtility.spriteRootPath);
            return;
        }

        // 绝对路径
        string absoluteBasePath = Application.dataPath + "/" + basePath;
        if (!Directory.Exists(absoluteBasePath))
        {
            Debug.LogError("Please select the sprite folder to pack!!!");
            return;
        }

        DirectoryInfo spriteDirInfo = new DirectoryInfo(absoluteBasePath);
        string atlasName = "";
        if (!basePath.Equals(UIAtlasUtility.spriteRootPath))
        {
            atlasName = basePath.Replace('/', UIAtlasUtility.altasNameSeparator);
            atlasName= atlasName.ToLower();
        }

        // 将basePath下的sprite进行递归打包atlas
        PackDir(spriteDirInfo, atlasName);
    }

    private static void PackDir(DirectoryInfo dirInfo, string atlasName)
    {
        if (atlasName != "")
        {
            GameObject go = new GameObject(atlasName);
            UIAtlas uiAtlas = go.AddComponent<UIAtlas>();
            uiAtlas.atlasName = atlasName;
            foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.TopDirectoryOnly))
            {
                string allPath = pngFile.FullName;
                string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (null == sprite)
                {
                    Debug.LogError("PackAtlas Error. Cannot load sprite: " + assetPath);
                    continue;
                }
                uiAtlas.AddSprite(sprite);
            }

            string atlasPath = UIAtlasUtility.atlasRootPath + atlasName + ".prefab";
            PrefabUtility.CreatePrefab("Assets/" + atlasPath, go);
            GameObject.DestroyImmediate(go);
        }

        foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
        {
            string subAtlasName = atlasName + UIAtlasUtility.altasNameSeparator + subDirInfo.Name;
            if (atlasName == "")
            {
                subAtlasName = subDirInfo.Name;
            }

            PackDir(subDirInfo, subAtlasName);
        }
    }
}
