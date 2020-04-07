using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using XGameKit.Core;
using Object = UnityEngine.Object;

namespace XGameKit.XUI
{

    public interface IXUIAssetLoader
    {
        void LoadAsset(string name, Action<GameObject> callback = null);
        void UnloadAsset(string name);
        void UnloadAllAssets();
        void AddListener(Action<string, GameObject> callback);
        void DelListener(Action<string, GameObject> callback);
    }

    public class XUIAssetLoaderDefault : IXUIAssetLoader
    {
        protected Dictionary<string, Object> m_caches = new Dictionary<string, Object>();
        protected Action<string, GameObject> m_callback;
        public void LoadAsset(string name, Action<GameObject> callback = null)
        {
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
            callback?.Invoke(prefab as GameObject);

            m_callback?.Invoke(name, prefab as GameObject);
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

        public void AddListener(Action<string, GameObject> callback)
        {
            m_callback += callback;
        }

        public void DelListener(Action<string, GameObject> callback)
        {
            m_callback -= callback;
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

        public void Clear()
        {
            MsgManager = null;
            EvtManager = null;
            uiRoot = null;
            AssetLoader = null;
            LocalizationLoader = null;
        }
    }
}