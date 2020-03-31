using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

public class XTaskSample : MonoBehaviour
{
    protected XTaskSampleSchedule _Schedule = new XTaskSampleSchedule();
    // Start is called before the first frame update
    void Start()
    {
        _Schedule.AddTask(new XTaskStepWait<int>(2f));
        _Schedule.AddTask(new XTaskStepWait<int>(2f));
        _Schedule.AddTask(new XTaskStepWait<int>(2f));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _Schedule.Start(1);
        }
        _Schedule.Update(Time.deltaTime);
    }
}

#region 例子

public class XTaskSampleSchedule : XTaskSchedule<int>
{
    public class SampleData : XTaskData<int>
    {
        
    }
    protected override XTaskData<int> _CreateTaskData(int data)
    {
        return XObjectPool.Alloc<SampleData>();
    }
}

#endregion