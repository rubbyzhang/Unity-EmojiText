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
            if (inlineSpriteAsset == null)
            {
                return s_WhiteTexture;
            }

            if (inlineSpriteAsset.TextureSource == null)
            {
                Debug.LogError("InlineSpriteGraphic: SpriteAsset.texSource is null");
                return s_WhiteTexture;
            }
            else
            {
                return TestImage.sprite.texture;
            }
        }
    }

    public new void UpdateMaterial()
    {
        base.UpdateMaterial();
    }



    protected override void Awake()
    {
        if (inlineSpriteAsset != null && inlineSpriteAsset.TextureSource != null)
        {
            return;
        }

        InlineTextManager.Instance.RebulidSpriteData();

        inlineSpriteAsset = InlineTextManager.Instance.InlineSpriteAsset;

        UpdateMaterial();

        transform.localPosition = new Vector3(1000,1000,1000);

        base.Awake();
    }


    public SpriteAssetInfo GetSpriteInfo(int index)
    {
        return InlineSpriteAssetManager.Instance.GetSpriteInfo(index);
    }

    public SpriteAssetInfo GetSpriteInfo(string name)
    {
        return InlineSpriteAssetManager.Instance.GetSpriteUVInfo(AtlasAssetPath , name);
    }

    public List<SpriteAssetInfo> GetSpriteInfosFromPrefix(string namePrefix)
    {
        return InlineSpriteAssetManager.Instance.GetSpriteInfosFromPrefix(namePrefix);
    }

    public List<string> GetSpriteNamesFromPrefix(string namePrefix)
    {
        return InlineSpriteAssetManager.Instance.GetSpriteNamesFromPrefix(namePrefix);
    }


}
