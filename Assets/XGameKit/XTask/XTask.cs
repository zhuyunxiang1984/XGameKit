using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{    
    public abstract class XTask
    {
        public virtual int weight => 1;

        public abstract void Enter();
        public abstract void Leave();

        //失败:x<0 执行中:0<x<1 成功:x>=1
        public abstract float Tick(float elapsedTime);
    }
}