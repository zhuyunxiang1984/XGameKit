using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    /// <summary>
    /// 顺序节点
    /// 逐个执行子节点
    /// 全部成功，返回成功
    /// 一个失败，返回失败
    /// </summary>
    [BTTaskMemo("[组合]顺序节点")]
    public class XBTComposite_Sequence : XBTCommonTask
    {
        protected XBTBehavior m_beahvior = new XBTBehavior();
        protected int m_index;
        protected bool m_runEnter;
        public override void OnEnter(object obj)
        {
            m_index = 0;
            m_runEnter = true;
        }

        public override void OnLeave(object obj)
        {
            m_beahvior.Stop(obj);
        }

        public override EnumTaskStatus OnUpdate(object obj, float elapsedTime)
        {
            if (m_node.children == null || m_node.children.Count < 1)
                return EnumTaskStatus.Success;

            while (m_index < m_node.children.Count)
            {
                if (m_runEnter)
                {
                    XDebug.Log(XBTConst.Tag,$"顺序节点 {m_index} {m_node.children[m_index].taskClassName}");
                    m_beahvior.Start(m_node.children[m_index], obj);
                    m_runEnter = false;
                }
                var status = m_beahvior.Update(obj, elapsedTime);
                if (status == EnumTaskStatus.Success)
                {
                    m_index = m_index + 1;
                    m_runEnter = true;
                    continue;
                }
                return status;
            }
            return EnumTaskStatus.Success;
        }
    }
}