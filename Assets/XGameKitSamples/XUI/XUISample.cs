using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XBehaviorTree;
using XGameKit.XUI;

public class XUISample : MonoBehaviour
{
    private XUIManager m_uiManager;
    void Start()
    {
        XDebug.Initialize();
        m_uiManager = XService.AddService<XUIManager>();
        XBTClassFactory.Init(AutoClass_TaskClassReflect.datas);
    }

    protected float m_time;

    protected string[] m_windows = new[] {"SampleWindow1", "SampleWindow2", "SampleWindow3", "SampleWindow4",};
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var name = "SampleWindow1";
            OpenOrCloseWindow(name);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var name = "SampleWindow2";
            OpenOrCloseWindow(name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var name = "SampleWindow3";
            OpenOrCloseWindow(name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var name = "SampleWindow4";
            OpenOrCloseWindow(name);
        }

        if (Input.GetKey(KeyCode.R))
        {
            m_time += 1;
            if (m_time >= Random.Range(0, 10))
            {
                m_time = 0;
                OpenOrCloseWindow(m_windows[Random.Range(0,m_windows.Length)]);
            }
        }
    }

    void OpenOrCloseWindow(string name)
    {
        if(m_uiManager.IsShow(name))
            m_uiManager.HideWindow(name);
        else
            m_uiManager.ShowWindow(name);
    }
}
