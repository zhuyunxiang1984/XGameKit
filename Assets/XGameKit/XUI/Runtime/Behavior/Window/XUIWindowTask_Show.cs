using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XBehaviorTree;

namespace XGameKit.XUI
{
    [BTTaskMemo("[XUI]显示UI")]
    public class XUIWindowTask_Show : XBTTask<XUIWindow>
    {
        protected bool m_complete;
        public override void OnEnter(XUIWindow obj)
        {
            XDebug.Log(XUIConst.Tag, $"XUIWindowTask_Show enter {obj.name}");
            //设置数据
            obj.mono.ShowController(obj.initParam);
            //处理缓存消息
            foreach (var msg in obj.msgCacheList)
            {
                obj.MsgManager.SendMsg(msg);
            }
            obj.msgCacheList.Clear();

            if (obj.canvas == null)
            {
                //将窗口消息器挂入总消息器
                XMsgManager.Append(obj.uiManager.MsgManager, obj.MsgManager);
                //加入canvas排序
                obj.layer = obj.mono.layerData.GetValue();
                int index = obj.uiManager.GetSort(obj);
                obj.uiManager.AddSort(obj, index);
                obj.canvas = obj.uiManager.uiRoot.uiCanvasManager.AppendClone(index);
                obj.gameObject.transform.SetParent(obj.canvas.transform, false);
                obj.gameObject.SetActive(true);
            }
            if (obj.mono.showAnim != null)
            {
                obj.mono.showAnim.Play(delegate { m_complete = true; });
                m_complete = false;
            }
            else
            {
                m_complete = true;
            }
            obj.isShowAnimating = true;
        }

        public override void OnLeave(XUIWindow obj)
        {
            XDebug.Log(XUIConst.Tag, $"XUIWindowTask_Show leave {obj.name}");
        }

        public override EnumTaskStatus OnUpdate(XUIWindow obj, float elapsedTime)
        {
            obj.mono.Tick(elapsedTime);
            if (!m_complete)
            {
                return EnumTaskStatus.Running;
            }
            obj.isShowAnimating = false;
            return EnumTaskStatus.Success;
        }
    }

}
