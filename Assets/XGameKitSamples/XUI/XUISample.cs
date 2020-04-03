using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XUI;

public class XUISample : MonoBehaviour
{
    private XUIManager m_uiManager;
    void Start()
    {
        m_uiManager = XService.AddService<XUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var name = "SampleWindow1";
            if(m_uiManager.IsShow(name))
                m_uiManager.HideWindow(name);
            else
                m_uiManager.ShowWindow(name);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var name = "SampleWindow2";
            if(m_uiManager.IsShow(name))
                m_uiManager.HideWindow(name);
            else
                m_uiManager.ShowWindow(name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var name = "SampleWindow3";
            if(m_uiManager.IsShow(name))
                m_uiManager.HideWindow(name);
            else
                m_uiManager.ShowWindow(name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var name = "SampleWindow4";
            if(m_uiManager.IsShow(name))
                m_uiManager.HideWindow(name);
            else
                m_uiManager.ShowWindow(name);
        }
    }
}
