using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

public class XTaskSample : MonoBehaviour
{
    protected XTaskSchedule<XTaskSampleData> _Schedule = new XTaskSchedule<XTaskSampleData>();
    // Start is called before the first frame update
    void Start()
    {
        _Schedule.AddMethod((data)=> { Debug.Log("execute something 111");  });
        _Schedule.AddWait(2f);
        _Schedule.AddMethod((data)=> { Debug.Log("execute something 222");  });
        _Schedule.AddWait(2f);
        _Schedule.AddMethod((data)=> { Debug.Log("execute something 333");  });
        _Schedule.AddWait(2f);
        _Schedule.AddMethod((data)=> { Debug.Log("execute something 444");  });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _Schedule.Start(new XTaskSampleData());
        }
        _Schedule.Update(Time.deltaTime);
    }
}

#region 例子

public class XTaskSampleData
{
}

#endregion