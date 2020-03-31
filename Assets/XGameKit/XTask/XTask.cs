using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public enum EnumXTaskResult
    {
        Execute = 0,   //执行中
        Success,   //执行成功
        Failure,   //执行失败
    }
    
    public abstract class XTask<DATA>
    {
        //重试次数
        protected virtual int retry
        {
            get { return 0; }
        }
        //重试间隔
        protected virtual float retryInterval
        {
            get { return 0f; }
        }

        public void Stop(XTaskData<DATA> taskData)
        {
            if (taskData.State == EnumXTaskState.Execute)
                OnLeave(taskData);
            taskData.State = EnumXTaskState.Remove;
        }
        public void Execute(XTaskData<DATA> taskData, float elapsedTime)
        {
            switch (taskData.State)
            {
                case EnumXTaskState.None:
                    //开始执行
                    OnEnter(taskData);
                    taskData.State = EnumXTaskState.Execute;
                    break;
                case EnumXTaskState.Execute:
                    var result = OnExecute(taskData, elapsedTime);
                    if (result != EnumXTaskResult.Execute)
                    {
                        //执行结束
                        OnLeave(taskData);
                        if (result == EnumXTaskResult.Failure && taskData.RetryTimesCounter < retry)
                        {
                            XDebug.Log(XTaskConst.Tag, $"{retryInterval}秒后重试... ");
                            taskData.State = EnumXTaskState.Retry;
                            taskData.RetryIntervalCounter = 0f;
                        }
                        else
                        {
                            taskData.State = result == EnumXTaskResult.Success ? EnumXTaskState.Success : EnumXTaskState.Failure;
                        }
                    }
                    break;
                case EnumXTaskState.Retry:
                    taskData.RetryIntervalCounter += elapsedTime;
                    if (taskData.RetryIntervalCounter >= retryInterval)
                    {
                        taskData.RetryIntervalCounter = 0f;
                        taskData.State = EnumXTaskState.None;
                        ++taskData.RetryTimesCounter;
                        XDebug.Log(XTaskConst.Tag, $"重试 {taskData.RetryTimesCounter}/{retry}");
                    }
                    break;
            }
        }
        
        public abstract bool IsDone(XTaskData<DATA> data);
        protected abstract void OnEnter(XTaskData<DATA> data);
        protected abstract void OnLeave(XTaskData<DATA> data);
        protected abstract EnumXTaskResult OnExecute(XTaskData<DATA> data, float elapsedTime);
    }
}