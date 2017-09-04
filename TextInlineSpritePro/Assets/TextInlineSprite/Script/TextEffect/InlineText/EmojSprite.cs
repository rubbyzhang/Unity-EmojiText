using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class EmojSprite : MaskableGraphic
{
    public Sprite sprite;




    public override Texture mainTexture
    {
        get
        {
            if (sprite == null)
            {
                if (material != null && material.mainTexture != null)
                {
                    return material.mainTexture;
                }
                return s_WhiteTexture;
            }
            return sprite.texture;
        }
    }

    protected override void UpdateMaterial()
    {
        base.UpdateMaterial();
    }
}