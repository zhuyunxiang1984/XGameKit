using System;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using Object = UnityEngine.Object;

namespace XGameKit.XAssetManager.Backup
{
    public class XAssetLoader
    {
        protected XAssetManager m_manager;
        protected List<string> m_loadedAssets = new List<string>();
        public XAssetLoader()
        {
            m_manager = XService.GetService<XAssetManager>();
        }
        public void Dispose()
        {
            foreach (var assetName in m_loadedAssets)
            {
                m_manager.UnloadAsset(assetName);
            }
            m_loadedAssets.Clear();
        }

        public T LoadAsset<T>(string assetName) where T : Object
        {
            m_loadedAssets.Add(assetName);
            return m_manager.LoadAsset<T>(assetName);
        }
        public void LoadAssetAsync<T>(string assetName, Action<string, T> callback = null) where T : Object
        {
            m_loadedAssets.Add(assetName);
            m_manager.LoadAssetAsync<T>(assetName, callback);
        }
        public void UnloadAsset(string assetName)
        {
            m_loadedAssets.Remove(assetName);
            m_manager.UnloadAsset(assetName);
        }
    }


}
