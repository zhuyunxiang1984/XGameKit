using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public enum EnumXTaskState
    {
        None = 0,
        Execute,   //执行中
        Retry,     //重试
        Success,   //执行成功
        Failure,   //执行失败
        Remove,    //移除
    }
    
    public abstract class XTaskData : IXPoolable
    {
        //状态
        public EnumXTaskState State;
        //当前步骤
        public int CurStep;
        //当前重试次数
        public int RetryTimesCounter;
        //重试间隔计数
        public float RetryIntervalCounter;
        //等待时间
        public float WaitTimeCounter;

        public void Reset()
        {
            State = EnumXTaskState.None;
            CurStep = 0;
            RetryTimesCounter = 0;
            RetryIntervalCounter = 0f;
        }
    }

    public abstract class XTaskData<T> : XTaskData
    {
        public T Data { get; protected set; }

        public void Init(T data)
        {
            Data = data;
        }
    }
}