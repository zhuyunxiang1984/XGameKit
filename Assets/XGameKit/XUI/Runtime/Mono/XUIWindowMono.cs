using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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
                return ((int) layer * XUIConst.LayerPadding + offset) * XUIConst.LayerPaddingInner;
            }
        }
        [HideLabel]
        public LayerData layerData;

        [LabelText("遮罩")]
        public bool mask;

        [LabelText("缓存时间")] public int cacheTime = 5;

    }


}