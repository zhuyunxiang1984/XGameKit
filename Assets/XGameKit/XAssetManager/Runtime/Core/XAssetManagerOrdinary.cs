using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XGameKit.Core;
using Object = UnityEngine.Object;

namespace XGameKit.XAssetManager
{
    public class XAssetManagerOrdinary : IXAssetManager
    {
        //AssetBundle数据
        protected Dictionary<string, XAssetBundle> m_dictAssetBundles = new Dictionary<string, XAssetBundle>();
        //AssetObject数据
        protected Dictionary<string, XAssetObject> m_dictAssetObjects = new Dictionary<string, XAssetObject>();
        
        protected List<string> m_listDestroyAssetBundles = new List<string>();
        protected List<string> m_listDestroyAssetObjects = new List<string>();
        
        //状态
        public EnumJobState CheckHotfixState { get; protected set; }
        public EnumJobState DownloadHotfixState { get; protected set; }
        
        //
        public XABAssetInfoManager AssetInfoManager
        {
            get { return m_AssetInfoManager; }
        }

        //清单数据
        protected XABManifest m_staticManifest;
        protected XABManifest m_hotfixManifest;
        
        //资源信息管理
        protected XABAssetInfoManager m_AssetInfoManager = new XABAssetInfoManager();
        protected XABHotfixTaskSchedule m_InitTaskSchedule = new XABHotfixTaskSchedule();
        public XAssetManagerOrdinary()
        {
            m_AssetInfoManager.SetStaticManifest(XABUtilities.ReadManifest(XABUtilities.GetResPath(EnumFileLocation.Stream, EnumBundleType.Static)));

        }
        public string serverAddress { get; protected set; }
        public void Initialize(string serverAddress)
        {
            this.serverAddress = serverAddress;
            
            m_InitTaskSchedule.Start(this, delegate(bool flag) {
                Debug.Log("complete " + flag);
            });
        }

        public void Dispose()
        {
            foreach (var assetBundle in m_dictAssetBundles.Values)
            {
                assetBundle.Dispose();
            }
            m_dictAssetBundles.Clear();
            
            foreach (var assetObject in m_dictAssetObjects.Values)
            {
                assetObject.Dispose();
            }
            m_dictAssetObjects.Clear();
        }

        public void Tick(float deltaTime)
        {
            m_InitTaskSchedule.Update(deltaTime);
            foreach (var assetBundle in m_dictAssetBundles.Values)
            {
                assetBundle.Tick(deltaTime);
                if (assetBundle.CanDestroy())
                {
                    m_listDestroyAssetBundles.Add(assetBundle.Name);
                }
            }
            foreach (var assetObject in m_dictAssetObjects.Values)
            {
                assetObject.Tick(deltaTime);
                if (assetObject.CanDestroy())
                {
                    m_listDestroyAssetObjects.Add(assetObject.Name);
                }
            }

            if (m_listDestroyAssetBundles.Count > 0)
            {
                foreach (var name in m_listDestroyAssetBundles)
                {
                    m_dictAssetBundles[name].Dispose();
                    m_dictAssetBundles.Remove(name);
                }
                m_listDestroyAssetBundles.Clear();
            }
            if (m_listDestroyAssetObjects.Count > 0)
            {
                foreach (var name in m_listDestroyAssetObjects)
                {
                    m_dictAssetObjects[name].Dispose();
                    m_dictAssetObjects.Remove(name);
                }
                m_listDestroyAssetObjects.Clear();
            }
        }
        public List<string> GetDependencies(string bundleName)
        {
            var bundleInfo = AssetInfoManager.GetBundleInfo(bundleName);
            if (bundleInfo == null)
                return null;
            return bundleInfo.dependencies;
        }
        public string GetBundleNameByAssetName(string assetName)
        {
            return AssetInfoManager.GetBundleNameByAssetName(assetName);
        }
        
