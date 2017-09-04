using UnityEngine;
using System.Collections.Generic;
using Celf;
using UnityEngine.UI;

/// <summary>
/// 表情渲染数据
/// </summary>
public class inlineSprite_New : MaskableGraphic
{
    public string AtlasAssetPath = "UI/atlas/common";

    public override Texture mainTexture
    {
        get
        {
            Texture tex = GetMainTextture();

            if (tex == null)
            {
                Debug.LogError("InlineSpriteGraphic: SpriteAsset.texSource is null");
                return s_WhiteTexture;
            }

            return tex;
        }
    }

    public new void UpdateMaterial()
    {
        base.UpdateMaterial();
    }

    protected override void Awake()
    {
        UpdateMaterial();

        transform.localPosition = new Vector3(10000,10000,1000);

        base.Awake();
    }

    public Texture GetMainTextture()
    {
        UIAtlas atlas = InlineSpriteAssetManager.Instance.GetAtlas(AtlasAssetPath);
        if (atlas == null)
        {
            return null;
        }

        Sprite sprite = atlas.GetSprite(0);
        if (sprite != null)
        {
            return sprite.texture;
        }

        return null;
    }


    public SpriteAssetInfo GetSpriteInfo(int index)
    {
        return InlineSpriteAssetManager.Instance.GetSpriteUvInfoByIndex(AtlasAssetPath, index);
    }

    public SpriteAssetInfo GetSpriteInfo(string name)
    {
        return InlineSpriteAssetManager.Instance.GetSpriteUVInfo(AtlasAssetPath , name);
    }

    public List<SpriteAssetInfo> GetSpriteInfosFromPrefix(string namePrefix)
    {
        return InlineSpriteAssetManager.Instance.GetSpriteInfosFromPrefix(AtlasAssetPath, namePrefix);
    }

    public List<string> GetSpriteNamesFromPrefix(string namePrefix)
    {
        return InlineSpriteAssetManager.Instance.GetSpriteNamesFromPrefix(AtlasAssetPath, namePrefix);
    }


}
