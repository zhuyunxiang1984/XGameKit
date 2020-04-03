using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIManager : IXService
    {
        public static T AllocParam<T>() where T: class, IXPoolable, new()
        {
            return XObjectPool.Alloc<T>();
        }
        
        //
        public XUIRoot uiRoot { get; protected set; }
        //msgmanager
        public XMsgManager MsgManager { get; protected set; } = new XMsgManager();
        //evtmanager
        public XEvtManager EvtManager { get; protected set; } = new XEvtManager();
        //assetloader
        public IXUIAssetLoader assetLoader { get; protected set; } = new XUIAssetLoaderDefault();

        //windowlist
        protected Dictionary<string, XUIWindow> m_dictWindows = new Dictionary<string, XUIWindow>();
        protected List<XUIWindow> m_listWindows = new List<XUIWindow>();
        
        public XUIManager()
        {
            uiRoot = XUIRoot.CreateInstance("XUIRoot");
        }
        public void Dispose()
        {
            
        }

        public void Tick(float step)
        {
            foreach (var window in m_listWindows)
            {
                window.Tick(step);
            }
        }

        public bool IsShow(string name)
        {
            if (!m_dictWindows.ContainsKey(name))
                return false;
            return m_dictWindows[name].isShow;
        }
        public void ShowWindow(string name, object param = null)
        {
            Debug.Log($"ShowWindow {name}");
            XUIWindow window = null;
            if (m_dictWindows.ContainsKey(name))
            {
                window = m_dictWindows[name];
            }
            else
            {
                window = XObjectPool.Alloc<XUIWindow>();
                window.Init(this, name, param);
                m_listWindows.Add(window);
                m_dictWindows.Add(name, window);
            }
            window.isShow = true;
        }

        public void HideWindow(string name)
        {
            if (!m_dictWindows.ContainsKey(name))
                return;
            XUIWindow window = m_dictWindows[name];
            window.isShow = false;
        }
    }
}
