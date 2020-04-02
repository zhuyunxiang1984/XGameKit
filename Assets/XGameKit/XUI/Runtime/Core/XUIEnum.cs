using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XUI
{
    //XUI层级枚举 (-50, 50](50, 100]
    public enum EnumXUILayer
    {
        Background  = -2, //背景层
        Bottom      = -1, //靠后
        Default     = 0,  //默认层
        Front       = 1,  //靠前
        Pop         = 2,  //弹出层
        Overlay     = 3,  //最顶层
    }
}