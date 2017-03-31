using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 已知问题：
/// 1. 资源加载方式
/// 2. 多张贴图使用
/// </summary>
public class InlineTextManager : MonoBehaviour
{
    [SerializeField]
    public InlineSpriteAsset inlineSpriteAsset;

    private Dictionary<string, SpriteAssetInfo> mSpriteInfoDic;

    //Resource/路径下
    private readonly string defaultSpriteAssetResPath = "emoji/default_emoji";

    private static InlineTextManager mInstance;
    public  static InlineTextManager Instance
    {
        get
        {
            if (mInstance == null)
            {
				#if UNITY_EDITOR
				    mInstance = FindObjectOfType<InlineTextManager>() ;
                #endif

                if (mInstance == null) 
                {
					mInstance = new GameObject ("(Singleton) " + typeof(InlineTextManager).Name).AddComponent<InlineTextManager> ();
                }
            }
            return mInstance;
        }
    }

    void Awake()
    {
        CheckSpriteAsset();
    }

    public InlineSpriteAsset InlineSpriteAsset
    {
        get
        {
            if (false == CheckSpriteAsset())
            {
                return null;
            }

            return inlineSpriteAsset;
        }
        set
        {
            if (value == null)
            {
                Debug.LogError("InlineTextManager Value input is nill");
                return;
            }

            inlineSpriteAsset = value;
        }
    }


    /// <summary>
    /// 获得资源中所有表情名字
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllEmojiNames()
    {
        if (inlineSpriteAsset == null)
        {
            CheckSpriteAsset();
        }

        if (inlineSpriteAsset == null)
        {
            return null;
        }

        if (mSpriteInfoDic == null)
        {
            RebulidSpriteData();
        }

        Dictionary<string,string> nameDic = new Dictionary<string, string>();

        if (mSpriteInfoDic != null)
        {
            List<string> keys = mSpriteInfoDic.Keys.ToList();

            for (int i = 0; i < keys.Count; ++i)
            {
                string[] strs = keys[i].Split('_');
                if (strs.Length == 2)
                {
                    if (!nameDic.ContainsKey(strs[0]))
                    {
                        nameDic.Add(strs[0], strs[0]);
                    }
                }
                else
                {
                    Debug.Log("GetAllEmojiNames Split Fail");
                }
            }
        }

        return nameDic.Keys.ToList();
    }

    private bool CheckSpriteAsset() 
    {
        if (inlineSpriteAsset != null && inlineSpriteAsset.TextureSource != null)
        {
            return true;
        }

        //TODO  
        inlineSpriteAsset = Resources.Load<InlineSpriteAsset>(defaultSpriteAssetResPath);
        if (inlineSpriteAsset == null)
        {
            Debug.LogError(defaultSpriteAssetResPath + " Load Failed");
            return false;
        }
        return true;
    }

    public void RebulidSpriteData()
    {
        if (inlineSpriteAsset == null)
        {
            CheckSpriteAsset();
        }
        if (inlineSpriteAsset == null)
        {
            return;
        }

        if (mSpriteInfoDic == null)
        {
            mSpriteInfoDic = new Dictionary<string, SpriteAssetInfo>();
        }

        for (int i = 0; i < inlineSpriteAsset.listSpriteInfor.Count; ++i)
        {
            SpriteAssetInfo t = inlineSpriteAsset.listSpriteInfor[i];
            mSpriteInfoDic[t.name] = t;
        }
    }

    public SpriteAssetInfo GetSpriteInfo(int index)
    {
        if (inlineSpriteAsset == null)
        {
            CheckSpriteAsset();
        }

        if (inlineSpriteAsset == null)
        {
            return null;
        }

        if (index < 0 || index >= inlineSpriteAsset.listSpriteInfor.Count)
        {
            return null;
        }

        return inlineSpriteAsset.listSpriteInfor[index];
    }


    public SpriteAssetInfo GetSpriteInfo(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        if (inlineSpriteAsset == null)
        {
            CheckSpriteAsset();
        }

        if (mSpriteInfoDic == null)
        {
            RebulidSpriteData();
        }

        if (mSpriteInfoDic != null && mSpriteInfoDic.ContainsKey(name))
        {
            return mSpriteInfoDic[name];
        }
        return null;
    }

    /// <summary>
    /// 根据名字前缀获得列表，支持最大8个扩展，如Prefix_1 ,Prefix_2,Prefix_3
    /// </summary>
    /// <param name="namePrefix"></param>
    /// <returns></returns>
    public List<SpriteAssetInfo> GetSpriteInfosFromPrefix(string namePrefix)
    {
        if (string.IsNullOrEmpty(namePrefix))
        {
            return null;
        }

        #if UNITY_EDITOR //TODO
            if (inlineSpriteAsset == null)
            {
                CheckSpriteAsset();
            }
        #endif

        if (mSpriteInfoDic == null)
        {
            RebulidSpriteData();
        }

        int MaxCount = 8;
        List<string> names = new List<string>();
        for (int i = 0; i < MaxCount; ++i)
        {
            names.Add(namePrefix + "_" + i.ToString());
        }

        List<SpriteAssetInfo> sprites = new List<SpriteAssetInfo>();

        for (int i = 0; i < MaxCount; ++i)
        {
            SpriteAssetInfo t = GetSpriteInfo(names[i]);
            if (t != null)
            {
                sprites.Add(t);
            }
        }
        return sprites;
    }

    public List<string> GetSpriteNamesFromPrefix(string namePrefix)
    {
        List<SpriteAssetInfo> temp = GetSpriteInfosFromPrefix(namePrefix);
        if (temp == null)
        {
            return null;
        }

        List<string> strs = new List<string>(); 
        for (int i = 0; i < temp.Count; ++ i)
        {
            strs.Add(temp[i].name);
        }

        return strs;
    }
}
