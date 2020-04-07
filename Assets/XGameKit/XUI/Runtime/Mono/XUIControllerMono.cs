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

        public IXUIController Controller { get; protected set; }

        public virtual void Init(string windowName, XUIParamBundle paramBundle)
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
            Controller.Init(windowName, paramBundle, variables);
        }
        public virtual void Term()
        {
            Controller?.Term();
            Controller = null;

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
        
        //查找自己的子controller
        protected void _FindControllerMonos<T>(Transform node, ref List<T> list) where T : XUIControllerMono
        {
            if (node.childCount == 0)
                return;
            for (int i = 0; i < node.childCount; ++i)
            {
                var temp = node.GetChild(i);
                var mono = temp.GetComponent<T>();
                if (mono != null)
                {
                    list.Add(mono);
                }
                else
                {
                    _FindControllerMonos(temp, ref list);
                }
            }
        }
        
        //查找自己节点下所有的viewmono(包括自己),每个controller管理自己的viewMono
        protected void _FindViewMonos(Transform node, ref List<XUIViewMono> list)
        {
            XUIViewMono mono = null;
            mono = node.GetComponent<XUIViewMono>();
            if (mono != null)
            {
                list.Add(mono);
            }
            if (node.childCount > 0)
            {
                for (int i = 0; i < node.childCount; ++i)
                {
                    var temp = node.GetChild(i);
                    if (temp.GetComponent<XUIControllerMono>() != null)
                        continue;
                    mono = temp.GetComponent<XUIViewMono>();
                    if (mono != null)
                    {
                        list.Add(mono);
                    }
                    _FindViewMonos(temp, ref list);
                }
            }
        }
    }
}