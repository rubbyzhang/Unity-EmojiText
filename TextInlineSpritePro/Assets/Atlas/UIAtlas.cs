using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Celf
{
    public class UIAtlas : MonoBehaviour
    {
        public string atlasName;

        [SerializeField]
        private List<Sprite> sprites = new List<Sprite>();

        /// <summary>
        /// 向该atlas添加一个sprite
        /// </summary>
        /// <param name="sprite"></param>
        public void AddSprite(Sprite sprite)
        {
            if (null == sprite)
            {
                return;
            }

            sprites.Add(sprite);
        }

        public void RemoveAll()
        {
            sprites.Clear();
        }

        /// <summary>
        /// 根据名字获取该atlas下的某个sprite
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Sprite GetSprite(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("AddSprite Error. Sprite name invalid: " + name);
                return null;
            }

            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i] == null)
                {
                    Debug.LogError("Empty sprite " + i + " atlas name :" + atlasName);
                    continue;
                }
                if (sprites[i].name == name)
                {
                    return sprites[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 该atlas是否包含指定sprite
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public bool ContainsSprite(Sprite sprite)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i] == sprite)
                {
                    return true;
                }
            }

            return false;
        }

        public Sprite GetSprite(int index)
        {
            if (index < 0 || index >= sprites.Count)
            {
                return null;
            }
            return sprites[index];
        }

        public List<Sprite> GetSpriteList()
        {
            return sprites;
        }

        public int GetCount()
        {
            return sprites.Count;
        }
    }

}