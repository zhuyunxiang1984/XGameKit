using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XGameKit.XBehaviorTree
{
    //组合节点
    public class XBTNodeCompositeMono : XBTNodeMono
    {
        public List<XBTNodeMono> children;
    }
}
