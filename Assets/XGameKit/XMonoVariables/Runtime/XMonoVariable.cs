using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using Sirenix.OdinInspector.Editor;

#endif

namespace XGameKit.Core
{
    [System.Serializable]
    public class XMonoVariable
    {
        //变量名称
        public string name;
        //变量类型
        public XMonoVariableType type;
        //引用数据
        public Object objData;
        //值类数据
        public string valData;
        
#if UNITY_EDITOR
        //记录引用对象（GameObject）
        public GameObject gameobject;
        //记录引用脚本类型（Component）
        public System.Type componentType;
        //是否记录了引用路径
        public bool recorded;
        //引用自己节点
        public bool isself;
        //记录相对路径
        public string relativePath;
#endif
    }
    
#if UNITY_EDITOR
    public class XMonoVariableDrawer : OdinValueDrawer<XMonoVariable>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            XMonoVariable target = ValueEntry.SmartValue;

            EditorGUILayout.BeginHorizontal();

            if (target.type == XMonoVariableType.GameObject ||
                target.type == XMonoVariableType.Component)
            {
                EditorGUILayout.Toggle(target.recorded,GUILayout.Width(12));
            }
            target.name = EditorGUILayout.TextField(target.name, GUILayout.Width(100));
            
            EditorGUI.BeginChangeCheck();
            target.type = (XMonoVariableType)EditorGUILayout.EnumPopup(GUIContent.none, target.type, GUILayout.Width(100));
            if (EditorGUI.EndChangeCheck())
            {
                target.recorded = false;
                
            }
            
            switch (target.type)
            {
                case XMonoVariableType.GameObject:
                    DrawValueGameObject(target);
                    break;
                case XMonoVariableType.Component:
                    DrawValueComponent(target);
                    break;
                case XMonoVariableType.Bool:
                    DrawValueBoolean(target);
                    break;
                case XMonoVariableType.Int:
                    DrawValueInteger(target);
                    break;
                case XMonoVariableType.Float:
                    DrawValueFloat(target); 
                    break;
                case XMonoVariableType.Color:
                    DrawValueColor(target);
                    break;
                case XMonoVariableType.String:
                    DrawValueString(target); 
                    break;
                case XMonoVariableType.Vector2:
                    DrawValueVector2(target); 
                    break;
                case XMonoVariableType.Vector3:
                    DrawValueVector3(target);
                    break;
                case XMonoVariableType.Vector4:
                    DrawValueVector4(target);
                    break;
            }
            EditorGUILayout.EndHorizontal();

            
            ValueEntry.SmartValue = target;
        }

        #region 数值编辑绘制

        void DrawValueGameObject(XMonoVariable target)
        {
            EditorGUI.BeginChangeCheck();
            target.gameobject = EditorGUILayout.ObjectField(target.gameobject, typeof(GameObject), true, GUILayout.MinWidth(100)) as GameObject;
            target.objData = target.gameobject;
            if (EditorGUI.EndChangeCheck())
            {
                target.recorded = false;
            }
            //自动命名
            if (target.gameobject != null && string.IsNullOrEmpty(target.name))
            {
                target.name = target.gameobject.name;
            }
        }
        void DrawValueComponent(XMonoVariable target)
        {
            if (target.gameobject != null)
            {
                GameObject go = target.gameobject;
                if (go != null)
                {
                    var components = go.GetComponents<Component>();
                    int index = -1;
                    //通过组件引用查找
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (target.objData == components[i])
                        {
                            index = i;
                            break;
                        }
                    }
                    //通过组件类型查找
                    if (index == -1 && target.componentType != null)
                    {
                        for (int i = 0; i < components.Length; i++)
                        {
                            if (target.componentType == components[i].GetType())
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                    //没有找到的话，默认第一个
                    index = index < 0 ? 0 : index;
                    List<GUIContent> contents = new List<GUIContent>();
                    foreach (var component in components)
                    {
                        var type = component.GetType();
                        contents.Add(new GUIContent(type.Name, type.FullName));
                    }
                    EditorGUI.BeginChangeCheck();
                    var newIndex = EditorGUILayout.Popup(GUIContent.none, index, contents.ToArray(), GUILayout.MinWidth(100));
                    if (EditorGUI.EndChangeCheck())
                    {
                        target.objData = components[newIndex];
                        target.componentType = components[newIndex].GetType();
                    }
                }
            }
            EditorGUI.BeginChangeCheck();
            target.gameobject = EditorGUILayout.ObjectField(target.gameobject, typeof(GameObject), true, GUILayout.MinWidth(100)) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                target.recorded = false;
            }
            //自动命名
            if (target.gameobject != null && string.IsNullOrEmpty(target.name))
            {
                target.name = target.gameobject.name;
            }
        }
        void DrawValueBoolean(XMonoVariable target)
        {
            EditorGUI.BeginChangeCheck();
            var value = XMonoVariableUtility.ToBool(target.valData);
            value = EditorGUILayout.Toggle(value);
            if (EditorGUI.EndChangeCheck())
            {
                target.valData = XMonoVariableUtility.ToString(value);
            }
        }
        void DrawValueInteger(XMonoVariable target)
        {
            EditorGUI.BeginChangeCheck();
            var value = XMonoVariableUtility.ToInt32(target.valData);
            value = EditorGUILayout.IntField(value);
            if (EditorGUI.EndChangeCheck())
            {
                target.valData = XMonoVariableUtility.ToString(value);
            }
        }
        void DrawValueFloat(XMonoVariable target)
        {
            EditorGUI.BeginChangeCheck();
            var value = XMonoVariableUtility.ToFloat(target.valData);
            value = EditorGUILayout.FloatField(value);
            if (EditorGUI.EndChangeCheck())
            {
                target.valData = XMonoVariableUtility.ToString(value);
            }
        }
        void DrawValueString(XMonoVariable target)
        {
            EditorGUI.BeginChangeCheck();
            var value = target.valData;
            value = EditorGUILayout.TextField(value);
            if (EditorGUI.EndChangeCheck())
            {
                target.valData = value;
            }
        }
        void DrawValueColor(XMonoVariable target)
        {
            EditorGUI.BeginChangeCheck();
            var value = XMonoVariableUtility.ToColor(target.valData);
            value = EditorGUILayout.ColorField(value);
            if (EditorGUI.EndChangeCheck())
            {
                target.valData = XMonoVariableUtility.ToString(value);
            }
        }
        void DrawValueVector2(XMonoVariable target)
        {
            EditorGUI.BeginChangeCheck();
            var value = XMonoVariableUtility.ToVector2(target.valData);
            value = EditorGUILayout.Vector2Field(GUIContent.none, value);
            if (EditorGUI.EndChangeCheck())
            {
                target.valData = XMonoVariableUtility.ToString(value);
            }
        }
        void DrawValueVector3(XMonoVariable target)
        {
            EditorGUI.BeginChangeCheck();
            var value = XMonoVariableUtility.ToVector3(target.valData);
            value = EditorGUILayout.Vector3Field(GUIContent.none, value);
            if (EditorGUI.EndChangeCheck())
            {
                target.valData = XMonoVariableUtility.ToString(value);
            }
        }
        void DrawValueVector4(XMonoVariable target)
        {
            EditorGUI.BeginChangeCheck();
            var value = XMonoVariableUtility.ToVector4(target.valData);
            value = EditorGUILayout.Vector4Field(GUIContent.none, value);
            if (EditorGUI.EndChangeCheck())
            {
                target.valData = XMonoVariableUtility.ToString(value);
            }
        }
        #endregion

    }
#endif
}