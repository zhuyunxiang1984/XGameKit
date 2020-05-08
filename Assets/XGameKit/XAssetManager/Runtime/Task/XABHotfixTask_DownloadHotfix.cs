using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XGameKit.XAssetManager
{
    public class XABHotfixTask_DownloadHotfix : XTask<XAssetManagerOrdinary>
    {
        protected string m_serverAddress;
        protected int m_index;
        protected string m_curDownload;
        protected EnumJobState m_state;
        protected XAssetManagerOrdinary m_manager;
        public override void Enter(XAssetManagerOrdinary obj)
        {
            m_manager = obj;
            m_index = 0;
            m_state = EnumJobState.None;
            m_serverAddress = obj.serverAddress;
#if UNITY_EDITOR
            m_serverAddress = EditorPrefs.GetString(XABConst.EKResUrl);
            if (string.IsNullOrEmpty(m_serverAddress))
            {
                Debug.LogError("没有配置url");
                return;
            }
#endif
        }

        public override void Leave(XAssetManagerOrdinary obj)
        {
            
        }

        public override EnumXTaskResult Execute(XAssetManagerOrdinary obj, float elapsedTime)
        {
            var hotfixInfo = obj.AssetInfoManager.hotfixInfo;
            if (hotfixInfo.download_list.Count < 1)
                return EnumXTaskResult.Success;
            if (m_state == EnumJobState.None)
            {
                m_index = 0;
                _Download(hotfixInfo.download_list[m_index]);
            }

            if (m_state == EnumJobState.Done)
            {
                m_index += 1;
                if (m_index >= hotfixInfo.download_list.Count)
                    return EnumXTaskResult.Success;
                _Download(hotfixInfo.download_list[m_index]);
            }
            return EnumXTaskResult.Execute;
        }

        protected void _Download(string name)
        {
            m_state = EnumJobState.Process;
            m_curDownload = name;
            var url = $"{m_serverAddress}/{name}";
            XWebRequestManagerSingle.GetUrl(url,
                _OnDownloadComplete, "", _OnDowloadProgress);
        }

        protected void _OnDownloadComplete(string error, byte[] data)
        {
            m_state = EnumJobState.Done;

            if (data == null)
                return;
            var name = m_curDownload;
            m_manager.AssetInfoManager.SetLocation(name, EnumFileLocation.Client);
            
            //保存下载文件
            var path = XABUtilities.GetResPath(EnumFileLocation.Client, EnumBundleType.Hotfix);
            XUtilities.SaveFile($"{path}/{name}", data);

            var hotfixInfo = m_manager.AssetInfoManager.hotfixInfo;
            //保存filelist
            var client_filelist = hotfixInfo.client_filelist;
            var server_filelist = hotfixInfo.server_filelist;
            var serverFileInfo = server_filelist.GetFileInfo(name);
            var clientFileInfo = client_filelist.GetFileInfo(name);
            if (clientFileInfo == null)
            {
                clientFileInfo = new XFileList.XFileInfo();
                client_filelist.AddFileInfo(name, clientFileInfo);
            }
            clientFileInfo.name = name;
            clientFileInfo.path = serverFileInfo.path;
            clientFileInfo.length = serverFileInfo.length;
            clientFileInfo.md5 = serverFileInfo.md5;
            XFileList.SaveFileList(XABUtilities.GetResPath(EnumFileLocation.Client, EnumBundleType.Hotfix), client_filelist);
        }

        protected void _OnDowloadProgress(float progress)
        {
            var hotfixInfo = m_manager.AssetInfoManager.hotfixInfo;
            var server_filelist = hotfixInfo.server_filelist;
            var fileInfo = server_filelist.GetFileInfo(m_curDownload);
            XDebug.Log(XABConst.Tag, $"download {m_curDownload} {fileInfo.length * progress}");
        }
    }


}
