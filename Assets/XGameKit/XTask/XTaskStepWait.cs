using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public class XTaskStepWait<DATA> : XTask<DATA>
    {
        protected float m_wait;
        public XTaskStepWait(float time)
        {
            m_wait = time;
        }
        protected override int retry => 0;
        protected override float retryInterval => 0f;

        public override bool IsDone(XTaskData<DATA> data)
        {
            return false;
        }

        protected override void OnEnter(XTaskData<DATA> data)
        {
            XDebug.Log(XTaskConst.Tag, $"start wait");
            data.WaitTimeCounter = 0f;
        }

        protected override void OnLeave(XTaskData<DATA> data)
        {
            XDebug.Log(XTaskConst.Tag, $"finish wait {data.WaitTimeCounter.ToString("f2")}");
        }

        protected override EnumXTaskResult OnExecute(XTaskData<DATA> data, float elapsedTime)
        {
            data.WaitTimeCounter += elapsedTime;
            if (data.WaitTimeCounter >= m_wait)
            {
                data.WaitTimeCounter = m_wait;
                return EnumXTaskResult.Success;
            }
                
            return EnumXTaskResult.Execute;
        }
    }

}
