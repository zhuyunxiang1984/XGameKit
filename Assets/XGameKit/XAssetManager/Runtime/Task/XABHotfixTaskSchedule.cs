using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    public class XABHotfixTaskSchedule : XTaskSchedule
    {
        public XABHotfixTaskSchedule(XAssetManagerOrdinary manager)
        {
            AddTask(new XABHotfixTask_CheckHotfix(manager));
            AddTask(new XABHotfixTask_DownloadHotfix(manager));
            AddTask(new XABHotfixTask_LoadHotfixManifest(manager));
        }
    }
    
    public class XABHotfixTaskCheckSchedule : XTaskSchedule
    {
        public XABHotfixTaskCheckSchedule(XAssetManagerOrdinary manager)
        {
            AddTask(new XABHotfixTask_CheckHotfix(manager));
        }
    }
}
