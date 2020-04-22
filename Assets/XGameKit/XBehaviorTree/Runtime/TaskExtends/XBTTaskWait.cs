using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    [BTTaskMemo("等待time(秒)")]
    public class XBTTaskWait : XBTCommonTask<XBTTaskWait.Param>
    {
        public class Param
        {
            public float time;
        }
        protected float m_time;
        protected float m_timecounter;
        
        public override void OnEnter(object obj)
        {
            m_time = m_param.time;
            m_timecounter = 0f;
            XDebug.Log(XBTConst.Tag, $"start wait {m_time}");
        }

        public override void OnLeave(object obj)
        {
            XDebug.Log(XBTConst.Tag, $"finish wait {m_time}");
        }

        public override EnumTaskStatus OnUpdate(object obj, float elapsedTime)
        {
            if (m_time > 0)
            {
                m_timecounter += elapsedTime;
                if (m_timecounter >= m_time)
                {
                    return EnumTaskStatus.Success;
                }
            }
            return EnumTaskStatus.Running;
        }
    }


}