using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    public static class XABUtilities
    {
        //获取路径
        public static string GetAssetBundleFullPath(EnumLocation location, string name)
        {
            var fullPath = string.Empty;
            switch (location)
            {
                case EnumLocation.Download:
                    fullPath = $"{XABConst.DownloadPath}/{name}";
                    break;
                case EnumLocation.StreamingAssetsHotfix:
                    fullPath = $"{XABConst.StreamingAssetsHotfixPath}/{name}";
                    break;
                case EnumLocation.StreamingAssetsStatic:
                    fullPath = $"{XABConst.StreamingAssetsStaticPath}/{name}";
                    break;
            }
            return fullPath;
        }
        
        //加载AssetBundle(同步)
        public static AssetBundle LoadAssetBundle(string fullPath)
        {
            try
            {
                byte[] data = ReadFile(fullPath);
                //解密
                //读取AssetBundle
                return AssetBundle.LoadFromMemory(data);
            }
            catch (Exception e)
            {
                XDebug.Log(XABConst.Tag, $"加载assetbundle失败 {fullPath}\n{e.ToString()}");
            }
            return null;
        }

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
    }
}
