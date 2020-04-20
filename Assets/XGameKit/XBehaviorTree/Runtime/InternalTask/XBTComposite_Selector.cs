using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    /// <summary>
    /// 选择节点
    /// 逐个执行子节点
    /// 一个成功，返回成功
    /// 全部失败，返回失败
    /// </summary>
    public class XBTComposite_Selector : XBTCommonTask
    {
        protected int m_index;
        protected bool m_start;
        protected XBTBehavior m_beahvior = new XBTBehavior();

        public override void OnEnter(object obj)
        {
            m_index = 0;
            m_start = false;
        }

        public override void OnLeave(object obj)
        {
            m_beahvior.Stop(obj);
        }

        public override EnumTaskStatus OnUpdate(object obj, float elapsedTime)
        {
            if (m_node.children == null || m_node.children.Count < 1)
                return EnumTaskStatus.Success;
            if (!m_start)
            {
                m_beahvior.Start(m_node.children[m_index], obj);
                m_start = true;
            }
            var status = m_beahvior.Update(obj, elapsedTime);
            if (status == EnumTaskStatus.Failure)
            {
                if (m_index >= m_node.children.Count - 1)
                {
                    return EnumTaskStatus.Failure;
                }
                m_index = m_index + 1;
                m_start = false;
                return EnumTaskStatus.Running;
            }
            return status;
        }
    }
}