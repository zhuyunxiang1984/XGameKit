using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    #region 常量定义

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

#if UNITY_EDITOR

        //编译平台
        public const string BuildTargetKey = "BuildTargetKey";

        //运行平台
        public const string EditorRunTargetKey = "RunXXTargetKey";

        //模式
        public const string EditorRunModeKey = "EditorRunModeKey";

        //路径
        public const string EditorRunPathKey = "EditorRunPathKey";

        //网址
        public const string EditorRunUrlKey = "EditorRunUrlKey";

        //是否启用加密
        public const string EditorRunEnableEncryptKey = "EditorRunEnableEncryptKey";

        //加密秘钥
        public const string EditorRunEncryptKey = "EditorRunEncryptKey";


#endif
    }

    #endregion


    #region 枚举定义

    public enum EnumLocation
    {
        Download, //下载目录
        StreamingAssetsStatic, //StreamingAssets跟包目录
        StreamingAssetsHotfix, //StreamingAssets热更目录
    }

    public enum EnumLoadState
    {
        None = 0, //未加载
        Loading, //加载中    
        Done, //已完成
    }

#if UNITY_EDITOR

    public enum EnumEditorRunMode
    {
        Simulate, //本地模拟
        Local, //本地资源
        Remote, //远程资源
    }

#endif

    #endregion

    #region 其他定义

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

        public List<string> GetDependency(string name)
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
    }

    
    /*
     * 启动参数
     */
    public class XAssetManagerParam
    {
#if UNITY_EDITOR
        //模式
        public EnumEditorRunMode mode;
#endif
        //本地路径
        public string localAddress;
        //远程网址
        public string remoteAddress;
        //是否加密
        public bool enableEncrypt;
        //秘钥
        public string encryptKey;

    }
    
    #endregion
}