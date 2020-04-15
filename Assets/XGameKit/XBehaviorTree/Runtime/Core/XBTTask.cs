using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    public interface IXBTTask
    {
        void SetNode(XBTNode node);
        void Enter(object obj);
        void Leave(object obj);
        EnumTaskStatus Update(object obj, float elapsedTime);
    }
    //commontask 参数为object 需要自己强转
    public abstract class XBTCommonTask : IXBTTask
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
    public abstract class XBTCommonTask<Param> : XBTCommonTask where Param : class, new()
    {
        protected Param m_param = new Param();
        public override void SetNode(XBTNode node)
        {
            base.SetNode(node);
            XMonoVariableUtility.Inject(node.variables, ref m_param);
        }
    }
    
    //参数为T 可以指定类型自动强转
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
    public abstract class XBTTask<T, Param> : XBTTask<T>
        where T : class 
        where Param : class, new()
    {
        protected Param m_param = new Param();
        public override void SetNode(XBTNode node)
        {
            base.SetNode(node);
            XMonoVariableUtility.Inject(node.variables, ref m_param);
        }
    }
}