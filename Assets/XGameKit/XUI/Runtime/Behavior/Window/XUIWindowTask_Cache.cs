using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.XBehaviorTree;

namespace XGameKit.XUI
{
    [BTTaskMemo("[XUI]缓存窗口")]
    public class XUIWindowTask_Cache : XBTTask<XUIWindow>
    {
        protected float m_time;
        protected float m_timeCounter;
        public override void OnEnter(XUIWindow obj)
        {
            m_time = obj.cacheTime;
            m_timeCounter = 0f;
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override EnumTaskStatus OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (m_time > 0)
            {
                m_timeCounter += elapsedTime;
                if (m_timeCounter >= m_time)
                {
                    return EnumTaskStatus.Success;
                }
            }
            return EnumTaskStatus.Running;
        }
    }
}