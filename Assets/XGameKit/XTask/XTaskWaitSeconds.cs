using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public class XTaskWaitSeconds<T> : XTask<T>
    {
        protected float m_time;
        protected float m_timeCounter;
        public XTaskWaitSeconds(float time)
        {
            m_time = time;
        }
        public override void Enter(T obj)
        {
            XDebug.Log(XTaskConst.Tag, $"wait {m_time.ToString("f2")} seconds");
            m_timeCounter = 0f;
        }
        public override void Leave(T obj)
        {
        }
        public override EnumXTaskResult Execute(T obj, float elapsedTime)
        {
            m_timeCounter += elapsedTime;
            if (m_timeCounter < m_time)
                return EnumXTaskResult.Execute;
            m_timeCounter = m_time;
            return EnumXTaskResult.Success;
        }
    }
}
