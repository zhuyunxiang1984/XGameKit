using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    

//加载UI资源
    public class XUIWindowStateLoad : XState<XUIWindow>
    {
        protected string m_name;
        protected GameObject m_asset;
        public override void OnEnter(XUIWindow obj)
        {
            m_name = obj.resName;
            m_asset = null;
            obj.uiManager.assetLoader.AddListener(_OnLoadAsset);
            obj.uiManager.assetLoader.LoadAsset(m_name);
        }

        public override void OnLeave(XUIWindow obj)
        {
            obj.uiManager.assetLoader.DelListener(_OnLoadAsset);
            obj.uiManager.assetLoader.UnloadAsset(m_name);
        }

        void _OnLoadAsset(string name, GameObject asset)
        {
            if (m_name != name)
                return;
            m_asset = asset;
        }
        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (m_asset == null)
                return;
            obj.gameObject = GameObject.Instantiate(m_asset, obj.uiManager.uiRoot.uiUnusedNode);
            obj.gameObject.SetActive(false);
            obj.mono = obj.gameObject.GetComponent<XUIWindowMono>();
            obj.mono.Init(obj);
            obj.cacheTime = obj.mono.cacheTime;
            obj.stateMachine.ChangeState(XUIWindowStateMachine.stShow);
        }
        public override string Transition(XUIWindow obj)
        {
            if (!obj.isShow)
                return XUIWindowStateMachine.stUnload;
            return String.Empty;
        }
    }

}


