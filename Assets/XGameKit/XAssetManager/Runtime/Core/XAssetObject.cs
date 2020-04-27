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

        protected XAOLoader m_AOLoader;

        protected XAssetManager m_AssetManger;
        protected Object m_AssetObject;
        protected Action<string, Object> m_Callback;

        public T GetValue<T>() where T : Object
        {
            return m_AssetObject as T;
        }
        public void SetValue(Object value)
        {
            if (value != null)
            {
                State = EnumLoadState.Done;
            }
            else
            {
                State = EnumLoadState.None;
            }
            m_AssetObject = value;
            if (m_Callback != null)
            {
                m_Callback.Invoke(Name, m_AssetObject);
                m_Callback = null;
            }
        }
        
        public XAssetObject()
        {
            if (XABUtilities.IsSimulatMode())
            {
                m_AOLoader = new XAOLoaderSimulate();
            }
            else
            {
                m_AOLoader = new XAOLoaderNormal();
            }
        }

        public void Dispose()
        {
            if (m_AssetManger != null && State != EnumLoadState.None)
            {
                m_AssetManger.UnloadBundle(BundleName);
            }
            if (State == EnumLoadState.Loading)
            {
                m_AOLoader.StopAsync();
            }
            State = EnumLoadState.None;
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
            var assetObject = m_AOLoader.Load<T>(assetBundle, name);
            SetValue(assetObject);
        }
        public void LoadAsync<T>(XAssetManager manager, string name, Action<string, Object> callback = null) where T : Object
        {
            m_AssetManger = manager;
            Name = name;
            BundleName = manager.GetAssetBundleName(name);
            
            manager.LoadBundleAsync(BundleName, (p1,p2)=>
            {
                if (this.BundleName != p1)
                    return;
                m_AOLoader.LoadAsync<T>(p2, name);
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
            m_AOLoader.StopAsync();
            State = EnumLoadState.None;
        }
        public void Tick(float deltaTime)
        {
            switch (State)
            {
                case EnumLoadState.Loading:
                    m_AOLoader.Tick(deltaTime);
                    if (m_AOLoader.IsDone)
                    {
                        SetValue(m_AOLoader.GetValue());
                        State = EnumLoadState.Done;
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
