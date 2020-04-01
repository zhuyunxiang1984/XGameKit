using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

public class XEventSystemSample : MonoBehaviour
{
    void Start()
    {
        XService.AddService<XEvtManager>();
        
        var aa = XService.AddService<XMsgManager>();
        var bb = XService.AddService<XMsgManager>("bb");
        XMsgManager.Append(aa, bb);
        
        aa.Register<XSampleMsg1>(OnHandleMsg1aa);
        bb.Register<XSampleMsg1>(OnHandleMsg1bb);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var evtManager = XService.GetService<XEvtManager>();
            var evt = evtManager.GetEvent<XSampleEvent1>();
            evt.value = 100;
            evtManager.PostEvent(evt);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var evtManager = XService.GetService<XEvtManager>();
            var evt = evtManager.GetEvent<XSampleEvent1>();
            evt.value = 200;
            evtManager.PostEvent(evt);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var msgManager = XService.GetService<XMsgManager>();
            var mgr = msgManager.GetMsg<XSampleMsg1>();
            mgr.value = 200;
            msgManager.SendMsg(mgr);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            XDebug.Log("XObjectPool", XObjectPool.DumpLog());
        }
    }

    void OnHandleEvent1(XEvent evtbase)
    {
        var evt = evtbase as XSampleEvent1;
        Debug.Log(evt.value);
    }

    bool OnHandleMsg1aa(XMessage msgbase)
    {
        var msg = msgbase as XSampleMsg1;
        Debug.Log($"OnHandleMsg1aa {msg.value}");
        return false;
    }
    bool OnHandleMsg1bb(XMessage msgbase)
    {
        var msg = msgbase as XSampleMsg1;
        Debug.Log($"OnHandleMsg1bb {msg.value}");
        return false;
    }
}


public class XSampleEvent1 : XEvent
{
    public int value;
    public override void Reset()
    {
        
    }
}

public class XSampleMsg1 : XMessage
{
    public int value;
    public override void Reset()
    {
    }
}