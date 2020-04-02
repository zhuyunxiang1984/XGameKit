using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIControllerMono : MonoBehaviour
    {
        public bool lua;
        public string param;

        protected XUIViewMono[] m_viewMonos;
        public IXUIController Controller { get; protected set; }

        public virtual void Init(XUIWindow uiWindow)
        {
            param = param.Trim();
            if (string.IsNullOrEmpty(param))
            {
                Controller = new XUIControllerEmpty();
                return;
            }
            var variables = GetComponent<XMonoVariables>();
            if (variables == null)
            {
                Debug.LogError($"xuicontroller 没有找到XMonoVariables组件 !!!");
                return;
            }
            if (lua)
            {
                Controller = new XUIControllerLua();
            }
            else
            {
                //这里暂时用反射
                Controller = typeof(IXUIController).Assembly.CreateInstance(param) as IXUIController;
            }

            if (Controller == null)
            {
                Debug.LogError($"lua:{lua} param:{param} is not exist!");
                return;
                
            }
            Controller.Init(uiWindow, variables);
            
            //初始化节点下所有的viewmono
            m_viewMonos = transform.GetComponentsInChildren<XUIViewMono>();
            foreach (var viewMono in m_viewMonos)
            {
                viewMono.Init();
            }

        }
        public virtual void Term()
        {
            Controller?.Term();
            Controller = null;

            if (m_viewMonos != null)
            {
                foreach (var viewMono in m_viewMonos)
                {
                    viewMono.Term();
                }
                m_viewMonos = null;
            }
        }
        public virtual void Tick(float elapsedTime)
        {
            Controller?.Tick(elapsedTime);
        }

        public virtual void ShowController(object param = null)
        {
            Controller?.ShowController(param);
        }

        public virtual void HideController()
        {
            Controller?.HideController();
        }
        
        //查找自己节点下所有的第一层子节点
        protected void _FindChildren<T>(Transform node, ref List<T> children) where T : Component
        {
            if (node.childCount == 0)
                return;
            for (int i = 0; i < node.childCount; ++i)
            {
                var temp = node.GetChild(i);
                var mono = temp.GetComponent<T>();
                if (mono != null)
                {
                    children.Add(mono);
                }
                else
                {
                    _FindChildren(temp, ref children);
                }
            }
        }
    }
}