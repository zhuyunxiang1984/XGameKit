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
        protected EnumJobState m_state;
        protected XAssetManagerOrdinary m_manager;
        public override void Enter(XAssetManagerOrdinary obj)
        {
            m_manager = obj;
            m_index = 0;
            m_state = EnumJobState.None;
            var assetInfoManager = obj.AssetInfoManager;
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
                _Download(hotfixInfo, m_index);
            }

            if (m_state == EnumJobState.Done)
            {
                m_index += 1;
                if (m_index >= hotfixInfo.download_list.Count)
                    return EnumXTaskResult.Success;
                _Download(hotfixInfo, m_index);
            }
            return EnumXTaskResult.Execute;
        }

        protected void _Download(XABAssetInfoManager.HotfixInfo hotfixInfo, int index)
        {
            m_state = EnumJobState.Process;
            var name = hotfixInfo.download_list[index];
            var url = $"{m_serverAddress}/{name}";
            XWebRequestManagerSingle.GetUrl(url,
                delegate(string error, byte[] responseData)
                {
                    _OnDownload(name, responseData);
                });
        }

        protected void _OnDownload(string name, byte[] data)
        {
            m_state = EnumJobState.Done;

            if (data == null)
                return;
            
            m_manager.AssetInfoManager.SetLocation(name, EnumFileLocation.Client);
            var path = XABUtilities.GetResPath(EnumFileLocation.Client, EnumBundleType.Hotfix);
            XUtilities.SaveFile($"{path}/{name}", data);
        }
    }


}
