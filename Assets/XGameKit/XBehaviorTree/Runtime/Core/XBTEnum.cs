using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XBehaviorTree
{
    //任务执行状态
    public enum EnumTaskStatus
    {
        None = 0,
        Running, //执行
        Success, //成功
        Failure, //失败
    }
}