using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XGameKit.Core
{
    public class XStateMachine<T>
    {
        public const string Tag = "XStateMachine";
        
        protected XState<T> m_curState;
        protected string m_curr;
        protected string m_next;
        protected string m_default; //默认状态

        protected Dictionary<string, XState<T>> m_dictStates = new Dictionary<string, XState<T>>();

        public void AddState(string name, XState<T> state, bool initState = false)
        {
            if (m_dictStates.ContainsKey(name))
                return;
            state.SetMachine(this);
            m_dictStates.Add(name, state);
            if (initState)
            {
                m_default = name;
            }
        }
        public void Tick(T obj, float elapsedTime)
        {
            if (!string.IsNullOrEmpty(m_next))
            {
                XDebug.Log(Tag, $"state change {m_curr} -> {m_next}");
                _ChangeState(obj, m_next);
                m_next = string.Empty;
            }
            if (m_curState != null)
            {
                m_curState.OnUpdate(obj, elapsedTime);
            }
        }

        public string GetCurState()
        {
            return m_curr;
        }
        public void ChangeState(string state)
        {
            m_next = state;
        }

        public void ChangeToDefault()
        {
            m_next = m_default;
        }

        public void Start()
        {
            ChangeToDefault();
        }

        protected void _ChangeState(T obj, string state)
        {
            if (!m_dictStates.ContainsKey(state))
                return;
            var oldState = m_curState;
            var newState = m_dictStates[state];
            if (oldState != null)
                oldState.OnLeave(obj);
            if (newState != null)
                newState.OnEnter(obj);
            m_curr = state;
            m_curState = newState;
        }
    }

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
    }
}