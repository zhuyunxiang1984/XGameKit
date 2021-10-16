using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

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

    public void LoadBundle(string bundleName)
    {

    }

    public void UnloadBundle(string bundleName)
    {

    }
}