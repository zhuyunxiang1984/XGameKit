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
            m_uiManager.ShowWindow("xxx", null);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_uiManager.HideWindow("xxx");
        }
    }
}
