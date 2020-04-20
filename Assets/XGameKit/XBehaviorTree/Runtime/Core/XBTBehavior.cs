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
        protected bool m_start;

        public XBTBehavior()
        {
            m_node = null;
            m_task = null;
            m_status = EnumTaskStatus.None;
        }

        public void Start(XBTNode node, object obj)
        {
            m_node = node;
            m_task = XBTClassFactory.Alloc<IXBTTask>(m_node.taskClassName);
            m_task.SetNode(node);
            m_status = EnumTaskStatus.Running;
            m_start = false;
        }

        public void Stop(object obj)
        {
            if (m_task != null)
            {
                m_task.Leave(obj);
                XBTClassFactory.Free(m_task);
                m_task = null;
            }
            m_status = EnumTaskStatus.None;
        }
        public void Reset(object obj)
        {
            Stop(obj);
            Start(m_node, obj);
        }
        public EnumTaskStatus Update(object obj, float elapsedTime)
        {
            if (m_status != EnumTaskStatus.Running)
                return m_status;
            if (!m_start)
            {
                m_task.Enter(obj);
                m_start = true;
            }
            m_status = m_task.Update(obj, elapsedTime);
            if (m_status == EnumTaskStatus.Success ||
                m_status == EnumTaskStatus.Failure)
            {
                m_task.Leave(obj);
                XBTClassFactory.Free(m_task);
                m_task = null;
            }
            return m_status;
        }
    }
}
