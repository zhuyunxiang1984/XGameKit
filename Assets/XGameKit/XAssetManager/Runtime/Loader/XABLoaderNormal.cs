using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    public class XABLoaderNormal : XABLoader
    {
        //1读取文件 2解密 3加载到AssetBundle 4等待加载完成
        protected int m_Step;
        protected string m_FullPath;
        protected AssetBundleCreateRequest m_CreateRequest;
        protected byte[] m_Data;

        public override bool IsDone
        {
            get { return m_CreateRequest != null && m_CreateRequest.isDone; }
        }

        public override AssetBundle GetValue()
        {
            return m_CreateRequest != null ? m_CreateRequest.assetBundle : null;
        }
        
        public override AssetBundle Load(string fullPath)
        {
            try
            {
                byte[] data = XABUtilities.ReadFile(fullPath);
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

        public override void LoadAsync(string fullPath)
        {
            m_FullPath = fullPath;
            m_Step = 1;
        }

        public override void StopAsync()
        {
            m_Step = 0;
            m_CreateRequest = null;
        }

        public override void Tick(float deltaTime)
        {
            if (m_Step == 0)
                return;
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
            m_Step = 0;
        }
    }
}

