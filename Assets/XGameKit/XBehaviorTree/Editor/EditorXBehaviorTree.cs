using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    public static class EditorXBehaviorTree
    {
        private const string TreeNodeMenu = "GameObject/行为树/";

        #region 新增节点菜单

        private const string AddNodeMenu = TreeNodeMenu + "新增节点/";

        #endregion
        [MenuItem(AddNodeMenu + "子节点", priority = 49)]
        static void AddNodeMenu_Child()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                var mono = new GameObject().AddComponent<XBTNodeMono>();
                mono.transform.SetParent(gameObject.transform);
            }
        }
        [MenuItem(AddNodeMenu + "兄节点", priority = 49)]
        static void AddNodeMenu_Sibling()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                var mono = new GameObject().AddComponent<XBTNodeMono>();
                mono.transform.SetParent(gameObject.transform.parent);
            }
        }
        [MenuItem(AddNodeMenu + "父节点", priority = 49)]
        static void AddNodeMenu_Parent()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                var mono = new GameObject().AddComponent<XBTNodeMono>();
                mono.transform.SetParent(gameObject.transform.parent);
                gameObject.transform.SetParent(mono.transform);
            }
        }
        
        #region 设置节点菜单
        
        private const string CommonNodeMenu = TreeNodeMenu + "设置节点/";
        [MenuItem(CommonNodeMenu + "顺序节点", priority = 49)]
        static void CommonNodeMenu_XBTTaskSequence()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                var mono = gameObject.GetComponent<XBTNodeMono>();
                mono.className = typeof(XBTTaskSequence).Name;
                EditorUtility.SetDirty(gameObject);
            }
        }
        
        [MenuItem(CommonNodeMenu + "等待", priority = 49)]
        static void CommonNodeMenu_XBTTaskWait()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                var mono = gameObject.GetComponent<XBTNodeMono>();
                mono.className = typeof(XBTTaskWait).Name;
                var vals = gameObject.GetComponent<XMonoVariables>();
                if (vals == null)
                    vals = gameObject.AddComponent<XMonoVariables>();
                if (!vals.Exist("time"))
                {
                    vals.values.Clear();
                    vals.values.Add(new XMonoVariable(){name = "time", type = XMonoVariableType.Float});
                }
                EditorUtility.SetDirty(gameObject);
            }
        }
        
        [MenuItem(CommonNodeMenu + "打印LOG", priority = 49)]
        static void CommonNodeMenu_XBTTaskLog()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                var mono = gameObject.GetComponent<XBTNodeMono>();
                mono.className = typeof(XBTTaskLog).Name;
                var vals = gameObject.GetComponent<XMonoVariables>();
                if (vals == null)
                    vals = gameObject.AddComponent<XMonoVariables>();
                if (!vals.Exist("message"))
                {
                    vals.values.Clear();
                    vals.values.Add(new XMonoVariable(){name = "message", type = XMonoVariableType.String});
                }
                EditorUtility.SetDirty(gameObject);
            }
        }
        
        #endregion

    }
}
