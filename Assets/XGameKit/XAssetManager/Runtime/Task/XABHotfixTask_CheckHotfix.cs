using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace XGameKit.XAssetManager
{
    //比较文件列表
    public class XABHotfixTask_CheckHotfix : XTask<XAssetManagerOrdinary>
    {
        protected string m_serverAddress;
        protected XFileList m_server_filelist;
        protected XFileList m_client_filelist;
        protected XFileList m_origin_filelist;
        
        public override void Enter(XAssetManagerOrdinary obj)
        {
            var assetInfoManager = obj.AssetInfoManager;
            m_serverAddress = obj.serverAddress;
#if UNITY_EDITOR
            m_serverAddress = EditorPrefs.GetString(XABConst.EKResUrl);
            if (string.IsNullOrEmpty(m_serverAddress))
            {
                Debug.LogError("没有配置url");
                return;
            }
            m_serverAddress += "/_filelist.json";
#endif
            XWebRequestManagerSingle.GetUrl(m_serverAddress,
                delegate(string error, string responseData)
                {
                    if (!string.IsNullOrEmpty(error))
                        return;
                    m_server_filelist = JsonUtility.FromJson<XFileList>(responseData);
                    XDebug.Log(XABConst.Tag, m_server_filelist.ToLog());
                });
            m_server_filelist = null;
            m_client_filelist = XFileList.LoadFileList(XABUtilities.GetResPath(EnumFileLocation.Client, EnumBundleType.Hotfix));
            m_origin_filelist = XFileList.LoadFileList(XABUtilities.GetResPath(EnumFileLocation.Stream, EnumBundleType.Hotfix));
        }

        public override void Leave(XAssetManagerOrdinary obj)
        {
            
        }

        public override EnumXTaskResult Execute(XAssetManagerOrdinary obj, float elapsedTime)
        {
            if (string.IsNullOrEmpty(m_serverAddress))
                return EnumXTaskResult.Failure;
            if (m_server_filelist == null)
                return EnumXTaskResult.Execute;

            var hotfixInfo = obj.AssetInfoManager.hotfixInfo;
            _UpdateHotfixInfo(hotfixInfo, m_server_filelist, m_client_filelist, m_origin_filelist);
            XDebug.Log(XABConst.Tag, hotfixInfo.ToLog());

            return EnumXTaskResult.Success;
        }
        //设置热更信息
        protected void _UpdateHotfixInfo(XABAssetInfoManager.HotfixInfo hotfixInfo, XFileList server, XFileList client, XFileList origin)
        {
            //服务器清单
            var serverDatas = server.GetDatas();
            //客户端清单
            var clientDatas = client.GetDatas();
            //原始清单
            var originDatas = origin.GetDatas();

            hotfixInfo.Reset();
            hotfixInfo.client_filelist = client;

            //检查更新资源
            foreach (var fileInfo in serverDatas.Values)
            {
                var fileName = fileInfo.name;
                if (clientDatas.ContainsKey(fileInfo.name) && clientDatas[fileName].md5 == fileInfo.md5)
                {
                    continue;
                }
                if (originDatas.ContainsKey(fileInfo.name) && originDatas[fileName].md5 == fileInfo.md5)
                {
                    continue;
                }
                hotfixInfo.download_list.Add(fileName);
                hotfixInfo.download_size += fileInfo.length;
            }

            //检查删除资源
            foreach (var fileInfo in clientDatas.Values)
            {
                var fileName = fileInfo.name;
                if (serverDatas.ContainsKey(fileName))
                    continue;
                hotfixInfo.deleteXX_list.Add(fileName);
            }
        }
    }
}
