using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace XGameKit.XAssetManager
{
    #region 常量定义

    public sealed class XABConst
    {
        public const string Tag = "XAssetBundle";

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static string DocumentPath = $"{Application.dataPath}/../Documents/Res";
#else
        public static string DocumentPath = $"{Application.persistentDataPath}/Documents/Res";
#endif
        public static string StreamingAssetsPath = $"{Application.streamingAssetsPath}/Res";

#if UNITY_EDITOR

        //编译平台
        public const string EKResBuildPlatform = "EKResBuildPlatform";
        public const int EKResBuildPlatformValue = (int)EnumPlatform.Windows;
        
        //编译路径
        public const string EKResBuildPath = "EKResBuildPath";
        public static string EKResBuildPathValue = $"{Application.dataPath}/../AssetBundles";

        //运行平台
        public const string EKResRunPlatform = "EKResRunPlatform";
        public const int EKResRunPlatformValue = (int)EnumPlatform.Windows;

        //模式
        public const string EKResMode = "EKResMode";
        public const int EKResModeValue = (int)EnumResMode.Simulate;
        //路径
        public const string EKResPath = "EKResPath";
        public static string EKResPathValue = $"{Application.dataPath}/../AssetBundles";
        //网址
        public const string EKResUrl = "EKResUrl";
        public static string EKResUrlValue = $"file://{Application.dataPath}/../AssetBundles";

        //是否启用加密
        public const string EKResEnableEncrypt = "EKResEnableEncrypt";

        //加密秘钥
        public const string EKResEncryptKey = "EKResEncryptKey";

#endif
    }

    #endregion


    #region 枚举定义

    //平台定义
    public enum EnumPlatform
    {
        Windows,
        iOS,
        Android,
    }
    //文件定位
    public enum EnumFileLocation
    {
        Client, //可写目录
        Stream, //应用包内
    }
    
    //资源包类型
    public enum EnumBundleType
    {
        None = 0,
        Static,//跟包资源
        Hotfix,//热更资源
    }

    public enum EnumLoadState
    {
        None = 0, //未加载
        Loading, //加载中    
        Done, //已完成
    }

    public enum EnumResMode
    {
        Simulate = 0, //模拟模式
        Local,        //本地模式
        Remote,       //远程模式
        Release,      //发布模式
    }

    #endregion

    #region 其他定义

    //资源管理接口
    public interface IXAssetManager
    {
        void Initialize(string serverAddress = "");
        void Dispose();
        void Tick(float deltaTime);
        List<string> GetDependencies(string bundleName);
        string GetBundleNameByAssetName(string assetName);
        AssetBundle LoadBundle(string bundleName);
        void LoadBundleAsync(string bundleName, Action<string, AssetBundle> callback = null);
        void UnloadBundle(string bundleName);
        T LoadAsset<T>(string assetName) where T : Object;
        void LoadAssetAsync<T>(string assetName, Action<string, T> callback = null) where T : Object;
        void UnloadAsset(string assetName);
    }
    
    /// <summary>
    /// 自定义的清单文件
    /// 依赖关系
    /// 资源名对应包名关系
    /// </summary>
    [System.Serializable]
    public class XABManifest
    {
        [System.Serializable]
        public class ListWrapper
        {
            public List<string> values;
        }

        [System.Serializable]
        public class DictWrapper1 : XSerialDictionary<string, ListWrapper>
        {
        }

        [System.Serializable]
        public class DictWrapper2 : XSerialDictionary<string, string>
        {
        }

        //依赖数据
        [SerializeField] protected DictWrapper1 m_dictDependencies = new DictWrapper1();

        //资源名对应包名
        [SerializeField] protected DictWrapper2 m_dictAssetNameLinkBundleName = new DictWrapper2();

        public void Clear()
        {
            m_dictDependencies.Clear();
            m_dictAssetNameLinkBundleName.Clear();
        }

        public void SetDependency(string name, string[] array)
        {
            Debug.Log($"SetDependency {name} ");
            if (m_dictDependencies.ContainsKey(name))
            {
                m_dictDependencies[name].values.Clear();
                m_dictDependencies[name].values.AddRange(array);
            }
            else
            {
                m_dictDependencies.Add(name, new ListWrapper() {values = new List<string>(array)});
            }
        }

        public List<string> GetDependencies(string name)
        {
            if (m_dictDependencies.ContainsKey(name))
                return m_dictDependencies[name].values;
            return null;
        }

        public void SetAssetNameLink(string assetName, string bundleName)
        {
            if (m_dictAssetNameLinkBundleName.ContainsKey(assetName))
                return;
            m_dictAssetNameLinkBundleName.Add(assetName, bundleName);
        }

        public string GetBundleNameByAssetName(string assetName)
        {
            if (!m_dictAssetNameLinkBundleName.ContainsKey(assetName))
                return string.Empty;
            return m_dictAssetNameLinkBundleName[assetName];
        }

        public bool IsBundleExist(string bundleName)
        {
            return m_dictDependencies.ContainsKey(bundleName);
        }
        public bool IsAssetExist(string assetName)
        {
            return m_dictAssetNameLinkBundleName.ContainsKey(assetName);
        }
    }
    
    #endregion
}