        //同步加载
        public AssetBundle LoadBundle(string bundleName)
        {
            XDebug.Log(XABConst.Tag, $"--加载AssetBundle(同步) {bundleName}");
            bundleName = bundleName.ToLower();
            var bundleInfo = AssetInfoManager.GetBundleInfo(bundleName);
            if (bundleInfo == null)
                return null;
            var obj = _GetOrCreateAB(bundleName);
            obj.Retain();
            if (obj.State == EnumJobState.Done)
            {
                return obj.GetValue();
            }
            if (obj.State == EnumJobState.Process)
            {
                //正在执行异步加载，那么停止异步加载，直接同步加载
                obj.StopAsync();
            }
            //这里会通过其他数据获取location类型
            obj.Load(this, EnumFileLocation.Client, bundleInfo.bundleType, bundleName);
            return obj.GetValue();
        }
        //异步加载
        public void LoadBundleAsync(string bundleName, Action<string, AssetBundle> callback = null)
        {
            XDebug.Log(XABConst.Tag, $"--加载AssetBundle(异步) {bundleName}");
            bundleName = bundleName.ToLower();
            var bundleInfo = AssetInfoManager.GetBundleInfo(bundleName);
            if (bundleInfo == null)
            {
                callback?.Invoke(bundleName, null);
                return;
            }
                
            var obj = _GetOrCreateAB(bundleName);
            obj.Retain();
            if (obj.State == EnumJobState.Done)
            {
                callback?.Invoke(bundleName, obj.GetValue());
                return;
            }
            if (obj.State == EnumJobState.Process)
            {
                obj.AddCallback(callback);
                return;
            }
            obj.LoadAsync(this, EnumFileLocation.Client, bundleInfo.bundleType, bundleName, callback);
        }
        //卸载
        public void UnloadBundle(string bundleName)
        {
            XDebug.Log(XABConst.Tag, $"--卸载AssetBundle {bundleName}");
            bundleName = bundleName.ToLower();
            if (!m_dictAssetBundles.ContainsKey(bundleName))
                return;
            m_dictAssetBundles[bundleName].Release();
        }
        //同步加载
        public T LoadAsset<T>(string assetName) where T : Object
        {
            XDebug.Log(XABConst.Tag, $"--加载AssetObject(同步) {assetName}");
            assetName = assetName.ToLower();
            var bundleName = AssetInfoManager.GetBundleNameByAssetName(assetName);
            if (string.IsNullOrEmpty(bundleName))
            {
                return null;
            }
            
            var obj = _GetOrCreateAO(assetName);
            obj.Retain();
            if (obj.State == EnumJobState.Done)
            {
                return obj.GetValue<T>();
            }
            if (obj.State == EnumJobState.Process)
            {
                //正在执行异步加载，那么停止异步加载，直接同步加载
                obj.StopAsync();
            }
            obj.Load<T>(this, assetName);
            return obj.GetValue<T>();
        }
        //异步加载
        public void LoadAssetAsync<T>(string assetName, Action<string, T> callback = null) where T : Object
        {
            XDebug.Log(XABConst.Tag, $"--加载AssetObject(异步) {assetName}");
            assetName = assetName.ToLower();
            var bundleName = AssetInfoManager.GetBundleNameByAssetName(assetName);
            if (string.IsNullOrEmpty(bundleName))
            {
                callback?.Invoke(assetName, null);
                return;
            }
            var obj = _GetOrCreateAO(assetName);
            obj.Retain();
            if (obj.State == EnumJobState.Done)
            {
                callback?.Invoke(assetName, obj.GetValue<T>());
                return;
            }
            if (obj.State == EnumJobState.Process)
            {
                obj.AddCallback((p1, p2) =>
                {
                    callback?.Invoke(p1, p2 as T);
                });
                return;
            }
            obj.LoadAsync<T>(this, assetName, (p1, p2) =>
            {
                callback?.Invoke(p1, p2 as T);
            });
        }
        //卸载
        public void UnloadAsset(string assetName)
        {
            XDebug.Log(XABConst.Tag, $"--卸载AssetObject {assetName}");
            assetName = assetName.ToLower();
            if (!m_dictAssetObjects.ContainsKey(assetName))
                return;
            m_dictAssetObjects[assetName].Release();
        }

        XAssetObject _GetOrCreateAO(string name)
        {
            if (m_dictAssetObjects.ContainsKey(name))
                return m_dictAssetObjects[name];
            var obj = new XAssetObject();
            m_dictAssetObjects.Add(name, obj);
            return obj;
        }
        XAssetBundle _GetOrCreateAB(string name)
        {
            if (m_dictAssetBundles.ContainsKey(name))
                return m_dictAssetBundles[name];
            var obj = new XAssetBundle();
            m_dictAssetBundles.Add(name, obj);
            return obj;
        }
    
    }
}

