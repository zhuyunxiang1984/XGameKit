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
        //从文件读取
        public static byte[] ReadFile(string fullPath)
        {
            byte[] data = null;
            try
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                data = File.ReadAllBytes(fullPath);
#elif UNITY_ANDROID
                data = null;
                
#elif UNITY_IPHONE || UNITY_IOS
                data = null;
#endif
            }
            catch (Exception e)
            {
                XDebug.LogError(XABConst.Tag, $"ReadFile失败 {fullPath}\n{e.ToString()}");
            }
            return data;
        }

        public static XABManifest ReadManifest(string path)
        {
            var fullPath = path + "/manifest.json";
            var fileData = ReadFile(fullPath);
            if (fileData == null)
                return null;
            var fileText = Encoding.UTF8.GetString(fileData);
            return JsonUtility.FromJson<XABManifest>(fileText);
        }

        public static string GetBundleFullPath(string path, string bundleName)
        {
            return $"{path}/{bundleName}";
        }

        public static string AppendPath(this string path, EnumBundleType bundleType)
        {
            return $"{path}/{bundleType.ToString()}";
        }
        public static string AppendPath(this string path, EnumPlatform platform)
        {
            return $"{path}/{platform.ToString()}";
        }
        
#if UNITY_EDITOR
        
#endif
    }
}
