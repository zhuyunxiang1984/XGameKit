using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public class XTaskWaitSeconds : XTask
    {
        protected float m_time;
        protected float m_timeCounter;
        public XTaskWaitSeconds(float time)
        {
            m_time = time;
        }
        public override void Enter()
        {
            XDebug.Log(XTaskConst.Tag, $"wait {m_time.ToString("f2")} seconds");
            m_timeCounter = 0f;
        }
        public override void Leave()
        {
        }
        public override float Tick(float elapsedTime)
        {
            m_timeCounter += elapsedTime;
            if (m_timeCounter < m_time)
                return Mathf.Clamp01(m_timeCounter / m_time);
            m_timeCounter = m_time;
            return 1f;
        }
    }
}
