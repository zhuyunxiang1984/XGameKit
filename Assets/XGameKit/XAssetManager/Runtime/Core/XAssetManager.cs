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
                XDebug.Log(XABConst.Tag, "模拟模式");
            }
            else
#endif
            {
                m_instance = new XAssetManagerOrdinary();
                XDebug.Log(XABConst.Tag, "普通模式");
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
            XDebug.Log(XABConst.Tag, $"加载AssetBundle(同步) {bundleName}");
            return m_instance.LoadBundle(bundleName);
        }
        public void LoadBundleAsync(string bundleName, Action<string, AssetBundle> callback = null)
        {
            XDebug.Log(XABConst.Tag, $"加载AssetBundle(异步) {bundleName}");
            m_instance.LoadAssetAsync(bundleName, callback);
        }
        public void UnloadBundle(string bundleName)
        {
            XDebug.Log(XABConst.Tag, $"卸载AssetBundle {bundleName}");
            m_instance.UnloadBundle(bundleName);
        }
        public T LoadAsset<T>(string assetName) where T : Object
        {
            XDebug.Log(XABConst.Tag, $"加载AssetObject(同步) {assetName}");
            return m_instance.LoadAsset<T>(assetName);
        }
        public void LoadAssetAsync<T>(string assetName, Action<string, T> callback = null) where T : Object
        {
            XDebug.Log(XABConst.Tag, $"加载AssetObject(异步) {assetName}");
            m_instance.LoadAssetAsync<T>(assetName, callback);
        }
        public void UnloadAsset(string assetName)
        {
            XDebug.Log(XABConst.Tag, $"卸载AssetObject {assetName}");
            m_instance.UnloadAsset(assetName);
        }
    }

}
