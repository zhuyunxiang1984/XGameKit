using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

public static class SampleLogger
{
    public static XDebugLogger Sample1;
    public static XDebugLogger Sample2;
    public static XDebugLogger Sample3;
}

public class XDebugSample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        XDebug.Initialize();
        XDebug.SetLogClass(typeof(SampleLogger));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SampleLogger.Sample1.Log("press 1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SampleLogger.Sample2.LogError("press 2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SampleLogger.Sample3.LogWarning("press 3");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            XDebug.Log("GGYY","press 4");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            XDebug.Log("press 5");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            var logger = SampleLogger.Sample1.CreateMutiLogger();
            logger.Append("hahaha");
            for (int i = 0; i < 10; ++i)
            {
                logger.Append(i.ToString("D2"));
            }
            logger.Log();
        }
    }
}
