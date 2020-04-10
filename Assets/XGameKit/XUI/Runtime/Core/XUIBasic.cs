using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using XGameKit.Core;
using Object = UnityEngine.Object;

namespace XGameKit.XUI
{

    public interface IXUIAssetLoader
    {
        T LoadAsset<T>(string name) where T : Object;
        void LoadAssetAsyn<T>(string name, Action<T> callback) where T : Object;
        void UnloadAsset(string name);
        void UnloadAllAssets();
    }

    public class XUIAssetLoaderDefault : IXUIAssetLoader
    {
        protected Dictionary<string, Object> m_caches = new Dictionary<string, Object>();

        public T LoadAsset<T>(string name) where T : Object
        {
#if UNITY_EDITOR
            var asset = AssetDatabase.LoadAssetAtPath<T>(name);
            if (asset == null)
            {
                Debug.LogError($"没有找到 {name}");
                return null;
            }
            return asset;
#endif
            return null;
        }
        public void LoadAssetAsyn<T>(string name, Action<T> callback ) where T : Object
        {
#if UNITY_EDITOR
            var asset = AssetDatabase.LoadAssetAtPath<T>(name);
            if (asset == null)
            {
                Debug.LogError($"没有找到 {name}");
                return;
            }
            callback.Invoke(asset);
            return;
#endif
            Object prefab = null;
            if (m_caches.ContainsKey(name))
            {
                prefab = m_caches[name];
            }
            else
            {
                prefab = Resources.Load(name);
                m_caches.Add(name, prefab);
            }
            callback.Invoke(prefab as T);
        }

        public void UnloadAsset(string name)
        {
            if (!m_caches.ContainsKey(name))
                return;
            //Resources.UnloadAsset(m_caches[name]);
            m_caches.Remove(name);
        }

        public void UnloadAllAssets()
        {
//            foreach (var prefab in m_caches.Values)
//            {
//                Resources.UnloadAsset(prefab);
//            }
            m_caches.Clear();
        }
    }

    public interface IXUILocalizationLoader
    {
        string GetLanguage();
        string GetLanguageText(string key);
    }

    public class XUILocalizationLoaderDefault : IXUILocalizationLoader
    {
        public string GetLanguage()
        {
            return "None";
        }

        public string GetLanguageText(string key)
        {
            return key;
        }
    }
    
    //
    public struct XUIParamBundle
    {
        public XMsgManager MsgManager;
        public XEvtManager EvtManager;
        public XUIRoot uiRoot;
        public IXUIAssetLoader AssetLoader;
        public IXUILocalizationLoader LocalizationLoader;
        public XUITextureManager TextureManager;

        public void Clear()
        {
            MsgManager = null;
            EvtManager = null;
            uiRoot = null;
            AssetLoader = null;
            LocalizationLoader = null;
            TextureManager = null;
        }
    }
}