using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    public class XBTTaskSequence : XBTTask
    {
        protected int m_index;
        protected XBTBehavior m_beahvior = new XBTBehavior();
        protected XBTNodeComposite m_compositeNode;

        public override void SetNode(XBTNode node)
        {
            base.SetNode(node);
            m_compositeNode = m_node as XBTNodeComposite;
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
            if (m_compositeNode.m_children.Count < 1)
                return EnumTaskStatus.Success;
            if (m_index == -1)
            {
                m_index = 0;
                m_beahvior.Start(m_compositeNode.m_children[m_index], obj);
            }
            var status = m_beahvior.Update(obj, elapsedTime);
            if (status == EnumTaskStatus.Success)
            {
                if (m_index >= m_compositeNode.m_children.Count - 1)
                {
                    return EnumTaskStatus.Success;
                }
                m_index = m_index + 1;
                m_beahvior.Start(m_compositeNode.m_children[m_index], obj);
                return EnumTaskStatus.Execute;
            }
            return status;
        }
    }
}