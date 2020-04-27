using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XAssetManager
{
    public abstract class XABLoader
    {
        public abstract AssetBundle Load(string fullPath);
        public abstract void LoadAsync(string fullPath);
        public abstract void StopAsync();
        public abstract void Tick(float deltaTime);
        public abstract bool IsDone { get; }
        public abstract AssetBundle GetValue();
    }

    

}
