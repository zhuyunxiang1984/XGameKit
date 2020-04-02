using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    //view接口
    public interface IXUIView
    {
        void Init(XMonoVariables variables);
        void Term();
        void SetData(object param);
    }

    //csharp的view
    public abstract class XUIViewCSharp<VIEW, PARAM> : IXUIView 
        where VIEW : class, new()
        where PARAM : class, IXPoolable, new()
    {
        protected VIEW m_view = new VIEW();
        protected PARAM m_param;

        public void Init(XMonoVariables variables)
        {
            variables.Inject<VIEW>(m_view);
        }
        public void Term()
        {
            if (m_param != null)
            {
                XObjectPool.Free(m_param);
                m_param = null;
            }
        }
        public void SetData(object param)
        {
            if (m_param != null)
            {
                XObjectPool.Free(m_param);
                m_param = null;
            }
            m_param = param as PARAM;
            SetData(m_view, m_param);
        }
        protected abstract void SetData(VIEW view, PARAM param);
    }
    
    //lua的view
    public class XUIViewLua : IXUIView
    {
        public void Init(XMonoVariables variables)
        {
            
        }
        public void Term()
        {
            
        }
        public void SetData(object param)
        {
            
        }
    }

    //空view
    public class XUIViewEmpty : IXUIView
    {
        public void Init(XMonoVariables variables)
        {
        }
        public void Term()
        {
            
        }
        public void SetData(object param)
        {
        }
    }

}
