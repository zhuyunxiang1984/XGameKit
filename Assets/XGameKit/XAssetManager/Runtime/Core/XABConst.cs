using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XAssetManager
{
    public sealed class XABConst
    {
        public const string Tag = "XAssetBundle";

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static string DownloadPath = $"{Application.dataPath}/../Documents/Res";
#else
        public static string DownloadPath = $"{Application.persistentDataPath}/Documents/Res";
#endif
        public static string StreamingAssetsStaticPath = $"{Application.streamingAssetsPath}/Res/Static";
        public static string StreamingAssetsHotfixPath = $"{Application.streamingAssetsPath}/Res/Hotfix";
    }
}

