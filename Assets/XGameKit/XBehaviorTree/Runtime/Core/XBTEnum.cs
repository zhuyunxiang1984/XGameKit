using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XBehaviorTree
{
    //任务执行状态
    public enum EnumTaskStatus
    {
        None = 0,
        Execute, //执行中
        Success, //成功
        Failure, //失败
    }
}