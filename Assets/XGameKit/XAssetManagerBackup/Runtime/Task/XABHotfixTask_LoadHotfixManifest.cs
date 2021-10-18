using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager.Backup
{
    public class XABHotfixTask_LoadHotfixManifest : XTask
    {
        protected XAssetManagerOrdinary m_manager;

        public XABHotfixTask_LoadHotfixManifest(XAssetManagerOrdinary manager)
        {
            m_manager = manager;
        }

        public override void Enter()
        {
            var clientHotfixPath = XABUtilities.GetResPath(EnumFileLocation.Client, EnumBundleType.Hotfix);
            if (XABUtilities.ExistManifest(clientHotfixPath))
            {
                m_manager.AssetInfoManager.SetHotfixManifest(XABUtilities.ReadManifest(clientHotfixPath));
                return;
            }
            var streamHotfixPath = XABUtilities.GetResPath(EnumFileLocation.Stream, EnumBundleType.Hotfix);
            if (XABUtilities.ExistManifest(streamHotfixPath))
            {
                m_manager.AssetInfoManager.SetHotfixManifest(XABUtilities.ReadManifest(streamHotfixPath));
                return;
            }
        }
        public override void Leave()
        {
            
            
        }

        public override float Tick(float elapsedTime)
        {
            return 1f;
        }
    }


}
