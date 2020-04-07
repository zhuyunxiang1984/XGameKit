using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XGameKit.XUI
{
    public class XUIWidgetMono : XUIControllerMono
    {
        protected List<XUIWidgetMono> m_children = new List<XUIWidgetMono>();
        protected List<XUIViewMono> m_viewMonos = new List<XUIViewMono>();
        public override void Init(string windowName, XUIParamBundle paramBundle)
        {
            base.Init(windowName, paramBundle);

            m_children.Clear();
            _FindControllerMonos(transform, ref m_children);
            foreach (var child in m_children)
            {
                child.Init(windowName, paramBundle);
                child.Controller.SetParent(Controller);
            }
            //初始化节点下所有的viewmono
            m_viewMonos.Clear();
            _FindViewMonos(transform, ref m_viewMonos);
            foreach (var viewMono in m_viewMonos)
            {
                viewMono.Init(windowName, paramBundle);
            }
        }
        public override void Term()
        {
            base.Term();
            
            foreach (var child in m_children)
            {
                child.Term();
            }
            m_children.Clear();
            
            foreach (var viewMono in m_viewMonos)
            {
                viewMono.Term();
            }
            m_viewMonos.Clear();
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
