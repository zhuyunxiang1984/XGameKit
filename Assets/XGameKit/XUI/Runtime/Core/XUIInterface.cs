using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XGameKit.XUI
{
    public interface IXUIAssetLoader
    {
        void LoadAsset(string name, Action<GameObject> callback);
        void UnloadAsset(string name);
        void UnloadAllAssets();
    }

    public class XUIAssetLoaderDefault : IXUIAssetLoader
    {
        protected Dictionary<string, Object> m_caches = new Dictionary<string, Object>();
        public void LoadAsset(string name, Action<GameObject> callback)
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
        }

        public void UnloadAsset(string name)
        {
            if (!m_caches.ContainsKey(name))
                return;
            Resources.UnloadAsset(m_caches[name]);
            m_caches.Remove(name);
        }

        public void UnloadAllAssets()
        {
            foreach (var prefab in m_caches.Values)
            {
                Resources.UnloadAsset(prefab);
            }
            m_caches.Clear();
        }
    }
}