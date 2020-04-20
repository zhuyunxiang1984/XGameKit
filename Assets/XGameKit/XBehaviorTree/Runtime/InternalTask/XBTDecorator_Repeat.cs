using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    /// <summary>
    /// 循环执行子节点
    /// </summary>
    public class XBTDecorator_Repeat : XBTCommonTask
    {
        protected bool m_start;
        protected XBTBehavior m_beahvior = new XBTBehavior();
        
        public override void OnEnter(object obj)
        {
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
                m_beahvior.Start(m_node.children[0], obj);
                m_start = true;
            }
            var status = m_beahvior.Update(obj, elapsedTime);
            if (status != EnumTaskStatus.Running)
            {
                m_start = false;
            }
            return EnumTaskStatus.Running;
        }
    }

}