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
        public List<string> Dependents { get; protected set; } = new List<string>();
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
        //加载任务
        protected LoadAssetBundleTask m_Task = new LoadAssetBundleTask();
        //完成回调
        protected Action<string, AssetBundle> m_Callback;
        //依赖包加载计数
        protected int m_DependentCurCount = 0;
        protected int m_DependentMaxCount = 0;

        //Bundle缓存
        protected AssetBundle m_AssetBundle;

        //资源缓存
        protected Dictionary<string, XAssetObject> m_AssetObjects = new Dictionary<string, XAssetObject>();
        
        public AssetBundle GetValue()
        {
            return m_AssetBundle;
        }
        
        public void Dispose()
        {
            foreach (var assetObject in m_AssetObjects.Values)
            {
                assetObject.Dispose();
            }
            if (m_AssetManager != null && State != EnumLoadState.None)
            {
                foreach (var dependent in Dependents)
                {
                    m_AssetManager.UnloadBundle(dependent);
                }
            }
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(true);
                m_AssetBundle = null;
            }
            m_AssetObjects.Clear();
            m_Task.Stop();
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
            Dependents.Clear();
            
            var dependents = manager.GetDependents(Name);
            if (dependents != null)
            {
                Dependents.AddRange(dependents);
                foreach (var dependent in dependents)
                {
                    manager.LoadBundle(dependent);
                }
            }
            var fullPath = XABUtilities.GetAssetBundleFullPath(location, name);
            XDebug.Log(XABConst.Tag, $"加载 {fullPath}");
            m_AssetBundle = XABUtilities.LoadAssetBundle(fullPath);
            State = EnumLoadState.Done;

            if (m_Callback != null)
            {
                m_Callback.Invoke(name, m_AssetBundle);
                m_Callback = null;
            }
        }
        //异步加载
        public void LoadAsync(XAssetManager manager, EnumLocation location, string name, Action<string, AssetBundle> callback = null)
        {
            m_AssetManager = manager;
            Name = name;
            Dependents.Clear();
            
            var dependents = manager.GetDependents(Name);
            if (dependents != null)
            {
                Dependents.AddRange(dependents);
                m_DependentCurCount = 0;
                m_DependentMaxCount = dependents.Count;
                foreach (var dependent in dependents)
                {
                    manager.LoadBundleAsync(dependent, _OnLoadedDependent);
                }
            }
            var fullPath = XABUtilities.GetAssetBundleFullPath(location, name);
            XDebug.Log(XABConst.Tag, $"加载 {fullPath}");
            m_Task.Start(fullPath);
            State = EnumLoadState.Loading;
            
            m_Callback += callback;
        }
        public void StopAsync()
        {
            if (m_AssetManager != null && State == EnumLoadState.Loading)
            {
                foreach (var dependent in Dependents)
                {
                    m_AssetManager.UnloadBundle(dependent);
                }
            }
            m_Task.Stop();
            State = EnumLoadState.None;
        }
        public void Tick(float deltaTime)
        {
            switch (State)
            {
                case EnumLoadState.Loading:
                    m_Task.Tick(deltaTime);
                    if (m_Task.IsDone && (m_DependentMaxCount == 0 || m_DependentCurCount == m_DependentMaxCount))
                    {
                        m_AssetBundle = m_Task.GetValue();
                        State = EnumLoadState.Done;
                        if (m_Callback != null)
                        {
                            m_Callback.Invoke(Name, m_AssetBundle);
                            m_Callback = null;
                        }
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
        
        //--
        public class LoadAssetBundleTask
        {
            //1读取文件 2解密 3加载到AssetBundle 4等待加载完成
            protected int m_Step;
            protected string m_FullPath;
            protected AssetBundleCreateRequest m_CreateRequest;
            protected bool m_Done;
            protected byte[] m_Data;

            public bool IsDone
            {
                get { return m_Done; }
            }
            public AssetBundle GetValue()
            {
                return m_CreateRequest != null ? m_CreateRequest.assetBundle : null;
            }

            public void Start(string fullPath)
            {
                m_FullPath = fullPath;
                m_Step = 1;
                m_Done = false;
            }

            public void Stop()
            {
                m_Step = 0;
                m_CreateRequest = null;
            }
            public void Tick(float deltaTime)
            {
                switch (m_Step)
                {
                    case 1:
                        _ExecuteStep1();
                        break;
                    case 2:
                        _ExecuteStep2();
                        break;
                    case 3:
                        _ExecuteStep3();
                        break;
                    case 4:
                        _ExecuteStep4();
                        break;
                }
            }

            //读取文件
            void _ExecuteStep1()
            {
                m_Data = XABUtilities.ReadFile(m_FullPath);
                ++m_Step;
            }
            //解密
            void _ExecuteStep2()
            {
                ++m_Step;
            }
            //读取AssetBundle
            void _ExecuteStep3()
            {
                m_CreateRequest = AssetBundle.LoadFromMemoryAsync(m_Data);
                ++m_Step;
            }
            //等待Request完成
            void _ExecuteStep4()
            {
                if (!m_CreateRequest.isDone)
                    return;
                m_Done = true;
                ++m_Step;
            }
        }
    }
}

