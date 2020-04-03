using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIWindowStateShowAnim : XState<XUIWindow>
    {
        protected bool m_complete;
        public override void OnEnter(XUIWindow obj)
        {
            int index = obj.uiManager.GetSort(obj);
            obj.uiManager.AddSort(obj, index);
            obj.gameObject.SetActive(true);
            if (obj.mono.showAnim != null)
            {
                obj.mono.showAnim.Play(delegate { m_complete = true; });
                m_complete = false;
            }
            else
            {
                m_complete = true;
            }
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override void OnUpdate(XUIWindow obj, float elapsedTime)
        {
            obj.mono.Tick(elapsedTime);
            if (m_complete)
            {
                obj.stateMachine.ChangeState(XUIWindowStateMachine.stIdle);
            }
        }

        public override string Transition(XUIWindow obj)
        {
            if (!obj.isShow)
                return XUIWindowStateMachine.stHideAnim;
            return string.Empty;
        }
    }
}