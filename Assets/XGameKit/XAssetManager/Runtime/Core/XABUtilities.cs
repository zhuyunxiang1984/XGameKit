using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using XGameKit.Core;
using Object = System.Object;
#if UNITY_EDITOR
using UnityEditor;       
#endif

namespace XGameKit.XAssetManager
{
    public static class XABUtilities
    {
        public static bool ExistManifest(string path)
        {
            return XUtilities.ExistFile(path + "/manifest.json");
        }

        public static XABManifest ReadManifest(string path)
        {
            var fullPath = path + "/manifest.json";
            var fileData = XUtilities.ReadFile(fullPath);
            if (fileData == null)
                return null;
            var fileText = Encoding.UTF8.GetString(fileData);
            return JsonUtility.FromJson<XABManifest>(fileText);
        }
#if UNITY_EDITOR
        //获取资源路径(编辑器设置)
        public static string GetResPathEditSet()
        {
            var path = EditorPrefs.GetString(XABConst.EKResPath, XABConst.EKResPathValue);
            var platform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResRunPlatform, XABConst.EKResRunPlatformValue);
            return $"{path}/{platform.ToString()}";
        }
#endif
        //获取资源路径
        public static string GetResPath(EnumFileLocation location, EnumBundleType bundleType)
        {
#if UNITY_EDITOR
            var mode = (EnumResMode) EditorPrefs.GetInt(XABConst.EKResMode, XABConst.EKResModeValue);
            //模拟模式
            if (mode == EnumResMode.Simulate)
            {
                return string.Empty;
            }
            //本地模式
            if (mode == EnumResMode.Local)
            {
                return $"{GetResPathEditSet()}/{bundleType.ToString()}";
            }
            //远程模式
            if (mode == EnumResMode.Remote)
            {
                var path = string.Empty;
                switch (bundleType)
                {
                    case EnumBundleType.Static:
                        path = GetResPathEditSet();
                        break;
                    case EnumBundleType.Hotfix:
                        path = XABConst.DocumentPath;
                        break;
                }
                return $"{path}/{bundleType.ToString()}";;
            }
#endif
            //发布模式
            if (location == EnumFileLocation.Client)
            {
                return $"{XABConst.DocumentPath}/{bundleType.ToString()}";
            }
            if (location == EnumFileLocation.Stream)
            {
#if UNITY_EDITOR
                return $"{GetResPathEditSet()}/{bundleType.ToString()}";
#endif
                return $"{XABConst.StreamingAssetsPath}/{bundleType.ToString()}";
            }
            return string.Empty;
        }
        //获取资源包完整路径
        public static string GetBundleFullPath(EnumFileLocation location, EnumBundleType bundleType, string bundleName)
        {
            return $"{GetResPath(location, bundleType)}/{bundleName}";
        }

    }
}
