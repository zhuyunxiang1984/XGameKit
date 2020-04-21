using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        
#if UNITY_EDITOR
        public static List<(string, string, Type)> listAllTaskClass;
        public static Dictionary<string, (string, string, Type)> dictAllTaskClass;

        public static List<(string, string, Type)> GetAllTaskClassListEditor()
        {
            if (listAllTaskClass == null)
                listAllTaskClass = GetAllTaskClass();
            return listAllTaskClass;
        }
        public static Dictionary<string, (string, string, Type)> GetAllTaskClassDictEditor()
        {
            if (dictAllTaskClass == null)
            {
                dictAllTaskClass = new Dictionary<string, (string, string, Type)>();
                var listAllTaskClass = GetAllTaskClassListEditor();
                dictAllTaskClass.Clear();
                foreach (var data in listAllTaskClass)
                {
                    dictAllTaskClass.Add(data.Item1, data);
                }
            }
            return dictAllTaskClass;
        }
#endif
        
        //获取程序集中所有的task实现类
        public static List<(string, string, Type)> GetAllTaskClass()
        {
            var result = new List<(string, string, Type)>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetInterface("IXBTTask") == null)
                        continue;
                    if (!type.IsClass || type.IsAbstract)
                        continue;
                    //Debug.Log(type.Name);
                    var memo = string.Empty;
                    var attribute = type.GetCustomAttribute<BTTaskMemoAttribute>();
                    if (attribute != null)
                    {
                        memo = attribute.memo;
                    }
                    result.Add((type.Name, memo, type));
                }
            }
            return result;
        }
    }
}