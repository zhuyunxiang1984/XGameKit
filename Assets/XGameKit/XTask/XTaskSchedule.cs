using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XGameKit.Core
{
    public abstract class XTaskSchedule<DATA>
    {
        public Action<DATA, bool> OnComplete { get; set; }
        
        protected List<XTask<DATA>> m_tasks = new List<XTask<DATA>>();
        
        public void AddTask(XTask<DATA> task)
        {
            if (task == null)
                return;
            m_tasks.Add(task);
        }
        public void DelTask(XTask<DATA> task)
        {
            if (task == null)
                return;
            m_tasks.Remove(task);
        }
        protected List<XTaskData<DATA>> m_listTaskDatas = new List<XTaskData<DATA>>();
        protected Dictionary<DATA, XTaskData<DATA>> m_dictTaskDatas = new Dictionary<DATA, XTaskData<DATA>>();

        protected abstract XTaskData<DATA> _CreateTaskData(DATA data);

        public void Start(DATA data)
        {
            if (m_dictTaskDatas.ContainsKey(data))
                return;
            var taskData = _CreateTaskData(data);
            taskData.Init(data);
            m_listTaskDatas.Add(taskData);
            m_dictTaskDatas.Add(data, taskData);
        }
        public void Stop(DATA data)
        {
            if (!m_dictTaskDatas.ContainsKey(data))
                return;
            var taskData = m_dictTaskDatas[data];
            if (taskData.CurStep >= 0 && taskData.CurStep < m_tasks.Count)
            {
                m_tasks[taskData.CurStep].Stop(taskData);
            }
            m_listTaskDatas.Remove(taskData);
            m_dictTaskDatas.Remove(data);
        }

        public void Update(float elapsedTime)
        {
            int max = m_listTaskDatas.Count;
            if (max < 1)
                return;
            for (int i = 0; i < max; ++i)
            {
                var taskData = m_listTaskDatas[i];
                if (taskData.CurStep >= 0 || taskData.CurStep < m_tasks.Count)
                {
                    m_tasks[taskData.CurStep].Execute(taskData, elapsedTime);
                }
                else
                {
                    taskData.State = EnumXTaskState.Failure;
                }
                switch (taskData.State)
                {
                    case EnumXTaskState.Success:
                        taskData.CurStep += 1;
                        if (taskData.CurStep >= m_tasks.Count)
                        {
                            OnComplete?.Invoke(taskData.Data, true);
                            XObjectPool.Free(taskData);
                            m_listTaskDatas.RemoveAt(i);
                            --i;
                        }
                        else
                        {
                            taskData.State = EnumXTaskState.None;
                        }
                        break;
                    case EnumXTaskState.Failure:
                    case EnumXTaskState.Remove:
                        OnComplete?.Invoke(taskData.Data, false);
                        XObjectPool.Free(taskData);
                        m_listTaskDatas.RemoveAt(i);
                        --i;
                        break;
                }
            }
        }
    }
}