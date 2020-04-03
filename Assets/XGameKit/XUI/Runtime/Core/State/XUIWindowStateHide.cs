﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    //隐藏状态
    public class XUIWindowStateHide : XState<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }
        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            obj.mono.HideController();
            obj.uiManager.uiRoot.uiCanvasManager.RemoveCanvas(obj.canvas);
            obj.canvas = null;
            obj.gameObject.SetActive(false);
            obj.gameObject.transform.SetParent(obj.uiManager.uiRoot.uiUnusedNode, false);
            
            obj.stateMachine.ChangeState(XUIWindowStateMachine.stCache);
        }

        public override string Transition(XUIWindow obj)
        {
            if (obj.isShow)
                return XUIWindowStateMachine.stShow;
            return string.Empty;
        }
    }
}

