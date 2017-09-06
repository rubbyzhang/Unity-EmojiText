using UnityEngine;
using System.Collections.Generic;
using Celf;
using UnityEngine.UI;
using UnityEngine.Serialization;
/// <summary>
/// 表情渲染数据
/// </summary>
public class inlineSprite_New : MaskableGraphic
{
    [SerializeField]
    public string AtlasAssetPath = "UI/atlas/common";

    private Sprite m_Sprite;

    public Sprite sprite
    {
        get { return m_Sprite; }
        set
        {
            m_Sprite = value;
            SetAllDirty();
        }
    }

//    public override Texture mainTexture
//    {
//        get
//        {
//            return GetMainTextture();
//        }
//    }
//    public Texture GetMainTextture()
//    {
//        UIAtlas atlas = InlineSpriteAssetManager.Instance.GetAtlas(AtlasAssetPath);
//        if (atlas == null)
//        {
//            return s_WhiteTexture;
//        }
//
//        Sprite sprite = atlas.GetSprite(0);
//        if (sprite != null)
//        {
//            return sprite.texture;
//        }
//
//        return s_WhiteTexture;
//    }
    

    private Sprite GetSpite()
    {
        UIAtlas atlas = InlineSpriteAssetManager.Instance.GetAtlas(AtlasAssetPath);
        if (atlas == null)
        {
            return null;
        }

        return atlas.GetSprite(0);
    }

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


    protected override void Awake()
    {
        transform.localPosition = new Vector3(10000, 10000, 1000);
        sprite = GetSpite();
        UpdateMaterial();
    }





    public void UpdateMesh(Vector3[] mTempVertices , Vector2[] mTempUv , int[] mTempTriangles)
    {
        UnityEngine.Profiling.Profiler.BeginSample("inline SpriteManager DrawSprite UpdateMesh");

        Mesh newSpriteMesh = new Mesh();
        newSpriteMesh.vertices = mTempVertices;
        newSpriteMesh.uv = mTempUv;
        newSpriteMesh.triangles = mTempTriangles;
        GetComponent<CanvasRenderer>().SetMesh(newSpriteMesh);
        UpdateMaterial();

        UnityEngine.Profiling.Profiler.EndSample();
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
