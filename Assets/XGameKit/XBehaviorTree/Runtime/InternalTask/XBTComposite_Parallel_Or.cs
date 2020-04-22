using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    /// <summary>
    /// 平行节点（或门）
    /// 同时执行子节点
    /// 一个成功，返回成功
    /// 全部失败，返回失败
    /// </summary>
    [BTTaskMemo("[组合]平行节点（或门）")]
    public class XBTComposite_Parallel_Or : XBTCommonTask
    {
        protected List<XBTBehavior> m_hehaviors = new List<XBTBehavior>();
        protected bool m_runEnter;

        public override void OnEnter(object obj)
        {
            for (int i = 0; i < m_node.children.Count; ++i)
            {
                m_hehaviors.Add(XObjectPool.Alloc<XBTBehavior>());
            }
            m_runEnter = true;
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
            if (m_runEnter)
            {
                for (int i = 0; i < m_node.children.Count; ++i)
                {
                    XDebug.Log(XBTConst.Tag,$"平行节点（或门） {i} {m_node.children[i].taskClassName}");
                    m_hehaviors[i].Start(m_node.children[i], obj);
                }
                m_runEnter = false;
            }
            bool complete = true;
            foreach (var behavior in m_hehaviors)
            {
                var status = behavior.Update(obj, elapsedTime);
                if (status == EnumTaskStatus.Success)
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
            return EnumTaskStatus.Failure;
        }
    }
}