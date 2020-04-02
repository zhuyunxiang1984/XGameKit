using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XUI;

public class SampleWindowController : XUIController<SampleWindowController.View>
{
    public class View
    {
        public XUIViewMono SampleView;
    }

    protected float m_time;
    public override void Register()
    {
        m_time = 0f;
    }
    public override void Unregister()
    {
    }
    public override void TickUI(float elapsedTime)
    {
        m_time += elapsedTime;

        var param = XObjectPool.Alloc<SampleView1.Param>();
        param.Name = m_time.ToString("f2");
        m_view.SampleView.SetData(param);
    }

    protected override void ShowUI(View view)
    {
        var param = XObjectPool.Alloc<SampleView1.Param>();
        m_view.SampleView.SetData(param);
    }
    
}
