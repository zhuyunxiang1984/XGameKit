using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIWindowMono : XUIWidgetMono
    {
        [System.Serializable]
        public struct LayerData
        {
            [HorizontalGroup, LabelText("层级")]
            public EnumXUILayer layer;
            [HorizontalGroup, HideLabel, PropertyRange(-XUIConst.LayerHalfPadding, XUIConst.LayerHalfPadding - 1)]
            public int offset;

            public int GetValue()
            {
                return (int) layer * XUIConst.LayerPadding + offset;
            }
        }
        [HideLabel]
        public LayerData layerData;

        [LabelText("遮罩")]
        public bool mask;

        [LabelText("缓存时间")] public int cacheTime = 5;
        
        [LabelText("打开动画")] public XPlayableBase showAnim;
        [LabelText("关闭动画")] public XPlayableBase hideAnim;

    }


}