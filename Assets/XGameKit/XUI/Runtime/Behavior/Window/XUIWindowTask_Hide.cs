using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XBehaviorTree;

namespace XGameKit.XUI
{
    [BTTaskMemo("[XUI]隐藏UI")]
    public class XUIWindowTask_Hide : XBTTask<XUIWindow>
    {
        protected bool m_complete;
        public override void OnEnter(XUIWindow obj)
        {
            XDebug.Log(XUIConst.Tag, $"XUIWindowTask_Hide enter {obj.name}");
            
            if (obj.mono.hideAnim != null)
            {
                if (obj.mono.hideAnim == obj.mono.showAnim)
                {
                    obj.mono.showAnim.Revs(delegate { m_complete = true; });
                }
                else
                {
                    obj.mono.hideAnim.Play(delegate { m_complete = true; });
                }
                m_complete = false;
            }
            else
            {
                m_complete = true;
            }
            obj.isHideAnimating = true;
        }

        public override void OnLeave(XUIWindow obj)
        {
            XDebug.Log(XUIConst.Tag, $"XUIWindowTask_Hide leave {obj.name}");
            if (!m_complete)
            {
                _OnHide(obj);
                m_complete = true;
                obj.isHideAnimating = false;
            }
        }

        public override EnumTaskStatus OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (!m_complete)
            {
                return EnumTaskStatus.Running;
            }
            _OnHide(obj);
            m_complete = true;
            obj.isHideAnimating = false;
            return EnumTaskStatus.Success;
        }

        //隐藏Window处理
        protected void _OnHide(XUIWindow obj)
        {
            if (obj.canvas != null)
            {
                //将窗口消息器断开总消息器
                XMsgManager.Remove(obj.uiManager.MsgManager, obj.MsgManager);
                
                obj.mono.HideController();
                obj.uiManager.DelSort(obj);
                obj.uiManager.uiRoot.uiCanvasManager.RemoveClone(obj.canvas);
                obj.canvas = null;
                obj.gameObject.transform.SetParent(obj.uiManager.uiRoot.uiUnusedNode, false);
                obj.gameObject.SetActive(false);
            }
        }
    }

}

