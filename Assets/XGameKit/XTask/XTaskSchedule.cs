using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XGameKit.Core
{
    public class XTaskSchedule<T> where T : class
    {
        protected List<XTask<T>> m_tasks = new List<XTask<T>>();
        protected Action<bool> m_OnComplete;
        protected T m_obj;
        protected int m_step;

        public void AddMethod(Action<T> method)
        {
            if (method == null)
                return;
            m_tasks.Add(new XTaskCallMethod<T>(method));
        }

        public void AddWait(float seconds)
        {
            if (seconds <= 0f)
                return;
            m_tasks.Add(new XTaskWaitSeconds<T>(seconds));
        }
        public void AddTask(XTask<T> task)
        {
            if (task == null)
                return;
            m_tasks.Add(task);
        }
        public void Start(T obj, Action<bool> OnComplete = null)
        {
            m_obj = obj;
            m_OnComplete = OnComplete;
            m_step = 0;
            m_tasks[m_step].Enter(m_obj);
        }
        public void Stop()
        {
            if (m_obj == null)
                return;
            m_tasks[m_step].Leave(m_obj);
            m_obj = null;
            m_OnComplete?.Invoke(false);
            m_OnComplete = null;
        }
        public void Complete(bool success)
        {
            if (m_obj == null)
                return;
            m_tasks[m_step].Leave(m_obj);
            m_obj = null;
            m_OnComplete?.Invoke(success);
            m_OnComplete = null;
        }
        public void Update(float elapsedTime)
        {
            if (m_obj == null)
                return;
            var result = m_tasks[m_step].Execute(m_obj, elapsedTime);
            if (result == EnumXTaskResult.Execute)
                return;
            if (result == EnumXTaskResult.Failure)
            {
                Complete(false);
                return;
            }
            if (result == EnumXTaskResult.Success && m_step + 1 >= m_tasks.Count)
            {
                Complete(true);
                return;
            }
            m_tasks[m_step].Leave(m_obj);
            m_step += 1;
            m_tasks[m_step].Enter(m_obj);
        }
    }
}