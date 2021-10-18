using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XGameKit.XAssetManager
{
    public class XAssetManager : MonoBehaviour
    {
        public void Initialize()
        {
        
        }

        public T LoadAsset<T>(string assetName) where T : Object
        {
            return null;
        }

        public void LoadAssetAsync<T>(string assetName, Action<T> onComplete) where T : Object
        {
            StartCoroutine(LoadAssetAsyncInternal(assetName, onComplete));
        }

        private IEnumerator LoadAssetAsyncInternal<T>(string assetName, Action<T> onComplete) where T : Object
        {
            yield return null;
        }
    }
}

