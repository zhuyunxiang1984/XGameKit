using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    public class XWebRequestManagerSingle : MonoBehaviour
    {
        private static XWebRequestManagerSingle m_instanceCache;
        public static XWebRequestManagerSingle m_instance
        {
            get
            {
                if (m_instanceCache == null)
                {
                    var go = new GameObject("XWebRequestManagerSingle");
                    DontDestroyOnLoad(go);
                    m_instanceCache = go.AddComponent<XWebRequestManagerSingle>();
                }
                return m_instanceCache;
            }
        }
        
        public static void GetUrl(string url, Action<string, byte[]> response, string encrypt = "", Action<float> onProgress = null, int timeout = 5)
        {
            m_instance.RequestManager.GetUrl(url, response, encrypt, onProgress, timeout);
        }
        public static void GetUrl(string url, Action<string, string> response, string encrypt = "", Action<float> onProgress = null, int timeout = 5)
        {
            m_instance.RequestManager.GetUrl(url, response, encrypt, onProgress, timeout);
        }

        public static void PostUrl(string url, string data, Action<string, byte[]> response, string encrypt = "", Action<float> onProgress = null, int timeout = 5)
        {
            m_instance.RequestManager.PostUrl(url, data, response, encrypt, onProgress, timeout);
        }

        public static void PostUrl(string url, string data, Action<string, string> response, string encrypt = "", Action<float> onProgress = null, int timeout = 5)
        {
            m_instance.RequestManager.PostUrl(url, data, response, encrypt, onProgress, timeout);
        }
        
        public XWebRequestManager RequestManager { get; protected set; } = new XWebRequestManager();

        private void OnDestroy()
        {
            RequestManager.Dispose();
        }

        private void Update()
        {
            RequestManager.Tick(Time.deltaTime);
        }
    }

}
