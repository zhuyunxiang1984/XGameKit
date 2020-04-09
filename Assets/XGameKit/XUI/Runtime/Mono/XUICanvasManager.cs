using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUICanvasManager : XPrefabClone<Canvas>
    {
        protected override void _UpdateName()
        {
            base._UpdateName();
            
            foreach (var clone in m_clones)
            {
                clone.name = $"{prefab.name}_sort:{clone.sortingOrder}";
            }
        }
    }

}