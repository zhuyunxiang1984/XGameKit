using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using Object = UnityEngine.Object;

namespace XGameKit.XAssetManager.Backup
{
    public class XAssetObject
    {
        protected IXAssetManager m_AssetManger;
        protected string m_BundleName;
        protected string m_AssetXName;
        protected EnumJobState m_state;
        protected Object m_AssetObject;
        protected Action<string, Object> m_callback;
        protected int m_RefCount;
        protected float m_CacheTime = 5f;
        protected float m_CacheTimeCounter;
        protected AssetBundleRequest m_LoadAssetRequest;
        
        public EnumJobState State
        {
            get{return m_state;}
        }
        public void Retain()
        {
            ++m_RefCount;
        }
        public void Release()
        {
            --m_RefCount;
            m_CacheTimeCounter = 0;
        }
        public bool CanDestroy()
        {
            return m_RefCount <= 0 && m_CacheTimeCounter >= m_CacheTime;
        }
        public string Name
        {
            get { return m_AssetXName; }
        }
        public T GetValue<T>() where T : Object
        {
            return m_AssetObject as T;
        }
        public void SetValue(Object value)
        {
            if (value != null)
            {
                XDebug.Log(XABConst.Tag, $"加载成功！！ bundle:{m_BundleName} asset:{m_AssetXName}");
                m_state = EnumJobState.Done;
            }
            else
            {
                XDebug.LogError(XABConst.Tag, $"加载失败！！ bundle:{m_BundleName} asset:{m_AssetXName}");
                m_state = EnumJobState.None;
            }
            m_AssetObject = value;
            m_callback?.Invoke(m_AssetXName, m_AssetObject);
            m_callback = null;
        }
        public void Dispose()
        {
            if (m_state != EnumJobState.None)
            {
                m_AssetManger?.UnloadBundle(m_BundleName);
            }
            if (m_state == EnumJobState.Process)
            {
                _StopLoadAsync();
            }
            m_AssetObject = null;
            m_callback = null;
            m_state = EnumJobState.None;
        }
        public void AddCallback(Action<string, Object> callback)
        {
            if (callback == null)
                return;
            m_callback += callback;
        }
        public void Load<T>(IXAssetManager manager, string name) where T : Object
        {
            m_AssetManger = manager;
            m_BundleName = manager.GetBundleNameByAssetName(name);
            m_AssetXName = name;
            
            var assetBundle = manager.LoadBundle(m_BundleName);
            var assetObject = assetBundle.LoadAsset<T>(m_AssetXName);
            SetValue(assetObject);
        }

        public void LoadAsync<T>(IXAssetManager manager, string name, Action<string, Object> callback = null)
            where T : Object
        {
            m_AssetManger = manager;
            m_BundleName = manager.GetBundleNameByAssetName(name);
            m_AssetXName = name;
            
            manager.LoadBundleAsync(m_BundleName, (bundleName, assetBundle) =>
            {
                if (m_BundleName != bundleName)
                    return;
                if (assetBundle == null)
                {
                    SetValue(null);
                    return;
                }
                m_LoadAssetRequest = assetBundle.LoadAssetAsync<T>(m_AssetXName);
            });
            m_state = EnumJobState.Process;
            m_callback += callback;
        }

        void _StopLoadAsync()
        {
            m_LoadAssetRequest = null;
        }

        public void StopAsync()
        {
            if (m_state != EnumJobState.Process)
                return;
            m_AssetManger?.UnloadBundle(m_BundleName);
            _StopLoadAsync();
            m_state = EnumJobState.None;
        }
        public void Tick(float deltaTime)
        {
            switch (m_state)
            {
                case EnumJobState.Process:
                    if (m_LoadAssetRequest != null && m_LoadAssetRequest.isDone)
                    {
                        SetValue(m_LoadAssetRequest.asset);
                    }
                    break;
                case EnumJobState.Done:
                    if (m_RefCount <= 0 && m_CacheTimeCounter < m_CacheTime)
                    {
                        m_CacheTimeCounter += deltaTime;
                    }
                    break;
            }
        }
    }

}
