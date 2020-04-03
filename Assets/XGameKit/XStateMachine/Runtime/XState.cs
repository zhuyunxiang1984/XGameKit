using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    
    public abstract class XState<T>
    {
        protected XStateMachine<T> m_machine;

        public void SetMachine(XStateMachine<T> machine)
        {
            m_machine = machine;
        }
        public abstract void OnEnter(T obj);
        public abstract void OnLeave(T obj);
        public abstract void OnUpdate(T obj, float elapsedTime);
        public abstract string Transition(T obj);
    }

    public class XStateEmpty<T> : XState<T>
    {
        public override void OnEnter(T obj)
        {
        }
        public override void OnLeave(T obj)
        {
        }
        public override void OnUpdate(T obj, float elapsedTime)
        {
        }

        public override string Transition(T obj)
        {
            return string.Empty;
        }
    }
}