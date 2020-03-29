using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public enum XMonoVariableType
    {
        GameObject = 0,
        Component,
        Bool,
        Int,
        Float,
        String,
        Color,
        Vector2,
        Vector3,
        Vector4,
    }
    
    public static class XMonoVariableUtility
    {
        //检测child是否为root的子节点
        public static bool IsChild(Transform root, Transform child)
        {
            if (root == child)
                return true;
            Transform node = child;
            while (node != child.root)
            {
                var parent = node.parent;
                if (parent == root)
                    return true;
            }
            return false;
        }
        //获取节点的相对路径
        public static string GetChildPath(Transform root, Transform child)
        {
            if (root == child)
                return string.Empty;
            Transform node = child;
            var relativePath = string.Empty;
            while (node != child.root)
            {
                var parent = node.parent;
                if (parent == root)
                {
                    if (string.IsNullOrEmpty(relativePath))
                        return node.name;
                    return $"{node.name}/{relativePath}";
                }
            }
            return string.Empty;
        }
        
        #region 数据转换
        
        //convert bool
        public static string ToString(bool value)
        {
            return string.Empty;
        }
        public static bool ToBool(string text)
        {
            return false;
        }
        //convert int32
        public static string ToString(int value)
        {
            return string.Empty;
        }
        public static int ToInt32(string text)
        {
            return 0;
        }
        //convert float
        public static string ToString(float value)
        {
            return string.Empty;
        }
        public static float ToFloat(string text)
        {
            return 0f;
        }
        //convert color
        public static string ToString(Color value)
        {
            return string.Empty;
        }
        public static Color ToColor(string text)
        {
            return Color.white;
        }
        //convert vector2
        public static string ToString(Vector2 value)
        {
            return string.Empty;
        }
        public static Vector2 ToVector2(string text)
        {
            return Vector2.zero;
        }
        //convert vector3
        public static string ToString(Vector3 value)
        {
            return string.Empty;
        }
        public static Vector3 ToVector3(string text)
        {
            return Vector3.zero;
        }
        //convert vector4
        public static string ToString(Vector4 value)
        {
            return string.Empty;
        }
        public static Vector4 ToVector4(string text)
        {
            return Vector4.zero;
        }
        
        #endregion
    }


}
