using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public static class CreateSpriteAsset
{
    private static  string TargetPath = "Assets/Resources/emoji/";

    [MenuItem("Assets/Create/UGUI Sprite Asset",false,10)]
    static void main()
    {
        Object target = Selection.activeObject;
        if (target == null || target.GetType() != typeof (Texture2D))
        {
            return;
        }

        Texture2D sourceTex = target as Texture2D;
        //整体路径
        string filePathWithName = AssetDatabase.GetAssetPath(sourceTex);
        //带后缀的文件名
        //string fileNameWithExtension = Path.GetFileName(filePathWithName);
        //string filePath = filePathWithName.Replace(fileNameWithExtension, "");


        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathWithName);

        InlineSpriteAsset inlineSpriteAsset = AssetDatabase.LoadAssetAtPath(TargetPath + fileNameWithoutExtension + ".asset", typeof(InlineSpriteAsset)) as InlineSpriteAsset;
        bool isNewAsset = inlineSpriteAsset == null ? true : false;
       // if (isNewAsset)
        {
            inlineSpriteAsset = ScriptableObject.CreateInstance<InlineSpriteAsset>();
            inlineSpriteAsset.TextureSource = sourceTex;
            inlineSpriteAsset.listSpriteInfor = GetSpritesInfor(sourceTex);

            AssetDatabase.CreateAsset(inlineSpriteAsset, TargetPath + fileNameWithoutExtension + ".asset");
        }

        CheckSprite(sourceTex);

        Debug.Log("_______________________________File:" + fileNameWithoutExtension + "Gerenated sucess");
    }

    private static void CheckSprite(Texture2D tex)
    {
        string filePath = UnityEditor.AssetDatabase.GetAssetPath(tex);

        Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(filePath);

        Vector2 newTexSize = new Vector2(tex.width, tex.height);

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] is Sprite)
            {

            }
        }
    }

    public static
        List<SpriteAssetInfo> GetSpritesInfor(Texture2D tex)
    {
        List<SpriteAssetInfo> m_sprites = new List<SpriteAssetInfo>();

        string filePath = UnityEditor.AssetDatabase.GetAssetPath(tex);

        Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(filePath);

        Vector2 newTexSize = new Vector2(tex.width, tex.height);

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] is Sprite)
            {
                SpriteAssetInfo temp = new SpriteAssetInfo();
                Sprite sprite = objects[i] as Sprite;
                temp.name = sprite.name;
                Rect newRect = new Rect();
                newRect.x = sprite.rect.x / newTexSize.x;
                newRect.y = sprite.rect.y / newTexSize.y;
                newRect.width = sprite.rect.width / newTexSize.x;
                newRect.height = sprite.rect.height / newTexSize.y;
                temp.rect = newRect;
                m_sprites.Add(temp);
            }
        }
        return m_sprites;
    }
}
