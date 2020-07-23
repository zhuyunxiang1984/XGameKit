using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XGameKit.Core
{
    public class XTaskSchedule
    {
        public enum Mode
        {
            All = 0,
            StopByFailure,
        }
        protected List<XTask> m_tasks = new List<XTask>();
        protected Action<bool, int> m_OnComplete;
        protected Mode m_mode;
        protected int m_step;
        protected int m_failure;

        protected Action<float> m_OnProgress;
        protected float m_curWeight;
        protected float m_maxWeight;

        public XTaskSchedule()
        {
            m_step = -1;
        }

        public void AddMethod(Action method)
        {
            if (method == null)
                return;
            m_tasks.Add(new XTaskCallMethod(method));
        }

        public void AddWait(float seconds)
        {
            if (seconds <= 0f)
                return;
            m_tasks.Add(new XTaskWaitSeconds(seconds));
        }
        public void AddTask(XTask task)
        {
            if (task == null)
                return;
            m_tasks.Add(task);
        }
        public void Start(Action<bool, int> OnComplete = null, Action<float> OnProgress = null, Mode mode = Mode.All)
        {
            m_mode = mode;
            m_step = 0;
            m_failure = 0;
            m_OnComplete = OnComplete;

            m_curWeight = 0;
            m_maxWeight = 0;
            foreach (var task in m_tasks)
            {
                m_maxWeight += task.weight;
            }
            m_OnProgress = OnProgress;
            m_OnProgress?.Invoke(0f);

            m_tasks[m_step].Enter();
        }
        public void Stop()
        {
            m_tasks[m_step].Leave();
            m_step = -1;
            m_OnComplete?.Invoke(false, m_failure);
        }
        public void Tick(float elapsedTime)
        {
            if (m_step == -1)
                return;
            var result = m_tasks[m_step].Tick(elapsedTime);
            if (result >= 0f && result < 1f)
            {
                float progress = (m_curWeight + m_tasks[m_step].weight * result) / m_maxWeight;
                m_OnProgress?.Invoke(progress);
                return;
            }
            if (result < 0f)
            {
                ++m_failure;
                if (m_mode == Mode.StopByFailure)
                {
                    m_step = -1;
                    m_OnComplete?.Invoke(false, m_failure);
                    return;
                }
            }
            if (result >= 1f)
            {
                if (m_step + 1 >= m_tasks.Count)
                {
                    m_step = -1;
                    m_OnProgress?.Invoke(1f);
                    m_OnComplete?.Invoke(true, m_failure);
                    return;
                }
            }
            m_curWeight += m_tasks[m_step].weight;
            m_OnProgress?.Invoke(m_curWeight / m_maxWeight);

            m_tasks[m_step].Leave();
            m_step += 1;
            m_tasks[m_step].Enter();
        }
    }
}