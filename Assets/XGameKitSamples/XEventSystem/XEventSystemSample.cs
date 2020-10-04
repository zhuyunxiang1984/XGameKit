using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

public class XEventSystemSample : MonoBehaviour
{
    protected XEventManager m_EventManager;
    void Start()
    {
        XService.AddService<XEventManager>();
        
        var aa = XService.AddService<XMsgManager>();
        var bb = XService.AddService<XMsgManager>("bb");
        XMsgManager.Append(aa, bb);
        
        aa.Register<XSampleMsg1>(OnHandleMsg1aa);
        bb.Register<XSampleMsg1>(OnHandleMsg1bb);
        
        //
        m_EventManager = XService.GetService<XEventManager>();
        m_EventManager.AddListener<int>("test1", (param1) => { Debug.Log(param1);});
        m_EventManager.AddListener<int, int>("test2", (param1,param2) => { Debug.Log($"{param1},{param2}");});
        m_EventManager.AddListener<int, int, int>("test3", (param1,param2,param3) => { Debug.Log($"{param1},{param2},{param3}");});
        m_EventManager.AddListener<XSampleEvent1>("test4", (evt) => { Debug.Log(evt.value);});
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_EventManager.PostEvent("test1",100);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_EventManager.PostEvent("test2",100,200);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_EventManager.PostEvent("test3",100,200,300);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            m_EventManager.PostEvent<XSampleEvent1>("test4", (evt) => { evt.value = 999;});
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            XDebug.Log("XObjectPool", XObjectPool.DumpLog());
        }
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


public class XSampleEvent1 : XCustomEvent<XSampleEvent1>
{
    public int value;
}

public class XSampleMsg1 : XMessage
{
    public int value;
    public override void Reset()
    {
    }
}