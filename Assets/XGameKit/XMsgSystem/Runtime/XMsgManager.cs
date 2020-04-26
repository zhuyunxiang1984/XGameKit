using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public enum EnumXMsgMode
    {
        Normal,    //向子节点传递
        Reverse,   //向父节点传递
    }
    public class XMsgManager : IXService
    {
        public struct MsgData
        {
            public XMessage msg;
            public EnumXMsgMode mode;
        }
        
        //消息列表
        private Queue<MsgData> _msgDatas = new Queue<MsgData>();
        //消息处理
        private Dictionary<string, Func<XMessage, bool>> _handles = new Dictionary<string, Func<XMessage, bool>>();
        //父节点
        private XMsgManager _parent;
        //子节点
        private List<XMsgManager> _children = new List<XMsgManager>();

        public void Dispose()
        {
            Clear();
            
            if (_parent != null)
            {
                _parent._DelChild(this);
                _parent = null;
            }
            foreach (var child in _children)
            {
                child._SetParent(null);
            }
            _children.Clear();
        }
        
        //清除所有消息
        public void Clear()
        {
            foreach (var msgData in _msgDatas)
            {
                msgData.msg.Reset();
                XObjectPool.Free(msgData.msg);
            }
            _msgDatas.Clear();
        }
        public void Tick(float elapsedTime)
        {
            while (_msgDatas.Count > 0)
            {
                var msgData = _msgDatas.Dequeue();
                if (_HandleMsg(msgData.msg) || !_TransitMsg(msgData))
                {
                    XObjectPool.Free(msgData.msg);
                }
            }
        }

        public void Register<T>(Func<XMessage, bool> callback) where T : XMessage
        {
            string msgName = typeof(T).Name;
            if (_handles.ContainsKey(msgName))
            {
                _handles[msgName] = callback;
            }
            else
            {
                _handles.Add(msgName, callback);
            }
        }
        public void Unregister<T>() where T : XMessage
        {
            string msgName = typeof(T).Name;
            if (!_handles.ContainsKey(msgName))
                return;
            _handles.Remove(msgName);
        }
        public T GetMsg<T>() where T : XMessage, new()
        {
            var obj = XObjectPool.Alloc<T>();
            obj.SetName(typeof(T).Name);
            return obj;
        }
        public void SendMsg(XMessage msg, EnumXMsgMode mode = EnumXMsgMode.Normal)
        {
            _msgDatas.Enqueue(new MsgData()
            {
                msg = msg,
                mode = mode,
            });
        }
        //处理消息
        protected bool _HandleMsg(XMessage msg)
        {
            if (!_handles.ContainsKey(msg.name))
                return false;
            if (_handles[msg.name] == null)
                return false;
            return _handles[msg.name].Invoke(msg);
        }
        //消息流转
        protected bool _TransitMsg(MsgData msgData)
        {
            bool success = false;
            switch (msgData.mode)
            {
                case EnumXMsgMode.Normal:
                    if (_children.Count > 0)
                    {
                        foreach (var child in _children)
                        {
                            child.SendMsg(msgData.msg, msgData.mode);
                        }
                        success = true;
                    }
                    break;
                case EnumXMsgMode.Reverse:
                    if (_parent != null)
                    {
                        _parent.SendMsg(msgData.msg, msgData.mode);
                        success = true;
                    }
                    break;
            }
            return success;
        }
        
        public static void Append(XMsgManager parent, XMsgManager child)
        {
            if (parent == null || child == null)
                return;
            parent._AddChild(child);
            child._SetParent(parent);
        }

        public static void Remove(XMsgManager parent, XMsgManager child)
        {
            if (parent == null || child == null)
                return;
            parent._DelChild(child);
            child._SetParent(null);
        }
        protected void _SetParent(XMsgManager parent)
        {
            _parent = parent;
        }
        protected void _AddChild(XMsgManager child)
        {
            if (_children.Contains(child))
                return;
            _children.Add(child);
        }
        protected void _DelChild(XMsgManager child)
        {
            if (!_children.Contains(child))
                return;
            _children.Remove(child);
        }

        
    }

}