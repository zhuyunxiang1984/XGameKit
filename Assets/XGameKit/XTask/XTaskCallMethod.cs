using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public class XTaskCallMethod : XTask
    {
        protected Action m_method;
        public XTaskCallMethod(Action method)
        {
            m_method = method;
        }
        public override void Enter()
        {
        }
        public override void Leave()
        {
        }
        public override float Tick(float elapsedTime)
        {
            m_method?.Invoke();
            return 1f;
        }
    }

}
