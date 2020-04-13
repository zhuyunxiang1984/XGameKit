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
        public string taskClassName;
        public XMonoVariables variables;
    }

}
