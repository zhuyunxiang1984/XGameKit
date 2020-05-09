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
        protected IXAssetManager m_AssetManager;
        protected string m_BundleName;
        protected List<string> m_dependencies = new List<string>();
        protected int m_dependencyCurCount = 0;//依赖包加载计数
        protected int m_dependencyMaxCount = 0;
        protected EnumJobState m_state;
        protected AssetBundle m_AssetBundle;//Bundle缓存
        protected Action<string, AssetBundle> m_callback;//完成回调
        protected int m_RefCount;
        protected float m_CacheTime = 5f;
        protected float m_CacheTimeCounter;

        public enum EnumLoadStep
        {
            None = 0,
            Step1,
            Step2,
            Step3,
            Step4,
            Complete,
        }
        protected EnumLoadStep m_step;
        protected string m_fullPath;
        protected byte[] m_fileData;
        protected AssetBundleCreateRequest m_CreateRequest;
        
        
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
            get { return m_BundleName; }
        }

        public EnumJobState State
        {
            get { return m_state; }
        }

        public AssetBundle GetValue()
        {
            return m_AssetBundle;
        }
        public void SetValue(AssetBundle value)
        {
            if (value != null)
            {
                XDebug.Log(XABConst.Tag, $"加载成功！！ bundle:{m_BundleName}");
                m_state = EnumJobState.Done;
            }
            else
            {
                XDebug.LogError(XABConst.Tag, $"加载失败！！ bundle:{m_BundleName}");
                m_state = EnumJobState.None;
            }
            m_AssetBundle = value;
            m_callback?.Invoke(m_BundleName, m_AssetBundle);
            m_callback = null;
        }
        public void Dispose()
        {
            if (m_AssetManager != null && m_state != EnumJobState.None)
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
            if (m_state == EnumJobState.Process)
            {
                _StopLoadAsync();
            }
            m_AssetBundle = null;
            m_callback = null;
            m_state = EnumJobState.None;
        }
        public void AddCallback(Action<string, AssetBundle> callback)
        {
            if (callback == null)
                return;
            m_callback += callback;
        }
        //同步加载
        public void Load(IXAssetManager manager, EnumFileLocation location, EnumBundleType bundleType, string name)
        {
            m_AssetManager = manager;
            m_BundleName = name;
            
            m_dependencies.Clear();
            var dependencies = manager.GetDependencies(m_BundleName);
            if (dependencies != null)
            {
                m_dependencies.AddRange(dependencies);
                foreach (var dependency in dependencies)
                {
                    manager.LoadBundle(dependency);
                }
            }
            var fullPath = XABUtilities.GetBundleFullPath(location, bundleType, name);
            XDebug.Log(XABConst.Tag, $"加载 {fullPath}");
            SetValue(_LoadInternal(fullPath));
        }
        AssetBundle _LoadInternal(string fullPath)
        {
            try
            {
                byte[] data = XUtilities.ReadFile(fullPath);
                //解密
                //读取AssetBundle
                return AssetBundle.LoadFromMemory(data);
            }
            catch (Exception e)
            {
                XDebug.Log(XABConst.Tag, $"加载assetbundle失败 {fullPath}\n{e.ToString()}");
            }
            return null;
        }
        //异步加载
        public void LoadAsync(IXAssetManager manager, EnumFileLocation location, EnumBundleType bundleType, string name, Action<string, AssetBundle> callback = null)
        {
            m_AssetManager = manager;
            m_BundleName = name;
            m_dependencies.Clear();
            
            var dependencies = manager.GetDependencies(m_BundleName);
            if (dependencies != null)
            {
                m_dependencies.AddRange(dependencies);
                m_dependencyCurCount = 0;
                m_dependencyMaxCount = dependencies.Count;
                foreach (var dependent in dependencies)
                {
                    manager.LoadBundleAsync(dependent, _OnLoadedDependent);
                }
            }
            var fullPath = XABUtilities.GetBundleFullPath(location, bundleType, name);
            XDebug.Log(XABConst.Tag, $"加载 {fullPath}");
            _StartLoadAsync(fullPath);
            m_state = EnumJobState.Process;
            m_callback += callback;
        }
        void _StartLoadAsync(string fullPath)
        {
            m_fullPath = fullPath;
            m_step = EnumLoadStep.Step1;
        }

        void _StopLoadAsync()
        {
            m_CreateRequest = null;
            m_step = EnumLoadStep.None;
        }
        void _OnLoadedDependent(string name, AssetBundle assetBundle)
        {
            m_dependencyCurCount += 1;
        }
        bool _CheckDependenciesCompleted()
        {
            return (m_dependencyMaxCount == 0 || m_dependencyCurCount == m_dependencyMaxCount);
        }
        public void StopAsync()
        {
            if (m_state != EnumJobState.Process)
                return;
            if (m_AssetManager != null)
            {
                foreach (var dependent in m_dependencies)
                {
                    m_AssetManager.UnloadBundle(dependent);
                }
            }
            _StopLoadAsync();
            m_state = EnumJobState.None;
        }
        public void Tick(float deltaTime)
        {
            switch (m_state)
            {
                case EnumJobState.Process:
                    switch (m_step)
                    {
                        case EnumLoadStep.Step1:
                            _ExecuteStep1();
                            break;
                        case EnumLoadStep.Step2:
                            _ExecuteStep2();
                            break;
                        case EnumLoadStep.Step3:
                            _ExecuteStep3();
                            break;
                        case EnumLoadStep.Step4:
                            _ExecuteStep4();
                            break;
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
        //读取文件
        void _ExecuteStep1()
        {
            m_fileData = XUtilities.ReadFile(m_fullPath);
            m_step = EnumLoadStep.Step2;
        }
        //解密
        void _ExecuteStep2()
        {
            //m_fileData
            m_step = EnumLoadStep.Step3;
        }
        //读取AssetBundle
        void _ExecuteStep3()
        {
            m_CreateRequest = AssetBundle.LoadFromMemoryAsync(m_fileData);
            m_step = EnumLoadStep.Step4;
        }
        //等待Request完成
        void _ExecuteStep4()
        {
            if (m_CreateRequest == null)
            {
                m_step = EnumLoadStep.Complete;
                SetValue(null);
                return;
            }
            if (!m_CreateRequest.isDone || !_CheckDependenciesCompleted())
                return;
            m_step = EnumLoadStep.Complete;
            SetValue(m_CreateRequest.assetBundle);
        }
        
    }
}

