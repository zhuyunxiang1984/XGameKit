using System;
using System.Collections.Generic;

/// <summary>
/// 泛型对象池
/// </summary>

namespace XGameKit.Core
{
    public interface IXPoolable
    {
        void Reset();
    }
    public static class XObjectPool
    {
        static Dictionary<string, Stack<Object>> _caches = new Dictionary<string, Stack<object>>();
    
        public static T Alloc<T>() where T : class, new()
        {
            var className = typeof(T).FullName;
            if (string.IsNullOrEmpty(className))
            {
                return new T();
            }
            Stack<Object> stack = null;
            if (_caches.ContainsKey(className))
            {
                stack = _caches[className];
            }
            if (stack == null || stack.Count < 1)
            {
                return new T();
            }
            return stack.Pop() as T;
        }

        public static void Free<T>(T obj) where T : class
        {
            var className = obj.GetType().FullName;
            if (string.IsNullOrEmpty(className))
                return;
            Stack<Object> stack = null;
            if (_caches.ContainsKey(className))
            {
                stack = _caches[className];
            }
            else
            {
                stack = new Stack<Object>();
                _caches.Add(className, stack);
            }
            stack.Push(obj);
        }

        public static string DumpLog()
        {
            var text = string.Empty;
            text += "===XObjectPool对象池统计===\n";
            foreach (var pairs in _caches)
            {
                text += $"{pairs.Key}:{pairs.Value.Count}\n";
            }
            text += "===XObjectPool对象池统计完毕===";
            return text;
        }
    }
}
