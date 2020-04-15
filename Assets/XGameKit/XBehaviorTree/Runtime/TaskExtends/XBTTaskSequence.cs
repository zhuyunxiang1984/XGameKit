using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    public class XBTTaskSequence : XBTCommonTask
    {
        protected int m_index;
        protected XBTBehavior m_beahvior = new XBTBehavior();

        public override void SetNode(XBTNode node)
        {
            base.SetNode(node);
        }

        public override void OnEnter(object obj)
        {
            m_index = -1;
        }

        public override void OnLeave(object obj)
        {
        }

        public override EnumTaskStatus OnUpdate(object obj, float elapsedTime)
        {
            if (m_node.children == null || m_node.children.Count < 1)
                return EnumTaskStatus.Success;
            if (m_index == -1)
            {
                m_index = 0;
                m_beahvior.Start(m_node.children[m_index], obj);
            }
            var status = m_beahvior.Update(obj, elapsedTime);
            if (status == EnumTaskStatus.Success)
            {
                if (m_index >= m_node.children.Count - 1)
                {
                    return EnumTaskStatus.Success;
                }
                m_index = m_index + 1;
                m_beahvior.Start(m_node.children[m_index], obj);
                return EnumTaskStatus.Execute;
            }
            return status;
        }
    }
}