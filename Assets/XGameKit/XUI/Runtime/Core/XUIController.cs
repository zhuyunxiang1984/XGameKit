using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    //controller接口
    public interface IXUIController
    {
        XMsgManager MsgManager { get; set; }
        //初始化
        void Init(string windowName, XUIParamBundle paramBundle, XMonoVariables variables);
        //销毁
        void Term();
        //设置父节点
        void SetParent(IXUIController parent);
        //显示
        void ShowController(object param);
        //隐藏
        void HideController();
        //轮询
        void Tick(float elapsedTime);
    }
    
    //controller基础实现
    public abstract class XUIController : IXUIController
    {
        public XMsgManager MsgManager { get; set; } = new XMsgManager();
        
        protected IXUIController m_parent;
        protected string m_windowName;
        protected XUIParamBundle m_paramBundle;
        protected bool m_registered = false;
        
        public virtual void Init(string windowName, XUIParamBundle paramBundle, XMonoVariables variables)
        {
            m_windowName = windowName;
            m_paramBundle = paramBundle;
        }

        public virtual void Term()
        {
            if (m_parent != null)
            {
                XMsgManager.Remove(m_parent.MsgManager, MsgManager);
                m_parent = null;
            }
        }

        public void SetParent(IXUIController parent)
        {
            if (m_parent == parent)
                return;
            if (m_parent != null)
            {
                XMsgManager.Remove(m_parent.MsgManager, MsgManager);
                m_parent = null;
            }
            if (parent != null)
            {
                m_parent = parent;
                XMsgManager.Append(m_parent.MsgManager, MsgManager);
            }
        }

        public void ShowController(object param)
        {
            if (!m_registered)
            {
                Register();
                m_registered = true;
            }
            ShowUI(param);
        }

        public void HideController()
        {
            if (m_registered)
            {
                Unregister();
                m_registered = false;
            }
        }

        public virtual void Tick(float elapsedTime)
        {
            MsgManager.Tick(elapsedTime);
            TickUI(elapsedTime);
        }
        public abstract void Register();
        public abstract void Unregister();
        public abstract void ShowUI(object param);
        public abstract void TickUI(float elapsedTime);
    }
    
    //csharp的controller
    public abstract class XUIController<VIEW> : XUIController
        where VIEW : class, new()
    {
        protected VIEW m_view = new VIEW();
        
        public override void Init(string windowName, XUIParamBundle paramBundle, XMonoVariables variables)
        {
            base.Init(windowName, paramBundle, variables);
            variables.Inject(m_view);
        }

        public override void ShowUI(object param)
        {
            ShowUI(m_view);
        }
        protected abstract void ShowUI(VIEW view);
    }
    //csharp的controller
    public abstract class XUIController<VIEW, PARAM> : XUIController
        where VIEW : class, new()
        where PARAM : class, IXPoolable, new()
    {
        protected VIEW m_view = new VIEW();
        protected PARAM m_param;
        public override void Init(string windowName, XUIParamBundle paramBundle, XMonoVariables variables)
        {
            base.Init(windowName, paramBundle, variables);
            variables.Inject(variables);
        }

        public override void Term()
        {
            base.Term();
            if (m_param != null)
            {
                XObjectPool.Free(m_param);
                m_param = null;
            }
        }

        public override void ShowUI(object param)
        {
            if (m_param != null)
            {
                XObjectPool.Free(m_param);
                m_param = null;
            }
            m_param = param as PARAM;
            ShowUI(m_view, m_param);
        }

        protected abstract void ShowUI(VIEW view, PARAM param);
    }
    
    //lua的controller
    public class XUIControllerLua : XUIController
    {
        public override void Register()
        {
        }
        public override void Unregister()
        {
        }
        public override void ShowUI(object param)
        {
        }
        public override void TickUI(float elapsedTime)
        {
        }
    }
    
    //空controller
    public class XUIControllerEmpty : XUIController
    {
        public override void Register()
        {
        }
        public override void Unregister()
        {
        }
        public override void ShowUI(object param)
        {
        }
        public override void TickUI(float elapsedTime)
        {
        }
    }

}

