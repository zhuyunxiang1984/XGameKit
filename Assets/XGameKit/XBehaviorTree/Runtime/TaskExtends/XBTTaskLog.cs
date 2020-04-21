using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    [BTTaskMemo("输出Log")]
    public class XBTTaskLog : XBTCommonTask<XBTTaskLog.Param>
    {
        public class Param
        {
            public string message;
        }
        public override void OnEnter(object obj)
        {
        }

        public override void OnLeave(object obj)
        {
        }

        public override EnumTaskStatus OnUpdate(object obj, float elapsedTime)
        {
            XDebug.Log(XBTConst.Tag, m_param.message);
            return EnumTaskStatus.Success;
        }
    }
}

