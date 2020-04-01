using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIWindow
    {
        //uimanager
        public XUIManager uiManager { get; protected set; }
        public string name { get; protected set; }
        public int layer;
        public GameObject prefab;
        public object view;
        public object controller;
        public object canvas;
        public object initParam;
        public float openTick;
        
        //controller
        //view
        //msgcache
        public List<XMessage> msgCacheList { get; protected set; }= new List<XMessage>();
        //msgmanager
        public XMsgManager msgManager { get; protected set; } = new XMsgManager();

        //schedule
        protected XTaskSchedule<XUIWindow> m_schedule;
        //widgetlist
        //protected List<XUIWidget> m_widgets = new List<XUIWidget>();

        public void Init(XUIManager uiManager, object param)
        {
            this.uiManager = uiManager;
            initParam = param;
            XMsgManager.Append(uiManager.msgManager, msgManager);
        }

        public void Term()
        {
            XMsgManager.Remove(uiManager.msgManager, msgManager);
            uiManager = null;
        }
        public void Tick(float step)
        {
            m_schedule.Update(step);
        }
        public void SetState(XTaskSchedule<XUIWindow> state)
        {
            if(m_schedule != null)
                m_schedule.Stop();
            m_schedule = state;
            m_schedule.Start(this);
        }
    }

}