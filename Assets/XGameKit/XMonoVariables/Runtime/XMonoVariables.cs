using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using Sirenix.OdinInspector.Editor;

#endif

namespace XGameKit.Core
{
    public class XMonoVariables : MonoBehaviour
    {
        public List<XMonoVariable> values;

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
                if (string.IsNullOrEmpty(value.relativePath))
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
#endif

        private bool _initialized;
        private Dictionary<string, XMonoVariable> _dictValues = new Dictionary<string, XMonoVariable>();
        private void Awake()
        {
            _InitializeInternal();
        }

        private void _InitializeInternal()
        {
            if (_initialized)
                return;
            _dictValues.Clear();
            foreach (var value in values)
            {
                if (string.IsNullOrEmpty(value.name))
                    continue;
                if (_dictValues.ContainsKey(value.name))
                {
                    Debug.LogError($"{value.name} 重复定义");
                    continue;
                }
                _dictValues.Add(value.name, value);
            }
            _initialized = true;
        }

        //将变量注入制定对象
        public void Inject<T>(T target)
        {
            
        }
    }
}