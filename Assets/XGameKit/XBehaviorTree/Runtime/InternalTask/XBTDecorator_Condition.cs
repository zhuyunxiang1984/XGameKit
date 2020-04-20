using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    /// <summary>
    /// 满足条件执行子节点并返回结果
    /// 不满足条件返回失败
    /// </summary>
    public abstract class XBTDecorator_Condition<T, Param> : XBTTask<T, Param>
        where T : class 
        where Param : class, new()
    {
        protected bool m_start;
        protected XBTBehavior m_beahvior = new XBTBehavior();

        public override void OnEnter(T obj)
        {
            m_start = false;
        }

        public override void OnLeave(T obj)
        {
            m_beahvior.Stop(obj);
        }

        public override EnumTaskStatus OnUpdate(T obj, float elapsedTime)
        {
            if (!_CheckCondition(obj))
                return EnumTaskStatus.Failure;
            if (m_node.children == null || m_node.children.Count < 1)
                return EnumTaskStatus.Success;
            if (!m_start)
            {
                m_beahvior.Start(m_node.children[0], obj);
                m_start = true;
            }
            return m_beahvior.Update(obj, elapsedTime);
        }
        protected abstract bool _CheckCondition(T obj);
    }

}