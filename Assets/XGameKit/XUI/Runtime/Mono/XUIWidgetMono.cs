using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XGameKit.XUI
{
    public class XUIWidgetMono : XUIControllerMono
    {
        protected List<XUIWidgetMono> m_children = new List<XUIWidgetMono>();
        
        public override void Init(XUIWindow uiWindow)
        {
            base.Init(uiWindow);
            
            m_children.Clear();
            _FindChildren(transform, ref m_children);
            foreach (var child in m_children)
            {
                child.Init(uiWindow);
                child.Controller.SetParent(Controller);
            }
        }
        public override void Term()
        {
            base.Term();
            
            foreach (var child in m_children)
            {
                child.Term();
            }
        }
        public override void Tick(float elapsedTime)
        {
            base.Tick(elapsedTime);
            foreach (var child in m_children)
            {
                child.Tick(elapsedTime);
            }
        }

        public override void ShowController(object param = null)
        {
            base.ShowController(param);
            foreach (var child in m_children)
            {
                child.ShowController();
            }
            
        }

        public override void HideController()
        {
            base.HideController();
            foreach (var child in m_children)
            {
                child.HideController();
            }
        }
    }
}
