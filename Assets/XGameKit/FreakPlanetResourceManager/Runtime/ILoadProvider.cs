using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public interface ILoadProvider
{
    T LoadAsset<T>(string assetName) where T : Object;
    IEnumerator LoadAssetAsync<T>(string assetName, Action<T> OnComplete) where T : Object;
    void UnloadAsset(string assetName);
    void LoadBundle(string bundleName);
    void UnloadBundle(string bundleName);
}
