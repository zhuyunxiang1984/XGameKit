using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using Object = UnityEngine.Object;

namespace XGameKit.XAssetManager
{
    
    public class XAssetManager : IXAssetManager, IXService
    {
        protected IXAssetManager m_instance;
        public XAssetManager()
        {
#if UNITY_EDITOR
            var mode = (EnumResMode) UnityEditor.EditorPrefs.GetInt(XABConst.EKResMode, XABConst.EKResModeDefaultValue);
            if (mode == EnumResMode.Simulate)
            {
                m_instance = new XAssetManagerSimulate();
            }
            else
#endif
            {
                m_instance = new XAssetManagerOrdinary();
            }
        }
        public void Dispose()
        {
            m_instance.Dispose();
        }
        public void Tick(float deltaTime){
            m_instance.Tick(deltaTime);
        }
        public List<string> GetDependencies(string bundleName)
        {
            return m_instance.GetDependencies(bundleName);
        }
        public string GetBundleNameByAssetName(string assetName)
        {
            return m_instance.GetBundleNameByAssetName(assetName);
        }
        public AssetBundle LoadBundle(string bundleName)
        {
            return m_instance.LoadBundle(bundleName);
        }
        public void LoadBundleAsync(string bundleName, Action<string, AssetBundle> callback = null){
            m_instance.LoadAssetAsync(bundleName, callback);
        }
        public void UnloadBundle(string bundleName){
            m_instance.UnloadBundle(bundleName);
        }
        public T LoadAsset<T>(string assetName) where T : Object
        {
            return m_instance.LoadAsset<T>(assetName);
        }
        public void LoadAssetAsync<T>(string assetName, Action<string, T> callback = null) where T : Object{
            m_instance.LoadAssetAsync<T>(assetName, callback);
        }
        public void UnloadAsset(string assetName){
            m_instance.UnloadAsset(assetName);
        }
    }

}
