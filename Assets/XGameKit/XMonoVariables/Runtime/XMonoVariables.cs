using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using XGameKit.XUI;
#if UNITY_EDITOR

using UnityEditor;
using Sirenix.OdinInspector.Editor;

#endif

namespace XGameKit.Core
{
    public class XMonoVariables : MonoBehaviour
    {
        [CustomContextMenu("打印数据", "CheckValue")]
        public List<XMonoVariable> values = new List<XMonoVariable>();

#if UNITY_EDITOR
        [Button("记录引用", ButtonSizes.Large), HorizontalGroup()]
        void Record()
        {
            foreach (var value in values)
            {
                if (value.type != XMonoVariableType.GameObject &&
                    value.type != XMonoVariableType.Component)
                {
                    continue;
                }
                if (gameObject == value.gameobject)
                {
                    value.isself = true;
                    value.recorded = true;
                    Debug.Log($"{value.name} = 自己");
                    continue;
                }
                var relativePath = XMonoVariableUtility.GetChildPath(transform, value.gameobject.transform);
                if (string.IsNullOrEmpty(relativePath))
                {
                    Debug.LogError($"{value.name} 引用了不是自己Child的节点！！！！！！！");
                    value.recorded = false;
                    continue;
                }
                value.relativePath = relativePath;
                value.recorded = true;
                Debug.Log($"{value.name} = {value.relativePath}");
            }
        }
        [Button("恢复引用", ButtonSizes.Large), HorizontalGroup()]
        void Recover()
        {
            foreach (var value in values)
            {
                if (value.type != XMonoVariableType.GameObject &&
                    value.type != XMonoVariableType.Component)
                {
                    continue;
                }
                if (!value.recorded)
                {
                    Debug.LogError($"{value.name} 没有记录路径");
                    continue;
                }
                if (value.isself)
                {
                    value.gameobject = gameObject;
                    continue;
                }
                var child = transform.Find(value.relativePath);
                if (child == null)
                {
                    Debug.LogError($"{value.relativePath} 不存在");
                    continue;
                }
                value.gameobject = child.gameObject;
            }
        }

        void CheckValue()
        {
            var logger = XDebug.CreateMutiLogger("XMonoVariables");
            logger.Append($"===打印数据=== count:{values.Count}");
            foreach (var value in values)
            {
                logger.Append($"{value.name}:{value.GetValue()}");
            }
            logger.Log();
        }

        public bool Exist(string name)
        {
            foreach (var val in values)
            {
                if (val.name == name)
                    return true;
            }
            return false;
        }
#endif
    }
}