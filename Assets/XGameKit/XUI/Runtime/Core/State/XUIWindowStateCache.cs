using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    
//缓存状态
    public class XUIWindowStateCache : XState<XUIWindow>
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

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (m_time > 0)
            {
                m_timeCounter += elapsedTime;
                if (m_timeCounter >= m_time)
                {
                    obj.stateMachine.ChangeState(XUIWindowStateMachine.stUnload);
                }
            }
        }
        public override string Transition(XUIWindow obj)
        {
            if (obj.isShow)
                return XUIWindowStateMachine.stShow;
            return String.Empty;
        }
    }
}
