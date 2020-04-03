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
        public void Inject<T>(T obj) where T : class
        {
            Type type = obj.GetType();
            //Debug.Log(type.FullName);
            var fileds = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fieldInfo in fileds)
            {
                //Debug.Log(fieldInfo.Name);
                bool success = false;
                foreach (var value in values)
                {
                    if (fieldInfo.Name != value.name)
                        continue;
                    if (_Inject(obj, fieldInfo, value))
                    {
                        success = true;
                        break;
                    }
                }
                if (!success)
                {
                    Debug.LogErrorFormat("注入失败！ 字段:{0} 类型:{1}", fieldInfo.Name, fieldInfo.FieldType);
                }
            }

        }

        protected bool _Inject<T>(T obj, FieldInfo fieldInfo, XMonoVariable value)
        {
            switch (value.type)
            {
                case XMonoVariableType.GameObject:
                case XMonoVariableType.Component:
                    var objType = value.objData.GetType();
                    if (fieldInfo.FieldType == objType || objType.IsSubclassOf(fieldInfo.FieldType))
                    {
                        fieldInfo.SetValue(obj, value.objData);
                        return true;
                    }

                    break;
                case XMonoVariableType.Bool:
                    if (fieldInfo.FieldType == typeof(bool))
                    {
                        fieldInfo.SetValue(obj, XMonoVariableUtility.ToBool(value.valData));
                        return true;
                    }

                    break;
                case XMonoVariableType.Float:
                    if (fieldInfo.FieldType == typeof(float))
                    {
                        fieldInfo.SetValue(obj, XMonoVariableUtility.ToSingle(value.valData));
                        return true;
                    }

                    break;
                case XMonoVariableType.Int:
                    if (fieldInfo.FieldType == typeof(int))
                    {
                        fieldInfo.SetValue(obj, XMonoVariableUtility.ToInt32(value.valData));
                        return true;
                    }

                    break;
                case XMonoVariableType.String:
                    if (fieldInfo.FieldType == typeof(string))
                    {
                        fieldInfo.SetValue(obj, value.valData);
                        return true;
                    }

                    break;
                case XMonoVariableType.Color:
                    if (fieldInfo.FieldType == typeof(Color))
                    {
                        fieldInfo.SetValue(obj, XMonoVariableUtility.ToColor(value.valData));
                        return true;
                    }

                    break;
                case XMonoVariableType.Vector2:
                    if (fieldInfo.FieldType == typeof(Vector2))
                    {
                        fieldInfo.SetValue(obj, XMonoVariableUtility.ToVector2(value.valData));
                        return true;
                    }

                    break;
                case XMonoVariableType.Vector3:
                    if (fieldInfo.FieldType == typeof(Vector3))
                    {
                        fieldInfo.SetValue(obj, XMonoVariableUtility.ToVector3(value.valData));
                        return true;
                    }

                    break;
                case XMonoVariableType.Vector4:
                    if (fieldInfo.FieldType == typeof(Vector4))
                    {
                        fieldInfo.SetValue(obj, XMonoVariableUtility.ToVector4(value.valData));
                        return true;
                    }

                    break;
            }

            return false;
        }
    }
}