using UnityEngine;

namespace Celf
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T mInstance;
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                mInstance = FindObjectOfType<T>();
#endif
                if (mInstance == null)
                {
                    mInstance = new GameObject("(Singleton) " + typeof (T).Name).AddComponent<T>();
                }

                return mInstance;
            }
        }

        public static bool ContainsInstance() {
            return mInstance != null;
        }

        private void Awake()
        {
            T instance = this as T;

            if (mInstance != null && mInstance != instance)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            // 切换场景不要销毁GameObject
            DontDestroyOnLoad(gameObject);
            
            mInstance = instance;
            OnInit();
        }

        private void OnDestroy()
        {
            OnRelease();
            mInstance = null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void OnInit()
        {
            
        }

        /// <summary>
        /// 释放
        /// </summary>
        protected virtual void OnRelease()
        {
            
        }
    }
}