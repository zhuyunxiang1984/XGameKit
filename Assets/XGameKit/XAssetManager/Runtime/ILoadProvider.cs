using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XGameKit.XAssetManager
{

    public interface ILoadProvider
    {
        T LoadAsset<T>(string assetName) where T : Object;
        IEnumerator LoadAssetAsync<T>(string assetName, Action<T> OnComplete) where T : Object;
        void UnloadAsset(string assetName);
        
        AssetBundle LoadBundle(string bundleName);
        IEnumerator LoadBundleAsync(string bundleName);
        void UnloadBundle(string bundleName);
    }
}
