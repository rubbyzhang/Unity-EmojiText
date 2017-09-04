using System;
using UnityEngine;
using System.Collections;
using Celf;
using  System.Collections.Generic; 


/// <summary>
/// 管理表情資源列表數據
/// </summary>
public class InlineSpriteAssetManager : Singleton<InlineSpriteAssetManager>
{
    //已加載的Atlas
    private Dictionary<string,UIAtlas>  mAtlaseMap = new Dictionary<string, UIAtlas>();

    //Atlas中 uv信息
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
            return mAtlaseMap[assetPath];
        }
        

        UIAtlas atlas = AtlasManager.Instance.LoadAtlas(assetPath);
        if (atlas == null)
        {
            Debug.LogError("Load Atlas Failed , Path:" + assetPath);
            return null;
        }

        mAtlaseMap.Add(assetPath,atlas);


        Dictionary<string, SpriteAssetInfo> uvs = GetSpriteUVInfo(atlas);
        mSpriteUVMap.Add(assetPath, uvs);

        return atlas;
    }

    public Dictionary<string, SpriteAssetInfo> GetSpriteUVInfo(string assetPath)
    {
        UIAtlas atlas = GetAtlas(assetPath);
        if (atlas == null)
        {
            return null;
        }
    }

    private Dictionary<string, SpriteAssetInfo> GetSpriteUVInfo(UIAtlas atlas)
    {
        if (atlas == null)
        {
            return null;
        }

        return null;
    }


    public List<string> GetSpriteNamesFromPrefix(string namePrefix)
    {
        return null;
    }

    public List<SpriteAssetInfo> GetSpriteInfosFromPrefix(string namePrefix)
    {
        return null;
    }

    public SpriteAssetInfo GetSpriteUVInfo(string atlasAssetPath, string spriteName)
    {
        if (false == mSpriteUVMap.ContainsKey(atlasAssetPath))
        {
            Debug.LogError("_______________________GetSpriteUVInfo miss, path:" + atlasAssetPath);
            return null;
        }
        
        Dictionary<string, SpriteAssetInfo> uvs = mSpriteUVMap[atlasAssetPath];
        return uvs[spriteName];
    }

    public SpriteAssetInfo GetSpriteUvInfo(string atlasAssetPath, int index)
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
        return null;
    }
}
