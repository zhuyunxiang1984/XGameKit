using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XGameKit.XBehaviorTree
{
    public static class EditorXBehaviorTree
    {
        [MenuItem("CONTEXT/XBTNodeMono/添加树节点")]
        static void AddTreeNode(MenuCommand cmd)
        {
            var context = (XBTNodeMono) cmd.context;
            
            var mono = new GameObject().AddComponent<XBTNodeMono>();
            mono.transform.SetParent(context.transform);
        }

        [MenuItem("XGameKit/行为树/生成编辑菜单")]
        static void GenerateMenu()
        {
            
        }

        private static List<(string, Type)> TreeNodeDefines = new List<(string, Type)>()
        {
            ("顺序节点", typeof(XBTTaskSequence)),
            ("等待", typeof(XBTTaskWait)),
            ("打印log", typeof(XBTTaskLog)),
        };
    }
}
