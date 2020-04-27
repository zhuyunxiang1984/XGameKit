using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XAssetManager
{
    public class XABLoaderSimulate : XABLoader
    {
        public override bool IsDone
        {
            get { return true; }
        }
        public override AssetBundle GetValue()
        {
            return null;
        }

        public override AssetBundle Load(string fullPath)
        {
            return null;
        }

        public override void LoadAsync(string fullPath)
        {
        }

        public override void StopAsync()
        {
        }

        public override void Tick(float deltaTime)
        {
        }
    }
}
