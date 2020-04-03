using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
namespace XGameKit.XUI
{
    
//卸载UI资源
    public class XUIWindowStateUnload : XState<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            obj.mono.Term();
            obj.mono = null;
            GameObject.Destroy(obj.gameObject);
            obj.gameObject = null;
            obj.stateMachine.ChangeState(XUIWindowStateMachine.stDestroy);
        }
        public override string Transition(XUIWindow obj)
        {
            if (obj.isShow)
                return XUIWindowStateMachine.stLoad;
            return String.Empty;
        }
    }
}

