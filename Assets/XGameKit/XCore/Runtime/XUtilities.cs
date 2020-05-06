using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XGameKit.Core
{
    public static class XUtilities
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
                Debug.LogError($"ReadFile 失败 {fullPath}\n{e.ToString()}");
            }
            return data;
        }

        //写入文件
        public static bool SaveFile(string fullPath, string content)
        {
            try
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                File.WriteAllText(fullPath, content);
#elif UNITY_ANDROID
                                
#elif UNITY_IPHONE || UNITY_IOS

#endif
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveFile 失败 {fullPath}\n{e.ToString()}");
            }
            return false;
        }
        
        //是否是文件夹
        public static bool IsFolder(string fullPath)
        {
            return string.IsNullOrEmpty(Path.GetExtension(fullPath));
        }
        //获取路径
        public static string GetDirName(string fullPath)
        {
            if (IsFolder(fullPath))
                return fullPath;
            return Path.GetDirectoryName(fullPath);
        }
        //保证路径存在
        public static void MakePathExist(string fullPath)
        {
            var dir = GetDirName(fullPath);
            if (Directory.Exists(dir))
                return;
            Directory.CreateDirectory(dir);
        }
        //删除文件夹
        public static void DeleteFolder(string fullPath)
        {
            if(!IsFolder(fullPath))
            {
                Debug.Log($"{fullPath} 不是文件夹！");
                return;
            }
            string dir = GetDirName(fullPath);
            if (!Directory.Exists(dir))
                return;
            Directory.Delete(dir, true);
        }
        //清空文件夹，但是不删除
        public static void CleanFolder(string fullPath)
        {
            if(!IsFolder(fullPath))
            {
                Debug.Log($"{fullPath} 不是文件夹！");
                return;
            }
            string dir = GetDirName(fullPath);
            if (!Directory.Exists(dir))
                return;
            var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                File.Delete(file);
            }
            var paths = Directory.GetDirectories(dir);
            foreach (var path in paths)
            {
                Directory.Delete(path);
            }
        }
        
#if UNITY_EDITOR
        //获取或创建一个配置
        public static T GetOrCreateConfig<T>(string assetPath) where T : ScriptableObject
        {
            var config = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (config != null)
                return config;
            config = ScriptableObject.CreateInstance<T>();
            XUtilities.MakePathExist(assetPath);
            AssetDatabase.CreateAsset(config, assetPath);
            return config;
        }
#endif
    }
}
