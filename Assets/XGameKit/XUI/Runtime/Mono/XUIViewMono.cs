using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public abstract class XUIViewMono : MonoBehaviour
    {
        protected string m_windowName;
        protected XUIParamBundle m_paramBundle;
        public virtual void Init(string windowName, XUIParamBundle paramBundle)
        {
            m_windowName = windowName;
            m_paramBundle = paramBundle;
        }

        public virtual void Term()
        {
            m_windowName = string.Empty;
            m_paramBundle.Clear();
        }
        public virtual void SetData(object param){}
    }
}
