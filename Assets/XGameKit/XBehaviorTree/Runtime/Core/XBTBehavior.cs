using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XBehaviorTree
{
    public class XBTBehavior
    {
        protected XBTNode m_node;
        protected IXBTTask m_task;
        protected EnumTaskStatus m_status;
        
        public XBTBehavior()
        {
            m_node = null;
            m_task = null;
            m_status = EnumTaskStatus.None;
        }
        public void Start(XBTNode node, object obj)
        {
            if (m_status == EnumTaskStatus.Running)
            {
                m_task.Leave(obj);
            }
            if (m_node == null || m_node != node)
            {
                if (m_task != null)
                {
                    XBTClassFactory.Free(m_task);
                    m_task = null;
                }
                m_task = XBTClassFactory.Alloc<IXBTTask>(node.taskClassName);
                m_task.SetNode(node);
                m_node = node;
            }
            m_task.Enter(obj);
            m_status = EnumTaskStatus.Running;
        }

        public void Stop(object obj)
        {
            if (m_node == null)
                return;
            if (m_status == EnumTaskStatus.Running)
            {
                m_task.Leave(obj);
            }
            XBTClassFactory.Free(m_task);
            m_task = null;
            m_node = null;
            m_status = EnumTaskStatus.None;
        }
        public EnumTaskStatus Update(object obj, float elapsedTime)
        {
            if (m_status != EnumTaskStatus.Running)
                return m_status;
            m_status = m_task.Update(obj, elapsedTime);
            if (m_status == EnumTaskStatus.Success ||
                m_status == EnumTaskStatus.Failure)
            {
                m_task.Leave(obj);
            }
            return m_status;
        }
    }
}
