using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    public static class XBTUtilities
    {
        //将XBTNodeMono转换成XBTNode
        public static XBTNode ParseMono(XBTNodeMono mono, XBTNode parent = null)
        {
            var node = new XBTNode();
            node.parent = parent;
            node.taskClassName = mono.className;
            var variables = mono.GetComponent<XMonoVariables>();
            if (variables != null)
            {
                node.variables = XMonoVariableUtility.ConvertToDict(variables.values);
            }
            var trans = mono.transform;
            int count = trans.childCount;
            if (count > 0)
            {
                node.children = new List<XBTNode>();
                for (int i = 0; i < count; ++i)
                {
                    var child = trans.GetChild(i).GetComponent<XBTNodeMono>();
                    if (child == null)
                        continue;
                    node.children.Add(ParseMono(child, node));
                }
            }
            return node;
        }
    }
}