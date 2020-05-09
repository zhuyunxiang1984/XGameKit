using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    public class XABHotfixTaskSchedule : XTaskSchedule<XAssetManagerOrdinary>
    {
        public XABHotfixTaskSchedule()
        {
            AddTask(new XABHotfixTask_CheckHotfix());
            AddTask(new XABHotfixTask_DownloadHotfix());
            AddTask(new XABHotfixTask_LoadHotfixManifest());
        }
    }
    
    public class XABHotfixTaskCheckSchedule : XTaskSchedule<XAssetManagerOrdinary>
    {
        public XABHotfixTaskCheckSchedule()
        {
            AddTask(new XABHotfixTask_CheckHotfix());
        }
    }
}
