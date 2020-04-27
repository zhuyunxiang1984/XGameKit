using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XAssetManager
{
    public abstract class XABEnvironment
    {
        public abstract bool EnableEncrypt { get; }
        public abstract string GetBundleFullPath(EnumLocation location, string bundleName);
        public abstract string GetStaticManifest();
        public abstract string GetHotfixManifest();
    }
}
