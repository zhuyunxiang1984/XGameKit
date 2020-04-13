using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XGameKit.XBehaviorTree
{
    //组合节点
    [System.Serializable]
    public class XBTNodeComposite : XBTNode
    {
        public List<XBTNode> m_children;
    }
    
}
