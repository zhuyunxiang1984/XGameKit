using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XGameKit.Core;
using Object = UnityEngine.Object;

namespace XGameKit.XAssetManager
{
    public class XAssetManager : IXService
    {
        //AssetBundle数据
        protected Dictionary<string, XAssetBundle> m_dictAssetBundles = new Dictionary<string, XAssetBundle>();
        //AssetObject数据
        protected Dictionary<string, XAssetObject> m_dictAssetObjects = new Dictionary<string, XAssetObject>();
        
        protected List<string> m_listDestroyAssetBundles = new List<string>();
        protected List<string> m_listDestroyAssetObjects = new List<string>();
        
        //是否存在于document下
        protected Dictionary<string, bool> m_dictAssetBundleFlags = new Dictionary<string, bool>();
        
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
                if (assetBundle.RefCount == 0 && assetBundle.RefZeroTime > 5)
                {
                    m_listDestroyAssetBundles.Add(assetBundle.Name);
                }
            }
            foreach (var assetObject in m_dictAssetObjects.Values)
            {
                assetObject.Tick(deltaTime);
                if (assetObject.RefCount == 0 && assetObject.RefZeroTime > 5)
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

        //获取依赖项
        public List<string> GetDependents(string name)
        {
            return null;
        }
        //获取包名
        public string GetAssetBundleName(string assetName)
        {
            return string.Empty;
        }
        //获取标记
        public bool GetFlag(string name)
        {
            if (m_dictAssetBundleFlags.ContainsKey(name))
                return m_dictAssetBundleFlags[name];
            return false;
        }

        
        
        #region AssetObject

        XAssetObject _GetOrCreateAO(string name)
        {
            if (m_dictAssetObjects.ContainsKey(name))
                return m_dictAssetObjects[name];
            var obj = new XAssetObject();
            m_dictAssetObjects.Add(name, obj);
            return obj;
        }
        
        //同步加载
        public T LoadAsset<T>(string name) where T : Object
        {
            var obj = _GetOrCreateAO(name);
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
            obj.Load<T>(this, name);
            return obj.GetValue<T>();
        }
        //异步加载
        public void LoadAssetAsyn<T>(string name, Action<string, T> callback) where T : Object
        {
            var obj = _GetOrCreateAO(name);
            obj.Retain();
            if (obj.State == EnumLoadState.Done)
            {
                callback?.Invoke(name, obj.GetValue<T>());
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
            obj.LoadAsync<T>(this, name, (p1, p2) =>
            {
                callback?.Invoke(p1, p2 as T);
            });
        }
        //卸载
        public void UnloadAsset(string name)
        {
            if (!m_dictAssetObjects.ContainsKey(name))
                return;
            m_dictAssetObjects[name].Release();
        }
        
        #endregion
        
        
        #region AssetBundle

        XAssetBundle _GetOrCreateAB(string name)
        {
            if (m_dictAssetBundles.ContainsKey(name))
                return m_dictAssetBundles[name];
            var obj = new XAssetBundle();
            m_dictAssetBundles.Add(name, obj);
            return obj;
        }
        
        //同步加载
        public AssetBundle LoadBundle(string name)
        {
            var obj = _GetOrCreateAB(name);
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
            obj.Load(this, EnumLocation.Download, name);
            return obj.GetValue();
        }
        //异步加载
        public void LoadBundleAsync(string name, Action<string, AssetBundle> callback = null)
        {
            var obj = _GetOrCreateAB(name);
            obj.Retain();
            if (obj.State == EnumLoadState.Done)
            {
                callback?.Invoke(name, obj.GetValue());
                return;
            }
            if (obj.State == EnumLoadState.Loading)
            {
                obj.AddCallback(callback);
                return;
            }
            obj.LoadAsync(this, EnumLocation.Download, name, callback);
        }
        //卸载
        public void UnloadBundle(string name)
        {
            if (!m_dictAssetBundles.ContainsKey(name))
                return;
            m_dictAssetBundles[name].Release();
        }
        
        #endregion
    }
}

