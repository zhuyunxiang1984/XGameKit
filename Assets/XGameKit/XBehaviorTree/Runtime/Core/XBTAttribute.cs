using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XBehaviorTree
{
    //节点备忘录
    [AttributeUsage(AttributeTargets.Class)]
    public class BTTaskMemoAttribute : Attribute
    {
        public string memo;

        public BTTaskMemoAttribute(string memo)
        {
            this.memo = memo;
        }
    }

}