using System;
using System.Collections.Generic;

namespace XGameKit.Core
{
    public abstract class XEvent
    {
        public string Name { get; private set; }

        public void SetName(string value)
        {
            Name = value;
        }

        public virtual void Reset()
        {
        }

        public virtual void HandleEvent(Delegate handler)
        {
            (handler as Action<XEvent>)?.Invoke(this);
        }
    }

    //自定义事件类型，参数自动转换为自定义类型
    public abstract class XCustomEvent<T> : XEvent where T : XEvent
    {
        public override void HandleEvent(Delegate handler)
        {
            (handler as Action<T>)?.Invoke(this as T);
        }
    }

    public class XEvent<T> : XEvent
    {
        public T Param { get; set; }

        public override void HandleEvent(Delegate handler)
        {
            (handler as Action<T>)?.Invoke(Param);
        }
    }

    public class XEvent<T1, T2> : XEvent
    {
        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }

        public override void HandleEvent(Delegate handler)
        {
            (handler as Action<T1, T2>)?.Invoke(Param1, Param2);
        }
    }

    public class XEvent<T1, T2, T3> : XEvent
    {
        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }

        public override void HandleEvent(Delegate handler)
        {
            (handler as Action<T1, T2, T3>)?.Invoke(Param1, Param2, Param3);
        }
    }

    public class XEvent<T1, T2, T3, T4> : XEvent
    {
        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }

        public override void HandleEvent(Delegate handler)
        {
            (handler as Action<T1, T2, T3, T4>)?.Invoke(Param1, Param2, Param3, Param4);
        }
    }

    public class XEvent<T1, T2, T3, T4, T5> : XEvent
    {
        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }

        public override void HandleEvent(Delegate handler)
        {
            (handler as Action<T1, T2, T3, T4, T5>)?.Invoke(Param1, Param2, Param3, Param4, Param5);
        }
    }

    public class XEventPool : IDisposable
    {
        private Dictionary<string, Stack<XEvent>> m_dictDatas = new Dictionary<string, Stack<XEvent>>();

        public void Dispose()
        {
            m_dictDatas.Clear();
        }

        public T Get<T>() where T : XEvent, new()
        {
            return Get<T>(typeof(T).Name);
        }

        public void Recycle<T>(T evt) where T : XEvent
        {
            Recycle<T>(evt.GetType().Name, evt);
        }

        public T Get<T>(string name) where T : XEvent, new()
        {
            Stack<XEvent> stack = null;
            if (m_dictDatas.ContainsKey(name))
            {
                stack = m_dictDatas[name];
            }
            else
            {
                stack = new Stack<XEvent>();
                m_dictDatas.Add(name, stack);
            }
            if (stack.Count > 0)
                return stack.Pop() as T;
            return new T();
        }

        public void Recycle<T>(string name, T evt) where T : XEvent
        {
            Stack<XEvent> stack = null;
            if (m_dictDatas.ContainsKey(name))
            {
                stack = m_dictDatas[name];
            }
            else
            {
                stack = new Stack<XEvent>();
                m_dictDatas.Add(name, stack);
            }

            stack.Push(evt);
        }
    }
}