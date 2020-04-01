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
    
    public abstract class XTask<T>
    {
        public abstract void Enter(T obj);
        public abstract void Leave(T obj);
        public abstract EnumXTaskResult Execute(T obj, float elapsedTime);
    }
}