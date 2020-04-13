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
            m_node = node;
            m_task = XBTClassFactory.Alloc<IXBTTask>(m_node.taskClassName);
            m_task.SetNode(node);
            m_task.Enter(obj);
            m_status = EnumTaskStatus.Execute;
        }

        public void Stop(object obj)
        {
            //回收task
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
            Start(m_node, obj);
        }
        public EnumTaskStatus Update(object obj, float elapsedTime)
        {
            if (m_task == null)
                return m_status;
            switch (m_status)
            {
                case EnumTaskStatus.Execute:
                    m_status = m_task.Update(obj, elapsedTime);
                    if (m_status == EnumTaskStatus.Success ||
                        m_status == EnumTaskStatus.Failure)
                    {
                        m_task.Leave(obj);
                        XBTClassFactory.Free(m_task);
                        m_task = null;
                    }
                    break;
            }
            return m_status;
        }
    }
}
