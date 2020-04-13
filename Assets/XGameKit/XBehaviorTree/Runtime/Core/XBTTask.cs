using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace XGameKit.XBehaviorTree
{
    public interface IXBTTask
    {
        void SetNode(XBTNode node);
        void Enter(object obj);
        void Leave(object obj);
        EnumTaskStatus Update(object obj, float elapsedTime);
    }

    public abstract class XBTTask : IXBTTask
    {
        protected XBTNode m_node;
        public virtual void SetNode(XBTNode node)
        {
            m_node = node;
        }
        public void Enter(object obj)
        {
            OnEnter(obj);
        }
        public void Leave(object obj)
        {
            OnLeave(obj);
        }
        public EnumTaskStatus Update(object obj, float elapsedTime)
        {
            return OnUpdate(obj, elapsedTime);
        }
        public abstract void OnEnter(object obj);
        public abstract void OnLeave(object obj);
        public abstract EnumTaskStatus OnUpdate(object obj, float elapsedTime);
    }
    public abstract class XBTTask<T> : IXBTTask where T : class
    {
        protected XBTNode m_node;
        public virtual void SetNode(XBTNode node)
        {
            m_node = node;
        }
        public void Enter(object obj)
        {
            var t = _Convert(obj);
            if (t == null)
                return;
            OnEnter(t);
        }
        public void Leave(object obj)
        {
            var t = _Convert(obj);
            if (t == null)
                return;
            OnLeave(t);
        }
        public EnumTaskStatus Update(object obj, float elapsedTime)
        {
            var t = _Convert(obj);
            if (t == null)
                return EnumTaskStatus.Failure;
            return OnUpdate(t, elapsedTime);
        }

        T _Convert(object obj)
        {
            var t = obj as T;
            if (t == null)
            {
                Debug.LogError($"obj类型转换失败 {obj.GetType().Name} -> {typeof(T).Name}");
                return null;
            }
            return t;
        }
        public abstract void OnEnter(T obj);
        public abstract void OnLeave(T obj);
        public abstract EnumTaskStatus OnUpdate(T obj, float elapsedTime);
    }
    
}