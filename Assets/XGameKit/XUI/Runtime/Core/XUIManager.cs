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
        
        //msgmanager
        public XMsgManager msgManager { get; protected set; } = new XMsgManager();
        //evtmanager
        public XEvtManager evtManager { get; protected set; } = new XEvtManager();
        //assetloader
        public IXUIAssetLoader assetLoader { get; protected set; } = new XUIAssetLoaderDefault();

        //windowlist
        protected Dictionary<string, XUIWindow> m_dictWindows = new Dictionary<string, XUIWindow>();
        protected List<XUIWindow> m_listWindows = new List<XUIWindow>();
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

        public void ShowWindow(string name, object param)
        {
            
        }

        public void HideWindow(string name)
        {
            
        }
    }
}
