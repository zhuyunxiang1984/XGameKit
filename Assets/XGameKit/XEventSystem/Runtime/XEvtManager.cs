using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public class XEvtManager : IXService
    {
        //事件列表
        private Queue<XEvent> _events = new Queue<XEvent>();

        //事件监听列表
        private Dictionary<string, Action<XEvent>> _listeners = new Dictionary<string,  Action<XEvent>>();

        public void Dispose()
        {
            Clear();
            _listeners.Clear();
        }

        public void Tick(float elapsedTime)
        {
            while (_events.Count > 0)
            {
                var evt = _events.Dequeue();
                _HandleEvent(evt);
                XObjectPool.Free(evt);
            }
        }
        
        //清除所有事件
        public void Clear()
        {
            foreach (var evt in _events)
            {
                evt.Reset();
                XObjectPool.Free(evt);
            }
            _events.Clear();
        }
        //注册监听
        public void AddListener<T>(Action<XEvent> callback) where T : XEvent
        {
            string evtName = typeof(T).Name;
            if (_listeners.ContainsKey(evtName))
            {
                _listeners[evtName] += callback;
            }
            else
            {
                _listeners.Add(evtName, callback);
            }
        }
        //注销监听
        public void DelListener<T>(Action<XEvent> callback) where T : XEvent
        {
            string evtName = typeof(T).Name;
            if (!_listeners.ContainsKey(evtName))
                return;
            _listeners[evtName] -= callback;
        }
        public T GetEvent<T>() where T : XEvent, new()
        {
            var obj = XObjectPool.Alloc<T>();
            obj.SetName(typeof(T).Name);
            return obj;
        }
        public void PostEvent(XEvent evt)
        {
            _events.Enqueue(evt);
        }
        
        //处理事件
        public void _HandleEvent(XEvent evt)
        {
            if (!_listeners.ContainsKey(evt.name))
                return;
            _listeners[evt.name]?.Invoke(evt);
        }
    }
}
