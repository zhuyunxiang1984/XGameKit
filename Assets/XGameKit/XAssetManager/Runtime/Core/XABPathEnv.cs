using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XGameKit.XAssetManager
{
    public abstract class XABPathEnv
    {
        public abstract string GetPath(EnumFileLocation fileLocation, EnumBundleType bundleType);
    }
#if UNITY_EDITOR
    
    //本地资源模式
    public class XABPathEnvLocal : XABPathEnv
    { 
        public override string GetPath(EnumFileLocation fileLocation, EnumBundleType bundleType)
        {
            var platform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResBuildPlatform, XABConst.EKResBuildPlatformDefaultValue);
            var path = EditorPrefs.GetString(XABConst.EKResPath, XABConst.EKResPathDefaultValue);
            return path.AppendPath(platform).AppendPath(bundleType);
        }
    }
    //远程资源模式
    public class XABPathEnvRemote : XABPathEnv
    {
        public override string GetPath(EnumFileLocation fileLocation, EnumBundleType bundleType)
        {
            var platform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResBuildPlatform, XABConst.EKResBuildPlatformDefaultValue);
            var path = EditorPrefs.GetString(XABConst.EKResPath, XABConst.EKResPathDefaultValue);
            if (bundleType == EnumBundleType.Hotfix)
            {
                return path.AppendPath(platform).AppendPath(bundleType) + "Remote";
            }
            return path.AppendPath(platform).AppendPath(bundleType);
        }
    }
#endif

    public class XABPathEnvRelease : XABPathEnv
    {
        public override string GetPath(EnumFileLocation fileLocation, EnumBundleType bundleType)
        {
            var path = string.Empty;
            switch (fileLocation)
            {
                case EnumFileLocation.Client:
                    path = XABConst.DocumentPath;
                    break;
                case EnumFileLocation.Stream:
                    path = XABConst.StreamingAssetsPath;
                    break;
            }
            return path.AppendPath(bundleType);
        }
    }
    
}
