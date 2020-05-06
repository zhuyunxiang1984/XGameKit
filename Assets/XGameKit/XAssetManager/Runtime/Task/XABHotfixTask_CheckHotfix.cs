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
        protected XFileList m_stream_filelist;
        
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
            m_stream_filelist = XFileList.LoadFileList(XABUtilities.GetResPath(EnumFileLocation.Stream, EnumBundleType.Hotfix));
        }

        public override void Leave(XAssetManagerOrdinary obj)
        {
            
        }

        public override EnumXTaskResult Execute(XAssetManagerOrdinary obj, float elapsedTime)
        {
            if (string.IsNullOrEmpty(m_serverAddress))
                return EnumXTaskResult.Failure;
            var assetInfoManager = obj.AssetInfoManager;
            if (m_server_filelist == null)
                return EnumXTaskResult.Execute;
            
            var deleteList = _GetDeleteFiles(m_server_filelist, m_client_filelist);
            var downloads = _GetDownloadFiles(m_server_filelist, m_client_filelist, m_stream_filelist);

            var logger = XDebug.CreateMutiLogger(XABConst.Tag);
            logger.Append("===比较文件清单===");
            logger.Append("删除列表");
            foreach (var temp in deleteList)
            {
                logger.Append(temp);
            }
            logger.Append("下载列表");
            foreach (var temp in downloads)
            {
                logger.Append(temp);
            }
            logger.Log();
            return EnumXTaskResult.Success;
        }
        protected List<string> _GetDeleteFiles(XFileList remote, XFileList document)
        {
            var result = new List<string>();
            var remoteDatas = remote.GetDatas();
            var documentDatas = document.GetDatas();
            foreach (var fileInfo in documentDatas.Values)
            {
                var fileName = fileInfo.name;
                if (remoteDatas.ContainsKey(fileName))
                    continue;
                result.Add(fileName);
            }
            return result;
        }
        protected List<string> _GetDownloadFiles(XFileList remote, XFileList document, XFileList streamingAssets)
        {
            var result = new List<string>();
            var remoteDatas = remote.GetDatas();
            var documentDatas = document.GetDatas();
            var streamingAssetsDatas = streamingAssets.GetDatas();

            foreach (var fileInfo in remoteDatas.Values)
            {
                var fileName = fileInfo.name;
                if (documentDatas.ContainsKey(fileInfo.name))
                {
                    if (documentDatas[fileName].md5 != fileInfo.md5)
                    {
                        result.Add(fileName);
                    }
                }
                else if (streamingAssetsDatas.ContainsKey(fileInfo.name))
                {
                    if (streamingAssetsDatas[fileName].md5 != fileInfo.md5)
                    {
                        result.Add(fileName);
                    }
                }
                else
                {
                    //新增资源
                    result.Add(fileName);
                }
            }
            return result;
        }
    }
}
