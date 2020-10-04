using System;
using System.Collections.Generic;

namespace XGameKit.Core
{
    public class XEventManager : IXService
    {
        //事件列表
        private Queue<XEvent> m_Events = new Queue<XEvent>();

        //事件监听列表
        private Dictionary<string, Delegate> m_dictHandlers = new Dictionary<string,  Delegate>();

        private XEventPool m_EventPool = new XEventPool();

        public void Dispose()
        {
            Clear();
            m_EventPool.Dispose();
            m_dictHandlers.Clear();
        }
        //清除所有事件
        public void Clear()
        {
            foreach (var evt in m_Events)
            {
                evt.Reset();
                m_EventPool.Recycle(evt);
            }
            m_Events.Clear();
        }

        public void Tick(float elapsedTime)
        {
            while (m_Events.Count > 0)
            {
                var evt = m_Events.Dequeue();
                _HandleEvent(evt);
                m_EventPool.Recycle(evt);
            }
        }
        //处理事件
        private void _HandleEvent(XEvent evt)
        {
            if (!m_dictHandlers.ContainsKey(evt.Name))
                return;
            var handler = m_dictHandlers[evt.Name];
            if (handler == null)
                return;
            evt.HandleEvent(handler);
        }
        
        private T _CreatEvent<T>() where T : XEvent, new()
        {
            return m_EventPool.Get<T>();
        }

        private void _AddHandler(string name, Delegate handler)
        {
            if (m_dictHandlers.ContainsKey(name))
            {
                m_dictHandlers[name] = Delegate.Combine(m_dictHandlers[name], handler);
            }
            else
            {
                m_dictHandlers.Add(name, handler);
            }
        }

        private void _RemoveHandler(string name, Delegate handler)
        {
            if (!m_dictHandlers.ContainsKey(name))
                return;
            m_dictHandlers[name] = Delegate.Remove(m_dictHandlers[name], handler);
        }

        #region 抛出事件

        public void PostEvent<T>(string eventName, Action<T> callback) where T : XEvent, new()
        {
            var evt = _CreatEvent<T>();
            evt.SetName(eventName);
            callback?.Invoke(evt);
            m_Events.Enqueue(evt);
        }

        public void PostEvent<T>(string eventName, T param)
        {
            var evt = _CreatEvent<XEvent<T>>();
            evt.SetName(eventName);
            evt.Param = param;
            m_Events.Enqueue(evt);
        }
        public void PostEvent<T1, T2>(string eventName, T1 param1, T2 param2)
        {
            var evt = _CreatEvent<XEvent<T1, T2>>();
            evt.SetName(eventName);
            evt.Param1 = param1;
            evt.Param2 = param2;
            m_Events.Enqueue(evt);
        }
        public void PostEvent<T1, T2, T3>(string eventName, T1 param1, T2 param2, T3 param3)
        {
            var evt = _CreatEvent<XEvent<T1, T2, T3>>();
            evt.SetName(eventName);
            evt.Param1 = param1;
            evt.Param2 = param2;
            evt.Param3 = param3;
            m_Events.Enqueue(evt);
        }
        public void PostEvent<T1, T2, T3, T4>(string eventName, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            var evt = _CreatEvent<XEvent<T1, T2, T3, T4>>();
            evt.SetName(eventName);
            evt.Param1 = param1;
            evt.Param2 = param2;
            evt.Param3 = param3;
            evt.Param4 = param4;
            m_Events.Enqueue(evt);
        }
        public void PostEvent<T1, T2, T3, T4, T5>(string eventName, T1 param1, T2 param2,T3 param3, T4 param4, T5 param5)
        {
            var evt = _CreatEvent<XEvent<T1, T2, T3, T4, T5>>();
            evt.SetName(eventName);
            evt.Param1 = param1;
            evt.Param2 = param2;
            evt.Param3 = param3;
            evt.Param4 = param4;
            evt.Param5 = param5;
            m_Events.Enqueue(evt);
        }

        #endregion

        #region 注册/注销事件

        public void AddListener<T>(string eventName, Action<T> handler)
        {
            _AddHandler(eventName, handler);
        }
        public void AddListener<T1, T2>(string eventName, Action<T1, T2> handler)
        {
            _AddHandler(eventName, handler);
        }
        public void AddListener<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler)
        {
            _AddHandler(eventName, handler);
        }
        public void AddListener<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> handler)
        {
            _AddHandler(eventName, handler);
        }
        public void AddListener<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> handler)
        {
            _AddHandler(eventName, handler);
        }
        
        public void RemoveListener<T>(string eventName, Action<T> handler)
        {
            _RemoveHandler(eventName, handler);
        }
        public void RemoveListener<T1, T2>(string eventName, Action<T1, T2> handler)
        {
            _RemoveHandler(eventName, handler);
        }
        public void RemoveListener<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler)
        {
            _RemoveHandler(eventName, handler);
        }
        public void RemoveListener<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> handler)
        {
            _RemoveHandler(eventName, handler);
        }
        public void RemoveListener<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> handler)
        {
            _RemoveHandler(eventName, handler);
        }

        #endregion
        
        
    }
}
