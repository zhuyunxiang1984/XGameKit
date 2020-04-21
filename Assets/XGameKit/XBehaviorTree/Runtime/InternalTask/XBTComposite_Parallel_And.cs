using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    /// <summary>
    /// 平行节点（与门）
    /// 同时执行子节点
    /// 全部成功，返回成功
    /// 一个失败，返回失败
    /// </summary>
    [BTTaskMemo("[组合]平行节点（与门）")]
    public class XBTComposite_Parallel_And : XBTCommonTask
    {
        protected int m_index;
        protected bool m_start;
        protected List<XBTBehavior> m_hehaviors = new List<XBTBehavior>();

        public override void OnEnter(object obj)
        {
            m_index = 0;
            m_start = false;
            for (int i = 0; i < m_node.children.Count; ++i)
            {
                m_hehaviors.Add(XObjectPool.Alloc<XBTBehavior>());
            }
        }
        public override void OnLeave(object obj)
        {
            foreach (var behavior in m_hehaviors)
            {
                behavior.Stop(obj);
                XObjectPool.Free(behavior);
            }
            m_hehaviors.Clear();
        }
        public override EnumTaskStatus OnUpdate(object obj, float elapsedTime)
        {
            if (m_node.children == null || m_node.children.Count < 1)
                return EnumTaskStatus.Success;
            if (!m_start)
            {
                foreach (var behavior in m_hehaviors)
                {
                    behavior.Start(m_node.children[m_index], obj);
                }
                m_start = true;
            }
            bool complete = true;
            foreach (var behavior in m_hehaviors)
            {
                var status = behavior.Update(obj, elapsedTime);
                if (status == EnumTaskStatus.Failure)
                    return status;
                if (status == EnumTaskStatus.Running)
                {
                    complete = false;
                }
            }
            if (!complete)
            {
                return EnumTaskStatus.Running;
            }
            return EnumTaskStatus.Success;
        }
    }
}