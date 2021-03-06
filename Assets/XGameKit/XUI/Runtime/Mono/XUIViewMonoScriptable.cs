﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIViewMonoScriptable : XUIViewMono
    {
        public bool lua;
        public string param;

        protected IXUIView m_view;

        public override void Init(string windowName, XUIParamBundle paramBundle)
        {
            base.Init(windowName, paramBundle);
            
            param = param.Trim();//去除意外的空格
            if (string.IsNullOrEmpty(param))
            {
                m_view = new XUIViewEmpty();
                return;
            }
            var variables = GetComponent<XMonoVariables>();
            if (variables == null)
            {
                Debug.LogError($"xuiview 没有找到XMonoVariables组件 !!!");
                return;
            }
            if (lua)
            {
                m_view = new XUIViewLua();
            }
            else
            {
                //这里暂时用反射
                m_view = typeof(IXUIView).Assembly.CreateInstance(param) as IXUIView;
            }

            if (m_view == null)
            {
                Debug.LogError($"lua:{lua} param:{param} is not exist!");
                return;
                
            }
            m_view.Init(variables);
        }

        public override void Term()
        {
            base.Term();
            
            m_view?.Term();
            m_view = null;
        }
        public override void SetData(object param)
        {
            m_view?.SetData(param);
        }
    }
}

