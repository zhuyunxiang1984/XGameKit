using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIWindow : IXPoolable
    {
        public void Reset()
        {
        }

        public bool isShow;
        public XUIWindowStateMachine stateMachine = new XUIWindowStateMachine();
        
        //uimanager
        public XUIManager uiManager { get; protected set; }
        public string name { get; protected set; }
        public int layer;
        public string resName;
        public GameObject gameObject;
        public XUIWindowMono mono;

        public Canvas canvas;
        public object initParam;
        public long openTick;
        
        //缓存时间
        public float cacheTime;
        
        //controller
        //view
        //msgcache
        public List<XMessage> msgCacheList { get; protected set; }= new List<XMessage>();
        //msgmanager
        public XMsgManager MsgManager { get; protected set; } = new XMsgManager();
        
        //widgetlist
        //protected List<XUIWidget> m_widgets = new List<XUIWidget>();

        public void Init(XUIManager uiManager, string name, object param)
        {
            this.uiManager = uiManager;
            this.name = name;
            //临时
            resName = name;
            initParam = param;
            XMsgManager.Append(uiManager.MsgManager, MsgManager);
            stateMachine.Start();
        }

        public void Term()
        {
            XMsgManager.Remove(uiManager.MsgManager, MsgManager);
            uiManager = null;
        }
        public void Tick(float elapsedTime)
        {
            stateMachine.Tick(this, elapsedTime);
        }
    }
}