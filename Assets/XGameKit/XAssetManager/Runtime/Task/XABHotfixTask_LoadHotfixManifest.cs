using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    public class XABHotfixTask_LoadHotfixManifest : XTask<XAssetManagerOrdinary>
    {
        public override void Enter(XAssetManagerOrdinary obj)
        {
            var clientHotfixPath = XABUtilities.GetResPath(EnumFileLocation.Client, EnumBundleType.Hotfix);
            if (XABUtilities.ExistManifest(clientHotfixPath))
            {
                obj.AssetInfoManager.SetHotfixManifest(XABUtilities.ReadManifest(clientHotfixPath));
                return;
            }
            var streamHotfixPath = XABUtilities.GetResPath(EnumFileLocation.Stream, EnumBundleType.Hotfix);
            if (XABUtilities.ExistManifest(streamHotfixPath))
            {
                obj.AssetInfoManager.SetHotfixManifest(XABUtilities.ReadManifest(streamHotfixPath));
                return;
            }
        }
        public override void Leave(XAssetManagerOrdinary obj)
        {
            
            
        }

        public override EnumXTaskResult Execute(XAssetManagerOrdinary obj, float elapsedTime)
        {
            return EnumXTaskResult.Success;
        }
    }


}
