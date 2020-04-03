using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIWindowStateMachine : XStateMachine<XUIWindow>
    {
        public const string stShow = "stShow";
        public const string stIdle = "stIdle";
        public const string stHide = "stHide";
        public const string stLoad = "stLoad";
        public const string stUnload = "stUnload";
        public const string stCache = "stCache"; 
        public const string stDestroy = "stDestroy";

        public XUIWindowStateMachine()
        {
            AddState(stLoad, new XUIWindowStateLoad(), true);
            AddState(stUnload, new XUIWindowStateUnload());
            AddState(stShow, new XUIWindowStateShow());
            AddState(stIdle, new XUIWindowStateIdle());
            AddState(stHide, new XUIWindowStateHide());
            AddState(stCache, new XUIWindowStateCache());
            AddState(stDestroy, new XUIWindowStateDestroy());
        }
    }

}
