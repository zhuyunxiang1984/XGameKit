using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    public class XABInitTaskSchedule : XTaskSchedule<XAssetManagerOrdinary>
    {
        public XABInitTaskSchedule()
        {
            AddTask(new XABInitTask_CheckFileList());
        }
    }
}
