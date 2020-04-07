using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XUI;

public class SampleView1 : XUIViewCSharp<SampleView1.View, SampleView1.Param>
{
    public class View
    {
        public XUILabel uiLabel;
    }

    public class Param : IXPoolable
    {
        public string Name;

        public void Reset()
        {
            
        }
    }

    protected override void SetData(View view, Param param)
    {
        view.uiLabel.SetText(param.Name);
    }
}
