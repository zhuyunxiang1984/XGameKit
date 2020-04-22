using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.XBehaviorTree;

namespace XGameKit.XUI
{
    [BTTaskMemo("[XUI]空闲")]
    public class XUIWindowTask_Idle : XBTTask<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }

        public override void OnLeave(XUIWindow obj)
        {
        }
        public override EnumTaskStatus OnUpdate(XUIWindow obj, float elapsedTime)
        {
            obj.mono.Tick(elapsedTime);
            return EnumTaskStatus.Running;
        }
    }


}
