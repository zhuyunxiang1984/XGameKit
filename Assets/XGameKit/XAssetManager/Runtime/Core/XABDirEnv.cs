using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XGameKit.XAssetManager
{
    public abstract class XABDirEnv
    {
        public abstract string GetStaticAssetsPath();
        public abstract string GetHotfixAssetsPath();
        public abstract string GetBundlePath(EnumFileLocation fileLocation, EnumBundleType bundleType);
    }
    public class XABDirEnvEmpty : XABDirEnv
    {
        public override string GetStaticAssetsPath()
        {
            return string.Empty;
        }
        public override string GetHotfixAssetsPath()
        {
            return string.Empty;
        }
        public override string GetBundlePath(EnumFileLocation fileLocation, EnumBundleType bundleType)
        {
            return string.Empty;
        }
    }
#if UNITY_EDITOR
    
    //本地资源模式
    public class XABDirEnvLocal : XABDirEnv
    {
        public override string GetStaticAssetsPath()
        {
            var platform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResBuildPlatform, XABConst.EKResBuildPlatformDefaultValue);
            var path = EditorPrefs.GetString(XABConst.EKResPath, XABConst.EKResPathDefaultValue);
            return path.AppendPath(platform).AppendPath(EnumBundleType.Static);
        }

        public override string GetHotfixAssetsPath()
        {
            var platform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResBuildPlatform, XABConst.EKResBuildPlatformDefaultValue);
            var path = EditorPrefs.GetString(XABConst.EKResPath, XABConst.EKResPathDefaultValue);
            return path.AppendPath(platform).AppendPath(EnumBundleType.Hotfix);
        }

        public override string GetBundlePath(EnumFileLocation fileLocation, EnumBundleType bundleType)
        {
            var platform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResBuildPlatform, XABConst.EKResBuildPlatformDefaultValue);
            var path = EditorPrefs.GetString(XABConst.EKResPath, XABConst.EKResPathDefaultValue);
            return path.AppendPath(platform).AppendPath(bundleType);
        }
    }
    //远程资源模式
    public class XABDirEnvRemote : XABDirEnv
    {
        public override string GetStaticAssetsPath()
        {
            var platform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResBuildPlatform, XABConst.EKResBuildPlatformDefaultValue);
            var path = EditorPrefs.GetString(XABConst.EKResDownloadPath, XABConst.EKResDownloadPathDefaultValue);
            return path.AppendPath(platform).AppendPath(EnumBundleType.Static);
        }
        public override string GetHotfixAssetsPath()
        {
            var platform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResBuildPlatform, XABConst.EKResBuildPlatformDefaultValue);
            var path = EditorPrefs.GetString(XABConst.EKResDownloadPath, XABConst.EKResDownloadPathDefaultValue);
            return path.AppendPath(platform).AppendPath(EnumBundleType.Hotfix);
        }
        public override string GetBundlePath(EnumFileLocation fileLocation, EnumBundleType bundleType)
        {
            var platform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResBuildPlatform, XABConst.EKResBuildPlatformDefaultValue);
            var path = EditorPrefs.GetString(XABConst.EKResDownloadPath, XABConst.EKResDownloadPathDefaultValue);
            return path.AppendPath(platform).AppendPath(bundleType);
        }
    }
#endif
    
}
