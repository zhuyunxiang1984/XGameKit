using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIWindowStateMachine : XStateMachine<XUIWindow>
    {
        public const string stShow = "stShow";
        public const string stIdle = "stIdle";
        public const string stHide = "stHide";
        public const string stLoad = "stLoad";
        public const string stUnload = "stUnload";
        public const string stCache = "stCache"; 
        public const string stDestroy = "stDestroy";

        public XUIWindowStateMachine()
        {
            AddState(stLoad, new XUIWindowStateLoad(), true);
            AddState(stUnload, new XUIWindowStateUnload());
            AddState(stShow, new XUIWindowStateShow());
            AddState(stIdle, new XUIWindowStateIdle());
            AddState(stHide, new XUIWindowStateHide());
            AddState(stCache, new XUIWindowStateCache());
            AddState(stDestroy, new XUIWindowStateDestroy());
        }
    }
    
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
            if (!obj.isShow)
            {
                obj.stateMachine.ChangeState(XUIWindowStateMachine.stDestroy);
                return;
            }

            if (m_asset == null)
                return;
            obj.gameObject = GameObject.Instantiate(m_asset, obj.uiManager.uiRoot.uiUnusedNode);
            obj.mono = obj.gameObject.GetComponent<XUIWindowMono>();
            obj.mono.Init(obj);
            obj.cacheTime = obj.mono.cacheTime;
            obj.stateMachine.ChangeState(XUIWindowStateMachine.stShow);
        }
    }
    //卸载UI资源
    public class XUIWindowStateUnload : XState<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (obj.isShow)
            {
                obj.stateMachine.ChangeState(XUIWindowStateMachine.stShow);
                return;
            }
            obj.mono.Term();
            obj.mono = null;
            GameObject.Destroy(obj.gameObject);
            obj.gameObject = null;
            obj.uiManager.assetLoader.UnloadAsset(obj.resName);
            obj.stateMachine.ChangeState(XUIWindowStateMachine.stDestroy);
        }
    }
    //显示状态
    public class XUIWindowStateShow : XState<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (!obj.isShow)
            {
                obj.stateMachine.ChangeState(XUIWindowStateMachine.stUnload);
                return;
            }
            //设置数据
            obj.mono.ShowController(obj.initParam);
            //处理缓存消息
            foreach (var msg in obj.msgCacheList)
            {
                obj.MsgManager.SendMsg(msg);
            }
            //加入canvas排序
            obj.layer = obj.mono.layerData.GetValue();
            obj.canvas = obj.uiManager.uiRoot.uiCanvasManager.AppendCanvas(obj.layer, obj.openTick);
            obj.gameObject.transform.SetParent(obj.canvas.transform, false);
            obj.gameObject.SetActive(true);

            obj.stateMachine.ChangeState(XUIWindowStateMachine.stIdle);
        }
    }
    //隐藏状态
    public class XUIWindowStateHide : XState<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }
        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (obj.isShow)
            {
                obj.stateMachine.ChangeState(XUIWindowStateMachine.stIdle);
                return;
            }
            obj.mono.HideController();
            obj.uiManager.uiRoot.uiCanvasManager.RemoveCanvas(obj.canvas);
            obj.canvas = null;
            obj.gameObject.SetActive(false);
            obj.gameObject.transform.SetParent(obj.uiManager.uiRoot.uiUnusedNode, false);
            
            obj.stateMachine.ChangeState(XUIWindowStateMachine.stCache);
        }
    }
    //空闲状态
    public class XUIWindowStateIdle: XState<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (!obj.isShow)
            {
                obj.stateMachine.ChangeState(XUIWindowStateMachine.stHide);
                return;
            }
            obj.mono.Tick(elapsedTime);
        }
    }
    //缓存状态
    public class XUIWindowStateCache : XState<XUIWindow>
    {
        protected float m_time;
        protected float m_timeCounter;
        public override void OnEnter(XUIWindow obj)
        {
            m_time = obj.cacheTime;
            m_timeCounter = 0f;
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (obj.isShow)
            {
                obj.stateMachine.ChangeState(XUIWindowStateMachine.stShow);
                return;
            }
            if (m_time > 0)
            {
                m_timeCounter += elapsedTime;
                if (m_timeCounter >= m_time)
                {
                    obj.stateMachine.ChangeState(XUIWindowStateMachine.stUnload);
                }
            }
        }
    }
    //销毁状态
    public class XUIWindowStateDestroy : XState<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (obj.isShow)
            {
                obj.stateMachine.ChangeState(XUIWindowStateMachine.stLoad);
                return;
            }
        }
    }
        

}
