using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XGameKit.XAssetManager
{
    public class LoadProviderForDevMode : ILoadProvider
    {
        public T LoadAsset<T>(string assetName) where T : Object
        {
            return null;
        }

        public IEnumerator LoadAssetAsync<T>(string assetName, Action<T> OnComplete) where T : Object
        {
            yield return null;
        }

        public void UnloadAsset(string assetName)
        {

        }
        

        public AssetBundle LoadBundle(string bundleName)
        {
            return null;
        }
        public IEnumerator LoadBundleAsync(string bundleName)
        {
            yield break;
        }

        public void UnloadBundle(string bundleName)
        {

        }
    }
}
