using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public class XTaskCallMethod<T> : XTask<T>
    {
        protected Action<T> m_method;
        public XTaskCallMethod(Action<T> method)
        {
            m_method = method;
        }
        public override void Enter(T obj)
        {
        }
        public override void Leave(T obj)
        {
        }
        public override EnumXTaskResult Execute(T obj, float elapsedTime)
        {
            m_method?.Invoke(obj);
            return EnumXTaskResult.Success;
        }
    }

}
