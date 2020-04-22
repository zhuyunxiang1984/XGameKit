using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.XBehaviorTree;

namespace XGameKit.XUI
{
    public abstract class XUIWindowTask_Judgement : XBTTask<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }
        public override void OnLeave(XUIWindow obj)
        {
        }

        public override EnumTaskStatus OnUpdate(XUIWindow obj, float elapsedTime)
        {
            return _CheckMethod(obj) ? EnumTaskStatus.Success : EnumTaskStatus.Failure;
        }
        protected abstract bool _CheckMethod(XUIWindow obj);
    }
    
    [BTTaskMemo("[XUI]是否在显示状态")]
    public class XUIWindowTask_IsShow : XUIWindowTask_Judgement
    {
        protected override bool _CheckMethod(XUIWindow obj)
        {
            return obj.CurState == XUIWindow.EnumState.Show;
        }
    }
    [BTTaskMemo("[XUI]是否在隐藏状态")]
    public class XUIWindowTask_IsHide : XUIWindowTask_Judgement
    {
        protected override bool _CheckMethod(XUIWindow obj)
        {
            return obj.CurState == XUIWindow.EnumState.Hide;
        }
    }
    [BTTaskMemo("[XUI]是否在销毁状态")]
    public class XUIWindowTask_IsRemove : XUIWindowTask_Judgement
    {
        protected override bool _CheckMethod(XUIWindow obj)
        {
            return obj.CurState == XUIWindow.EnumState.Remove;
        }
    }
}

