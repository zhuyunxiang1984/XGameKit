using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    public static class XBTConst
    {
        public const string Tag = "XBehaviorTree";
    }
    public static class XBTClassFactory
    {
        private static Dictionary<string, Func<object>> m_datas = new Dictionary<string, Func<object>>();

        public static T Alloc<T>(string className) where T : class
        {
            if (!m_datas.ContainsKey(className))
            {
                Debug.LogError($"没有定义 类名:{className}");
                return null;
            }
            return m_datas[className]() as T;
        }
        public static void Free<T>(T obj)where T : class
        {
            XObjectPool.Free(obj);
        }
        public static void Init(Dictionary<string, Func<object>> datas)
        {
            m_datas.Clear();
            foreach (var pairs in datas)
            {
                m_datas.Add(pairs.Key, pairs.Value);
            }
        }
    }

}