﻿using System;
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
            return Convert.ToBase64String(BitConverter.GetBytes(value));
        }
        //convert int32
        public static string ToString(int value)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(value));
        }
        //convert float
        public static string ToString(float value)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(value));
        }
        //convert color
        public static string ToString(Color value)
        {
            byte[] inArray = new byte[16];
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.r), 0, (Array) inArray, 0, 4);
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.g), 0, (Array) inArray, 4, 4);
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.b), 0, (Array) inArray, 8, 4);
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.a), 0, (Array) inArray, 12, 4);
            return Convert.ToBase64String(inArray);
        }
        //convert vector2
        public static string ToString(Vector2 value)
        {
            byte[] inArray = new byte[8];
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.x), 0, (Array) inArray, 0, 4);
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.y), 0, (Array) inArray, 4, 4);
            return Convert.ToBase64String(inArray);
        }
        //convert vector3
        public static string ToString(Vector3 value)
        {
            byte[] inArray = new byte[12];
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.x), 0, (Array) inArray, 0, 4);
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.y), 0, (Array) inArray, 4, 4);
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.z), 0, (Array) inArray, 8, 4);
            return Convert.ToBase64String(inArray);
        }
        //convert vector4
        public static string ToString(Vector4 value)
        {
            byte[] inArray = new byte[16];
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.x), 0, (Array) inArray, 0, 4);
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.y), 0, (Array) inArray, 4, 4);
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.z), 0, (Array) inArray, 8, 4);
            Buffer.BlockCopy((Array) BitConverter.GetBytes(value.w), 0, (Array) inArray, 12, 4);
            return Convert.ToBase64String(inArray);
        }
        
        public static bool ToBool(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return BitConverter.ToBoolean(Convert.FromBase64String(value), 0);
        }
        public static int ToInt32(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            return BitConverter.ToInt32(Convert.FromBase64String(value), 0);
        }
        public static float ToSingle(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0.0f;
            return BitConverter.ToSingle(Convert.FromBase64String(value), 0);
        }
        public static Color ToColor(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Color.white;
            byte[] numArray = Convert.FromBase64String(value);
            Color color = Color.white;
            color.r = BitConverter.ToSingle(numArray, 0);
            color.g = BitConverter.ToSingle(numArray, 4);
            color.b = BitConverter.ToSingle(numArray, 8);
            color.a = BitConverter.ToSingle(numArray, 12);
            return color;
        }
        public static Vector2 ToVector2(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Vector2.zero;
            byte[] numArray = Convert.FromBase64String(value);
            Vector2 vec = Vector2.zero;
            vec.x = BitConverter.ToSingle(numArray, 0);
            vec.y = BitConverter.ToSingle(numArray, 4);
            return vec;
        }
        public static Vector3 ToVector3(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Vector3.zero;
            byte[] numArray = Convert.FromBase64String(value);
            Vector3 vec = Vector3.zero;
            vec.x = BitConverter.ToSingle(numArray, 0);
            vec.y = BitConverter.ToSingle(numArray, 4);
            vec.z = BitConverter.ToSingle(numArray, 8);
            return vec;
        }
        public static Vector4 ToVector4(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Vector4.zero;
            byte[] numArray = Convert.FromBase64String(value);
            Vector4 vec = Vector4.zero;
            vec.x = BitConverter.ToSingle(numArray, 0);
            vec.y = BitConverter.ToSingle(numArray, 4);
            vec.z = BitConverter.ToSingle(numArray, 8);
            vec.w = BitConverter.ToSingle(numArray, 12);
            return vec;
        }

        #endregion
    }


}