using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    //显示状态
    public class XUIWindowStateShow : XState<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            //设置数据
            obj.mono.ShowController(obj.initParam);
            //处理缓存消息
            foreach (var msg in obj.msgCacheList)
            {
                obj.MsgManager.SendMsg(msg);
            }
            //将窗口消息器挂入总消息器
            XMsgManager.Append(obj.uiManager.MsgManager, obj.MsgManager);
            //加入canvas排序
            obj.layer = obj.mono.layerData.GetValue();
            int index = obj.uiManager.GetSort(obj);
            obj.uiManager.AddSort(obj, index);
            obj.canvas = obj.uiManager.uiRoot.uiCanvasManager.AppendClone(index);
            obj.gameObject.transform.SetParent(obj.canvas.transform, false);
            
            obj.stateMachine.ChangeState(XUIWindowStateMachine.stShowAnim);
        }

        public override string Transition(XUIWindow obj)
        {
            if (!obj.isShow)
                return XUIWindowStateMachine.stHide;
            return string.Empty;
        }
    }
}
