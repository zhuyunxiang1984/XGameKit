using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XBehaviorTree;

namespace XGameKit.XUI
{
    [BTTaskMemo("[XUI]检测Window状态改变")]
    public class XUIWindowTask_StateChange : XBTTask<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }

        public override void OnLeave(XUIWindow obj)
        {
            
        }

        public override EnumTaskStatus OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (obj.CurState == obj.DstState || obj.isShowAnimating || obj.isHideAnimating)
                return EnumTaskStatus.Running;
            XDebug.Log(XUIConst.Tag, $"XUIWindowTask_StateChange {obj.name} {obj.CurState} -> {obj.DstState}");
            obj.CurState = obj.DstState;
            return EnumTaskStatus.Success;
        }
    }


}
