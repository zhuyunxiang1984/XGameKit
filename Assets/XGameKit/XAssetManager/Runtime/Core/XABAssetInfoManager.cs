using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    public class XABAssetInfoManager
    {
        //下载列表
        public List<string> ltDownload  { get; protected set; } = new List<string>();
        //删除列表
        public List<string> ltExpiredX  { get; protected set; } = new List<string>();

        protected XABManifest m_staticManifest;

        protected XABManifest m_hotfixManifest;
        //资源名->包名
        protected Dictionary<string, string> m_dictAssetNameToBundleName = new Dictionary<string, string>();
        //依赖关系
        public class BundleInfo
        {
            public List<string> dependencies;
            public EnumBundleType bundleType;
        }
        protected Dictionary<string, BundleInfo> m_dictBundles = new Dictionary<string, BundleInfo>();

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
            var datas = m_staticManifest.GetAssetNameToBundleNameDatas();
            foreach (var pairs in datas)
            {
                if (m_dictAssetNameToBundleName.ContainsKey(pairs.Key))
                {
                    Debug.LogError($"资源名重复 {pairs.Key}");
                    continue;
                }
                m_dictAssetNameToBundleName.Add(pairs.Key, pairs.Value);
            }

            var dependencyDatas = m_staticManifest.GetDependencyDatas();
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