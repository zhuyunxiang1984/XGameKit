using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;


namespace XGameKit.XUI
{
    //销毁状态
    public class XUIWindowStateDestroy : XState<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
        }

        public override string Transition(XUIWindow obj)
        {
            if (obj.isShow)
                return XUIWindowStateMachine.stLoad;
            return String.Empty;
        }
    }
}

