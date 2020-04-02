using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIWidget
    {
        public XUIManager uiManager { get; protected set; }
        public XUIWidgetMono mono;
        public object controller;
    }
}