

using XGameKit.Core;
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;

namespace XGameKit.XAssetManager
{
    public class XAssetManagerSimulate : IXAssetManager
    {
        public void Dispose()
        {
            
        }
        public void Tick(float deltaTime)
        {
            
        }
        public List<string> GetDependencies(string bundleName)
        {
            return null;
        }
        public string GetBundleNameByAssetName(string assetName)
        {
            return string.Empty;
        }
        public AssetBundle LoadBundle(string bundleName)
        {
            return null;
        }
        public void LoadBundleAsync(string bundleName, Action<string, AssetBundle> callback = null)
        {
            
        }
        public void UnloadBundle(string bundleName)
        {
            
        }
        public T LoadAsset<T>(string assetName) where T : Object
        {
            XDebug.Log(XABConst.Tag, $"--加载AssetObject(同步) {assetName}");
            assetName = assetName.ToLower();
            var assetPath = XABAssetNameConfig.GetAssetPath(assetName);
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
        public void LoadAssetAsync<T>(string assetName, Action<string, T> callback = null) where T : Object
        {
            XDebug.Log(XABConst.Tag, $"--加载AssetObject(异步) {assetName}");
            assetName = assetName.ToLower();
            var assetPath = XABAssetNameConfig.GetAssetPath(assetName);
            callback?.Invoke(assetName, AssetDatabase.LoadAssetAtPath<T>(assetPath));
        }
        public void UnloadAsset(string assetName)
        {
            
        }
    }
}
#endif
