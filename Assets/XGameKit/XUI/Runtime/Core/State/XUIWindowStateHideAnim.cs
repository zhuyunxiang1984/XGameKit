using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIWindowStateHideAnim : XState<XUIWindow>
    {
        protected bool m_complete;
        public override void OnEnter(XUIWindow obj)
        {
            if (obj.mono.hideAnim != null)
            {
                if (obj.mono.hideAnim == obj.mono.showAnim)
                {
                    obj.mono.showAnim.Revs(delegate { m_complete = true; });
                }
                else
                {
                    obj.mono.showAnim.Play(delegate { m_complete = true; });
                }
                
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
            if (m_complete)
            {
                obj.gameObject.SetActive(false);
                obj.stateMachine.ChangeState(XUIWindowStateMachine.stHide);
            }
        }
        public override string Transition(XUIWindow obj)
        {
            return string.Empty;
        }
    }
}