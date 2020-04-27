using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XAssetManager
{
    public abstract class XAOLoader
    {
        public abstract T Load<T>(AssetBundle assetBundle, string assetName) where T : Object;
        public abstract void LoadAsync<T>(AssetBundle assetBundle, string assetName) where T : Object;
        public abstract void StopAsync();
        public abstract void Tick(float deltaTime);
        public abstract bool IsDone { get; }
        public abstract Object GetValue();
    }

}
