using System;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
namespace XGameKit.XUI
{
    //空闲状态
    public class XUIWindowStateIdle: XState<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            obj.mono.Tick(elapsedTime);
        }
        public override string Transition(XUIWindow obj)
        {
            if (!obj.isShow)
                return XUIWindowStateMachine.stHideAnim;
            return String.Empty;
        }
    }
    

}

