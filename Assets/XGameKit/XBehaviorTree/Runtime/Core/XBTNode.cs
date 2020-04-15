using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    [System.Serializable]
    public class XBTNode
    {
        public XBTNode parent;
        public List<XBTNode> children;
        
        public string taskClassName;
        public Dictionary<string, XMonoVariable> variables;
        
    }

}
