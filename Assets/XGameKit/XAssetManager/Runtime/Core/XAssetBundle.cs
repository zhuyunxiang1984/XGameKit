using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.Utilities;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XBehaviorTree;

namespace XGameKit.XAssetManager
{
    public class XAssetBundle
    {
        public string Name { get; protected set; }
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

        protected XAssetManager m_AssetManager;
        protected XABLoader m_ABLoader;
        protected List<string> m_dependencies = new List<string>();

        //完成回调
        protected Action<string, AssetBundle> m_Callback;
        //依赖包加载计数
        protected int m_DependentCurCount = 0;
        protected int m_DependentMaxCount = 0;

        //Bundle缓存
        protected AssetBundle m_AssetBundle;
        
        public AssetBundle GetValue()
        {
            return m_AssetBundle;
        }
        public void SetValue(AssetBundle value)
        {
            if (value != null)
            {
                State = EnumLoadState.Done;
            }
            else
            {
                State = EnumLoadState.None;
            }
            m_AssetBundle = value;
            if (m_Callback != null)
            {
                m_Callback.Invoke(Name, m_AssetBundle);
                m_Callback = null;
            }
        }
        public XAssetBundle()
        {
            if (XABUtilities.IsSimulatMode())
            {
                m_ABLoader = new XABLoaderSimulate();
            }
            else
            {
                m_ABLoader = new XABLoaderNormal();
            }
        }
        
        public void Dispose()
        {
            if (m_AssetManager != null && State != EnumLoadState.None)
            {
                foreach (var dependent in m_dependencies)
                {
                    m_AssetManager.UnloadBundle(dependent);
                }
            }
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(true);
                m_AssetBundle = null;
            }
            if (State == EnumLoadState.Loading)
            {
                m_ABLoader.StopAsync();
            }
            m_Callback = null;
            State = EnumLoadState.None;
        }

        public void AddCallback(Action<string, AssetBundle> callback)
        {
            if (callback == null)
                return;
            m_Callback += callback;
        }
        //同步加载
        public void Load(XAssetManager manager, EnumLocation location, string name)
        {
            m_AssetManager = manager;
            Name = name;
            m_dependencies.Clear();
            
            var dependencies = manager.GetDependencies(Name);
            if (dependencies != null)
            {
                m_dependencies.AddRange(dependencies);
                foreach (var dependent in dependencies)
                {
                    manager.LoadBundle(dependent);
                }
            }
            var fullPath = XABUtilities.GetAssetBundleFullPath(location, name);
            XDebug.Log(XABConst.Tag, $"加载 {fullPath}");
            SetValue(m_ABLoader.Load(fullPath));
        }
        //异步加载
        public void LoadAsync(XAssetManager manager, EnumLocation location, string name, Action<string, AssetBundle> callback = null)
        {
            m_AssetManager = manager;
            Name = name;
            m_dependencies.Clear();
            
            var dependencies = manager.GetDependencies(Name);
            if (dependencies != null)
            {
                m_dependencies.AddRange(dependencies);
                m_DependentCurCount = 0;
                m_DependentMaxCount = dependencies.Count;
                foreach (var dependent in dependencies)
                {
                    manager.LoadBundleAsync(dependent, _OnLoadedDependent);
                }
            }
            var fullPath = XABUtilities.GetAssetBundleFullPath(location, name);
            XDebug.Log(XABConst.Tag, $"加载 {fullPath}");
            m_ABLoader.LoadAsync(fullPath);
            State = EnumLoadState.Loading;
            
            m_Callback += callback;
        }
        public void StopAsync()
        {
            if (m_AssetManager != null && State == EnumLoadState.Loading)
            {
                foreach (var dependent in m_dependencies)
                {
                    m_AssetManager.UnloadBundle(dependent);
                }
            }
            m_ABLoader.StopAsync();
            State = EnumLoadState.None;
        }
        public void Tick(float deltaTime)
        {
            switch (State)
            {
                case EnumLoadState.Loading:
                    m_ABLoader.Tick(deltaTime);
                    if (m_ABLoader.IsDone && (m_DependentMaxCount == 0 || m_DependentCurCount == m_DependentMaxCount))
                    {
                        SetValue(m_ABLoader.GetValue());
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
        void _OnLoadedDependent(string name, AssetBundle assetBundle)
        {
            m_DependentCurCount += 1;
        }
    }
}

