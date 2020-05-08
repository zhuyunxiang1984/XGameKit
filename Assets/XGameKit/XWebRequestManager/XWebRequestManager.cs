using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace XGameKit.Core
{
    public class XWebRequestManager : IXService
    {
        public const string Tag = "XWebRequestManager";
        public const int HttpCode_OK = 200;
        
        protected class XWebRequest
        {
            public UnityWebRequestAsyncOperation async;
            public string encrypt;
            public Action<string, byte[]> binResponse;
            public Action<string, string> strResponse;
            public Action<float> onProgress;
            
            public void Reset()
            {
                async = null;
                binResponse = null;
                strResponse = null;
            }
            
            public bool HandleRequest()
            {
                onProgress?.Invoke(async.progress);
                if (!async.isDone)
                    return false;
                var webRequest = async.webRequest;
                if (webRequest.isNetworkError || 
                    webRequest.isHttpError ||
                    webRequest.responseCode != HttpCode_OK)
                {
                    XDebug.LogError(Tag, $"url failed {webRequest.error}");
                    if (binResponse != null)
                    {
                        binResponse?.Invoke(webRequest.error, null);
                        binResponse = null;
                    }
                    if (strResponse != null)
                    {
                        strResponse?.Invoke(webRequest.error, null);
                        strResponse = null;
                    }
                }
                else
                {
                    XDebug.Log(Tag, $"url successed {webRequest.downloadHandler.text}");
                    var binResponseData = webRequest.downloadHandler.data;
                    //解密
                    if (!string.IsNullOrEmpty(encrypt))
                    {
                        
                    }
                    if (binResponse != null)
                    {
                        binResponse.Invoke(string.Empty, binResponseData);
                        binResponse = null;
                    }
                    if (strResponse != null)
                    {
                        var strResponseData = Encoding.UTF8.GetString(binResponseData);
                        strResponse.Invoke(string.Empty, strResponseData);
                        strResponse = null;
                    }
                }
                return true;
            }
        }
        List<XWebRequest> m_requests = new List<XWebRequest>();

        public void Dispose()
        {
            foreach (var request in m_requests)
            {
                request.Reset();
                XObjectPool.Free(request);
            }
            m_requests.Clear();
        }
        public void Tick(float deltaTime)
        {
            if (m_requests.Count < 1)
                return;
            for (int i = 0; i < m_requests.Count;)
            {
                var request = m_requests[i];
                if (request.HandleRequest())
                {
                    request.Reset();
                    XObjectPool.Free(request);
                    m_requests.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }

        public void GetUrl(string url, Action<string, byte[]> response, string encrypt,  Action<float> onProgress = null, int timeout = 5)
        {
            _GetUrlInternal(new ParamBundle()
            {
                url = url,
                strData = null,
                binData = null,
                strResponse = null,
                binResponse = response,
                encrypt = encrypt,
                timeout = timeout,
                onProgress = onProgress,
            });
        }
        public void GetUrl(string url, Action<string, string> response, string encrypt,  Action<float> onProgress = null, int timeout = 5)
        {
            _GetUrlInternal(new ParamBundle()
            {
                url = url,
                strData = null,
                binData = null,
                strResponse = response,
                binResponse = null,
                encrypt = encrypt,
                timeout = timeout,
                onProgress = onProgress,
            });
        }

        public void PostUrl(string url, string data, Action<string, byte[]> response, string encrypt, Action<float> onProgress = null, int timeout = 5)
        {
            _PostUrlInternal(new ParamBundle()
            {
                url = url,
                strData = data,
                binData = null,
                strResponse = null,
                binResponse = response,
                encrypt = encrypt,
                timeout = timeout,
                onProgress = onProgress,
            });
        }

        public void PostUrl(string url, string data, Action<string, string> response, string encrypt, Action<float> onProgress = null, int timeout = 5)
        {
            _PostUrlInternal(new ParamBundle()
            {
                url = url,
                strData = data,
                binData = null,
                strResponse = response,
                binResponse = null,
                encrypt = encrypt,
                timeout = timeout,
                onProgress = onProgress,
            });
        }
        
        protected struct ParamBundle
        {
            public string url;
            public string strData;
            public byte[] binData;
            public Action<string, string> strResponse;
            public Action<string, byte[]> binResponse;
            public Action<float> onProgress;
            public string encrypt;
            public int timeout;
        }

        protected void _GetUrlInternal(ParamBundle param)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(param.url);
            webRequest.timeout = param.timeout;
            _AddRequest(webRequest, param.encrypt, param.strResponse, param.binResponse, param.onProgress);
        }

        protected void _PostUrlInternal(ParamBundle param)
        {
            UnityWebRequest webRequest = new UnityWebRequest(param.url, UnityWebRequest.kHttpVerbPOST);
            var bodyData = param.binData;
            if (!string.IsNullOrEmpty(param.strData))
            {
                bodyData = Encoding.UTF8.GetBytes(param.strData);
            }
            if (bodyData != null)
            {
                webRequest.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyData);
            }
            //加密
            if (!string.IsNullOrEmpty(param.encrypt))
            {
                
            }
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = param.timeout;
            _AddRequest(webRequest, param.encrypt, param.strResponse, param.binResponse, param.onProgress);
        }

        protected void _AddRequest(UnityWebRequest webRequest, string encrypt, Action<string, string> strResponse, Action<string, byte[]> binResponse, Action<float> onProgress)
        {
            var request = XObjectPool.Alloc<XWebRequest>();
            request.async = webRequest.SendWebRequest();
            request.encrypt = encrypt;
            request.strResponse = strResponse;
            request.binResponse = binResponse;
            request.onProgress = onProgress;
            m_requests.Add(request);
        }
    }
}
