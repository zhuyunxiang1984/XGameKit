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
        //编辑器模式下的数据保存
        public bool    cacheBool;
        public float   cacheFloat;
        public int     cacheInt;
        public string  cacheString;
        public Color   cacheColor = Color.white;
        public Vector2 cacheVec2;
        public Vector3 cacheVec3;
        public Vector4 cacheVec4;
#endif

        public object GetValue()
        {
            switch (type)
            {
                case XMonoVariableType.Bool:
                    return XMonoVariableUtility.ToBool(valData);
                case XMonoVariableType.Float:
                    return XMonoVariableUtility.ToSingle(valData);
                case XMonoVariableType.Int:
                    return XMonoVariableUtility.ToInt32(valData);
                case XMonoVariableType.String:
                    return valData;
                case XMonoVariableType.Color:
                    return XMonoVariableUtility.ToColor(valData);
                case XMonoVariableType.Vector2:
                    return XMonoVariableUtility.ToVector2(valData);
                case XMonoVariableType.Vector3:
                    return XMonoVariableUtility.ToVector3(valData);
                case XMonoVariableType.Vector4:
                    return XMonoVariableUtility.ToVector4(valData);
                case XMonoVariableType.GameObject:
                    return objData;
                case XMonoVariableType.Component:
                    return objData;
                default:
                    throw new System.NotSupportedException();
            }
        }
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
                _SyncValData(target);
            }
            EditorGUI.BeginChangeCheck();
            switch (target.type)
            {
                case XMonoVariableType.GameObject:
                    DrawValueGameObject(target);
                    break;
                case XMonoVariableType.Component:
                    DrawValueComponent(target);
                    break;
                case XMonoVariableType.Bool:
                    target.cacheBool = EditorGUILayout.Toggle(target.cacheBool);
                    break;
                case XMonoVariableType.Int:
                    target.cacheInt = EditorGUILayout.IntField(target.cacheInt);
                    break;
                case XMonoVariableType.Float:
                    target.cacheFloat = EditorGUILayout.FloatField(target.cacheFloat);
                    break;
                case XMonoVariableType.String:
                    target.cacheString = EditorGUILayout.TextField(target.cacheString);
                    break;
                case XMonoVariableType.Color:
                    target.cacheColor = EditorGUILayout.ColorField(target.cacheColor);
                    break;
                case XMonoVariableType.Vector2:
                    target.cacheVec2 = EditorGUILayout.Vector2Field(GUIContent.none, target.cacheVec2);
                    break;
                case XMonoVariableType.Vector3:
                    target.cacheVec3 = EditorGUILayout.Vector3Field(GUIContent.none, target.cacheVec3);
                    break;
                case XMonoVariableType.Vector4:
                    target.cacheVec4 = EditorGUILayout.Vector4Field(GUIContent.none, target.cacheVec4);
                    break;
            }
            if (EditorGUI.EndChangeCheck())
            {
                _SyncValData(target);
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
        
        #endregion

        private void _SyncValData(XMonoVariable target)
        {
            switch (target.type)
            {
                case XMonoVariableType.Bool:
                    target.valData = XMonoVariableUtility.ToString(target.cacheBool);
                    break;
                case XMonoVariableType.Int:
                    target.valData = XMonoVariableUtility.ToString(target.cacheInt);
                    break;
                case XMonoVariableType.Float:
                    target.valData = XMonoVariableUtility.ToString(target.cacheFloat);
                    break;
                case XMonoVariableType.Color:
                    target.valData = XMonoVariableUtility.ToString(target.cacheColor);
                    break;
                case XMonoVariableType.String:
                    target.valData = target.cacheString;
                    break;
                case XMonoVariableType.Vector2:
                    target.valData = XMonoVariableUtility.ToString(target.cacheVec2);
                    break;
                case XMonoVariableType.Vector3:
                    target.valData = XMonoVariableUtility.ToString(target.cacheVec3);
                    break;
                case XMonoVariableType.Vector4:
                    target.valData = XMonoVariableUtility.ToString(target.cacheVec4);
                    break;
            }
        }
    }
#endif
}