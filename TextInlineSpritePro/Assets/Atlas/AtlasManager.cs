using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celf;

namespace Celf
{
    public class AtlasManager : Singleton<AtlasManager>
    {
        private Dictionary<string, UIAtlas> mAtlasMap = new Dictionary<string, UIAtlas>();

        protected override void OnInit()
        {
        }

        protected override void OnRelease()
        {
            ReleaseAll();
        }

        /// <param name="spriteAssetPath"> 相对asset的路径</param>
        public Sprite GetSprite(string spriteAssetPath)
        {
            if (string.IsNullOrEmpty(spriteAssetPath))
            {
                return null;
            }

            int slashIndex = spriteAssetPath.LastIndexOf('/');
            if (-1 == slashIndex)
            {
                return null;
            }

            string atlasPath = spriteAssetPath.Substring(0, slashIndex);

            UIAtlas atlas = LoadAtlas(atlasPath);
            if (atlas == null)
            {
                Debug.LogError("AtlasManager GetSprite Error: Atlas(" + spriteAssetPath + ") is null");
                return null;
            }

            string spriteName = spriteAssetPath.Substring(slashIndex + 1);

            return atlas.GetSprite(spriteName);
        }

        public string GetSpriteName(string spritePath)
        {
            if (string.IsNullOrEmpty(spritePath))
            {
                return string.Empty;
            }

            return spritePath.Substring(spritePath.LastIndexOf('/') + 1);
        }

        public void PreLoadAtlas(string assetPath)
        {
            LoadAtlas(assetPath);
        }

        public UIAtlas LoadAtlas(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            if (!mAtlasMap.ContainsKey(assetPath))
            {
                assetPath = assetPath.ToLower();
                assetPath = "atlas/" + assetPath.Replace('/', '_');
                UIAtlas atlas = Resources.Load<UIAtlas>(assetPath);
                if (null == atlas)
                {
                    Debug.LogError("Atlas is null!!!!!!!!!!!!!!!!!!");
                    return null;
                }
                else
                {
                    mAtlasMap[assetPath] = atlas;
                }
            }
            return mAtlasMap[assetPath];
        }

        public void ReleaseAtlas(string assetPath)
        {
            if (mAtlasMap.ContainsKey(assetPath))
            {
                mAtlasMap[assetPath].RemoveAll();
                mAtlasMap.Remove(assetPath);
//                Resources.UnloadAsset(assetPath);
            }
        }

        public void ReleaseAll()
        {
            List<string> keys = mAtlasMap.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                ReleaseAtlas(keys[i]);
            }
        }

        public void PreLoadAtlas(AtlasType type)
        {
            LoadAtlas(type);
        }

        public UIAtlas LoadAtlas(AtlasType type)
        {
            string path = GetPath(type);
            return LoadAtlas(path);
        }

        public void ReleaseAtlas(AtlasType type)
        {
            string path = GetPath(type);
            ReleaseAtlas(path);
        }

        private AtlasType GetType(string spritePath)
        {
            if (string.IsNullOrEmpty(spritePath))
            {
                return AtlasType.None;
            }

            spritePath = spritePath.ToLower();
            for (int i = 0; i < UIAtlasUtility.ConstAtlasPath.Length; i++)
            {
                if (spritePath.Contains(UIAtlasUtility.ConstAtlasPath[i]))
                {
                    return (AtlasType) i;
                }
            }
            return AtlasType.None;
        }

        private string GetPath(AtlasType type)
        {
            return UIAtlasUtility.ConstAtlasPath[(int) type];
        }
    }
}
