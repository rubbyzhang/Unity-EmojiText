using System;
using UnityEngine;
using System.Collections;
using Celf;
using  System.Collections.Generic;
using UnityEngine.Sprites;

/// <summary>
/// 管理表情資源列表數據
/// </summary>
public class InlineSpriteAssetManager : Singleton<InlineSpriteAssetManager>
{
    //已加載的Atlas
    [SerializeField]
    private Dictionary<string,UIAtlas>  mAtlaseMap = new Dictionary<string, UIAtlas>();

    //Atlas中 uv信息
    [SerializeField]
    private Dictionary<string, Dictionary<string, SpriteAssetInfo>> mSpriteUVMap = new Dictionary<string, Dictionary<string, SpriteAssetInfo>>();

    protected override void OnInit()
    {

    }
    
    protected override void OnRelease()
    {

    }

    public UIAtlas GetAtlas(string assetPath)
    {
        if (true == mAtlaseMap.ContainsKey(assetPath))
        {
            if (mAtlaseMap[assetPath].GetCount() > 0)
            {
                return mAtlaseMap[assetPath];
            }
            else
            {
                mAtlaseMap.Remove(assetPath);
                mSpriteUVMap.Remove(assetPath);
            }
        }

        UIAtlas atlas = AtlasManager.Instance.LoadAtlas(assetPath);
        if (atlas == null)
        {
            Debug.LogError("Load Atlas Failed , Path:" + assetPath);
            return null;
        }

        mAtlaseMap[assetPath] = atlas;


        Dictionary<string, SpriteAssetInfo> uvs = GetSpriteUVInfoMap(atlas);
        mSpriteUVMap[assetPath] =  uvs;

        return atlas;
    }
    
    private Dictionary<string, SpriteAssetInfo> GetSpriteUVInfoMap(UIAtlas atlas)
    {
        if (atlas == null)
        {
            return null;
        }

        Dictionary<string, SpriteAssetInfo> assetInfoMap = new Dictionary<string, SpriteAssetInfo>();
        List<Sprite> sprites = atlas.GetSpriteList();
        foreach (Sprite sprite in sprites)
        {
            SpriteAssetInfo assetInfo = GetUv(sprite);
            assetInfoMap.Add(sprite.name, assetInfo);
        }

        return assetInfoMap;
    }


    public List<string> GetSpriteNamesFromPrefix(string atlasAssetPath, string namePrefix)
    {
        List<SpriteAssetInfo> temp = GetSpriteInfosFromPrefix(atlasAssetPath, namePrefix);
        if (temp == null)
        {
            Debug.LogError("____________________GetSpriteNamesFromPrefix ERROR: 获取图片序列失败， name:" + namePrefix);
            return null;
        }

        List<string> strs = new List<string>();
        for (int i = 0; i < temp.Count; ++i)
        {
            strs.Add(temp[i].name);
        }

        return strs;
    }

    public List<SpriteAssetInfo> GetSpriteInfosFromPrefix(string atlasAssetPath, string namePrefix)
    {
        int MaxCount = 16;

        List<string> names = new List<string>();
        for (int i = 0; i < MaxCount; ++i)
        {
            names.Add(namePrefix + "_" + i.ToString());
        }

        List<SpriteAssetInfo> sprites = new List<SpriteAssetInfo>();

        for (int i = 0; i < MaxCount; ++i)
        {
            SpriteAssetInfo t = GetSpriteUVInfo(atlasAssetPath, names[i]);
            if (t != null)
            {
                sprites.Add(t);
            }
        }

        return sprites;
    }

    public SpriteAssetInfo GetSpriteUVInfo(string atlasAssetPath, string spriteName)
    {
        if (false == mSpriteUVMap.ContainsKey(atlasAssetPath))
        {
            return null;
        }
        
        Dictionary<string, SpriteAssetInfo> uvs = mSpriteUVMap[atlasAssetPath];
        if (false == uvs.ContainsKey(spriteName))
        {
            return null;
        }

        return uvs[spriteName];
    }

    public SpriteAssetInfo GetSpriteUvInfoByIndex(string atlasAssetPath, int index)
    {
        if (false == mAtlaseMap.ContainsKey(atlasAssetPath))
        {
            return null;
        }

        Sprite sprite = mAtlaseMap[atlasAssetPath].GetSprite(index);

        return GetUv(sprite);
    }

    private SpriteAssetInfo GetUv(Sprite sprite)
    {
        if (sprite == null)
        {
            return null;
        }

        SpriteAssetInfo assetInfo = new SpriteAssetInfo();
        Vector4 outer = DataUtility.GetOuterUV(sprite);
        assetInfo.name = sprite.name;
        assetInfo.rect = GetUvRect(sprite);
        return assetInfo;
    }

    private Rect GetUvRect(Sprite sprite)
    {
        Vector4 outer = DataUtility.GetOuterUV(sprite);
        Rect rect = new Rect();
        rect.x = outer.x;
        rect.y = outer.y;
        rect.width = outer.z - outer.x;
        rect.height = outer.w - outer.y;
        return rect;
    }
}
