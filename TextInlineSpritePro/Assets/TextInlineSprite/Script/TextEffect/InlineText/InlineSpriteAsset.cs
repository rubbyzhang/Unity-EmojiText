using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpriteAssetInfo
{
    public string name;
    public Rect rect;
}

/// <summary>
/// 表情图片序列化信息
/// </summary>
public class InlineSpriteAsset : ScriptableObject
{
    public Texture TextureSource;
    public List<SpriteAssetInfo> listSpriteInfor;
}
