using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    public class XBTTaskLog : XBTTask
    {
        public override void OnEnter(object obj)
        {
        }

        public override void OnLeave(object obj)
        {
        }

        public override EnumTaskStatus OnUpdate(object obj, float elapsedTime)
        {
            XDebug.Log(XBTConst.Tag, "testtesttest!!");
            return EnumTaskStatus.Success;
        }
    }
}

