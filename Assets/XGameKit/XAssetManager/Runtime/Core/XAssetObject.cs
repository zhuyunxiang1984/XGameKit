using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XGameKit.XAssetManager
{
    public class XAssetObject
    {
        public string Name { get; protected set; }
        public string BundleName { get; protected set; }

        public EnumLoadState State { get; protected set; }
        
        //引用计数
        public int RefCount{ get; protected set; }
        public float RefZeroTime { get; protected set; }
        
        public void Retain()
        {
            ++RefCount;
        }
        public void Release()
        {
            --RefCount;
            RefZeroTime = 0;
        }

        protected XAssetManager m_AssetManger;
        protected AssetBundleRequest m_Request;
        protected Object m_AssetObject;
        protected Action<string, Object> m_Callback;

        public T GetValue<T>() where T : Object
        {
            return m_AssetObject as T;
        }

        public void Dispose()
        {
            if (m_AssetManger != null && State != EnumLoadState.None)
            {
                m_AssetManger.UnloadBundle(BundleName);
            }
            State = EnumLoadState.None;
            m_Request = null;
            m_AssetObject = null;
            m_Callback = null;
            RefCount = 0;
        }
        public void AddCallback(Action<string, Object> callback)
        {
            if (callback == null)
                return;
            m_Callback += callback;
        }
        public void Load<T>(XAssetManager manager, string name) where T : Object
        {
            m_AssetManger = manager;
            Name = name;
            BundleName = manager.GetAssetBundleName(name);

            var assetBundle = manager.LoadBundle(BundleName);
            m_AssetObject = assetBundle.LoadAsset<T>(name);
            State = EnumLoadState.Done;
        }
        public void LoadAsync<T>(XAssetManager manager, string name, Action<string, Object> callback = null) where T : Object
        {
            m_AssetManger = manager;
            Name = name;
            BundleName = manager.GetAssetBundleName(name);
            
            manager.LoadBundleAsync(BundleName, (p1,p2)=>
            {
                if (Name != p1)
                    return;
                m_Request = p2.LoadAssetAsync<T>(name);
            });
            State = EnumLoadState.Loading;
            m_Callback += callback;
        }

        public void StopAsync()
        {
            if (m_AssetManger != null && State == EnumLoadState.Loading)
            {
                m_AssetManger.UnloadBundle(BundleName);
            }
            m_Request = null;
            State = EnumLoadState.None;
        }
        public void Tick(float deltaTime)
        {
            switch (State)
            {
                case EnumLoadState.Loading:
                    if (m_Request == null || !m_Request.isDone)
                        return;
                    m_AssetObject = m_Request.asset;
                    State = EnumLoadState.Done;
                    if (m_Callback != null)
                    {
                        m_Callback.Invoke(Name, m_AssetObject);
                        m_Callback = null;
                    }
                    break;
                case EnumLoadState.Done:
                    if (RefCount <= 0)
                    {
                        RefZeroTime += deltaTime;
                    }
                    break;
            }
        }
    }

}
