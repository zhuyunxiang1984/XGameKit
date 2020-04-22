using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.XBehaviorTree;

namespace XGameKit.XUI
{
    [BTTaskMemo("[XUI]等待销毁")]
    public class XUIWindowTask_Remove : XBTTask<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override EnumTaskStatus OnUpdate(XUIWindow obj, float elapsedTime)
        {
            obj.DstState = XUIWindow.EnumState.Remove;
            return EnumTaskStatus.Running;
        }
    }
}
