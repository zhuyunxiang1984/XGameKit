using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager.Backup
{
    public class XABAssetInfoManager
    {
        //更新信息
        public class HotfixInfo
        {
            public XFileList client_filelist;
            public XFileList server_filelist;
            public XFileList origin_filelist;
            public long download_size;
            public List<string> download_list = new List<string>();
            public List<string> deleteXX_list = new List<string>();

            public void Reset()
            {
                client_filelist = null;
                download_size = 0L;
                download_list.Clear();
                deleteXX_list.Clear();
            }

            public string ToLog()
            {
                var text = string.Empty;
                text += "===比较文件清单===\n";
                text += "删除列表\n";
                foreach (var temp in deleteXX_list)
                {
                    text += $"{temp}\n";
                }
                text += $"下载列表 下载总量:{download_size}\n";
                foreach (var temp in download_list)
                {
                    text += $"{temp}\n";
                }
                return text;
            }
        }
        public HotfixInfo hotfixInfo { get; protected set; } = new HotfixInfo();

        protected XABManifest m_staticManifest;

        protected XABManifest m_hotfixManifest;
        //资源名->包名
        protected Dictionary<string, string> m_dictAssetNameToBundleName = new Dictionary<string, string>();
        //资源包信息（依赖项，类型）
        public class BundleInfo
        {
            public List<string> dependencies;
            public EnumBundleType bundleType;
        }
        protected Dictionary<string, BundleInfo> m_dictBundles = new Dictionary<string, BundleInfo>();
        //资源包定位
        protected Dictionary<string, EnumFileLocation> m_dictBundleLocations = new Dictionary<string, EnumFileLocation>();

        public void ClearLocation()
        {
            m_dictBundleLocations.Clear();
        }
        public void SetLocation(string bundleName, EnumFileLocation location)
        {
            if (m_dictBundleLocations.ContainsKey(bundleName))
                m_dictBundleLocations[bundleName] = location;
            else
                m_dictBundleLocations.Add(bundleName, location);
        }
        public void SetStaticManifest(XABManifest manifest)
        {
            m_staticManifest = manifest;
            _UpdateManifestData();
        }
        public void SetHotfixManifest(XABManifest manifest)
        {
            m_hotfixManifest = manifest;
            _UpdateManifestData();
        }
        public string GetBundleNameByAssetName(string assetName)
        {
            if (m_dictAssetNameToBundleName.ContainsKey(assetName))
                return m_dictAssetNameToBundleName[assetName];
            XDebug.LogError(XABConst.Tag, $"无法找到该资源对应的包名 {assetName}");
            return string.Empty;
        }
        public BundleInfo GetBundleInfo(string bundleName)
        {
            if (m_dictBundles.ContainsKey(bundleName))
                return m_dictBundles[bundleName];
            return null;
        }
        protected void _UpdateManifestData()
        {
            m_dictAssetNameToBundleName.Clear();
            m_dictBundles.Clear();
            
            _CollectManifestData(m_staticManifest, EnumBundleType.Static);
            _CollectManifestData(m_hotfixManifest, EnumBundleType.Hotfix);
        }

        void _CollectManifestData(XABManifest manifest, EnumBundleType bundleType)
        {
            if (manifest == null)
                return;
            var datas = manifest.GetAssetNameToBundleNameDatas();
            foreach (var pairs in datas)
            {
                if (m_dictAssetNameToBundleName.ContainsKey(pairs.Key))
                {
                    Debug.LogError($"资源名重复 {pairs.Key}");
                    continue;
                }
                m_dictAssetNameToBundleName.Add(pairs.Key, pairs.Value);
            }

            var dependencyDatas = manifest.GetDependencyDatas();
            foreach (var pairs in dependencyDatas)
            {
                if (m_dictBundles.ContainsKey(pairs.Key))
                {
                    Debug.LogError($"包名重复 {pairs.Key}");
                    continue;
                }
                var bundleInfo = new BundleInfo();
                bundleInfo.dependencies = pairs.Value.values;
                bundleInfo.bundleType = bundleType;
                m_dictBundles.Add(pairs.Key, bundleInfo);
            }
        }
    }
}