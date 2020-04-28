﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        
        //路径环境
        protected XABDirEnv m_DirEnv;
        //清单数据
        protected XABManifest m_staticManifest;
        protected XABManifest m_hotfixManifest;
        //

        public XAssetManagerOrdinary()
        {
#if UNITY_EDITOR
            var mode = (EnumResMode) UnityEditor.EditorPrefs.GetInt(XABConst.EKResMode, XABConst.EKResModeDefaultValue);
            if (mode == EnumResMode.Local)
            {
                m_DirEnv = new XABDirEnvLocal();
            }
            else if (mode == EnumResMode.Remote)
            {
                m_DirEnv = new XABDirEnvRemote();
            }
            else
#endif
            {
                m_DirEnv = new XABDirEnvEmpty();
            }
            m_staticManifest = XABUtilities.ReadManifest(m_DirEnv.GetStaticAssetsPath());
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
        public EnumBundleType GetBundleType(string bundleName)
        {
            if (m_staticManifest != null && m_staticManifest.IsBundleExist(bundleName))
                return EnumBundleType.Static;
            if (m_hotfixManifest != null && m_hotfixManifest.IsBundleExist(bundleName))
                return EnumBundleType.Hotfix;
            Debug.LogError($"不存在资源包！！ {bundleName} ");
            return EnumBundleType.None;
        }

        public EnumBundleType GetAssetType(string assetName)
        {
            if (m_staticManifest != null && m_staticManifest.IsAssetExist(assetName))
                return EnumBundleType.Static;
            if (m_hotfixManifest != null && m_hotfixManifest.IsAssetExist(assetName))
                return EnumBundleType.Hotfix;
            Debug.LogError($"不存在资源！！ {assetName} ");
            return EnumBundleType.None;
        }
        public List<string> GetDependencies(string bundleName)
        {
            var bundleType = GetBundleType(bundleName);
            if (bundleType == EnumBundleType.None)
                return null;
            if (bundleType == EnumBundleType.Static)
                return m_staticManifest.GetDependencies(bundleName);
            if (bundleType == EnumBundleType.Hotfix)
                return m_hotfixManifest.GetDependencies(bundleName);
            return null;
        }
        public string GetBundleNameByAssetName(string assetName)
        {
            var bundleType = GetAssetType(assetName);
            if (bundleType == EnumBundleType.None)
                return null;
            if (bundleType == EnumBundleType.Static)
                return m_staticManifest.GetBundleNameByAssetName(assetName);
            if (bundleType == EnumBundleType.Hotfix)
                return m_hotfixManifest.GetBundleNameByAssetName(assetName);
            return string.Empty;
        }
        
        //同步加载
        public AssetBundle LoadBundle(string bundleName)
        {
            bundleName = bundleName.ToLower();
            var bundleType = GetBundleType(bundleName);
            if (bundleType == EnumBundleType.None)
                return null;
            var obj = _GetOrCreateAB(bundleName);
            obj.Retain();
            if (obj.State == EnumLoadState.Done)
            {
                return obj.GetValue();
            }
            if (obj.State == EnumLoadState.Loading)
            {
                //正在执行异步加载，那么停止异步加载，直接同步加载
                obj.StopAsync();
            }
            //这里会通过其他数据获取location类型
            obj.Load(this, m_DirEnv.GetBundlePath(EnumFileLocation.Document, bundleType), bundleName);
            return obj.GetValue();
        }
        //异步加载
        public void LoadBundleAsync(string bundleName, Action<string, AssetBundle> callback = null)
        {
            bundleName = bundleName.ToLower();
            var bundleType = GetBundleType(bundleName);
            if (bundleType == EnumBundleType.None)
            {
                callback?.Invoke(bundleName, null);
                return;
            }
                
            var obj = _GetOrCreateAB(bundleName);
            obj.Retain();
            if (obj.State == EnumLoadState.Done)
            {
                callback?.Invoke(bundleName, obj.GetValue());
                return;
            }
            if (obj.State == EnumLoadState.Loading)
            {
                obj.AddCallback(callback);
                return;
            }
            obj.LoadAsync(this, m_DirEnv.GetBundlePath(EnumFileLocation.Document, bundleType), bundleName, callback);
        }
        //卸载
        public void UnloadBundle(string bundleName)
        {
            bundleName = bundleName.ToLower();
            if (!m_dictAssetBundles.ContainsKey(bundleName))
                return;
            m_dictAssetBundles[bundleName].Release();
        }
        //同步加载
        public T LoadAsset<T>(string assetName) where T : Object
        {
            assetName = assetName.ToLower();
            var assetType = GetAssetType(assetName);
            if (assetType == EnumBundleType.None)
            {
                return null;
            }
            
            var obj = _GetOrCreateAO(assetName);
            obj.Retain();
            if (obj.State == EnumLoadState.Done)
            {
                return obj.GetValue<T>();
            }
            if (obj.State == EnumLoadState.Loading)
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
            assetName = assetName.ToLower();
            var assetType = GetAssetType(assetName);
            if (assetType == EnumBundleType.None)
            {
                callback?.Invoke(assetName, null);
                return;
            }
            var obj = _GetOrCreateAO(assetName);
            obj.Retain();
            if (obj.State == EnumLoadState.Done)
            {
                callback?.Invoke(assetName, obj.GetValue<T>());
                return;
            }
            if (obj.State == EnumLoadState.Loading)
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
