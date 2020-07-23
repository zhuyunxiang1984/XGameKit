using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

public class XTaskSample : MonoBehaviour
{
    protected XTaskSchedule _Schedule = new XTaskSchedule();
    // Start is called before the first frame update
    void Start()
    {
        _Schedule.AddMethod(()=> { Debug.Log("execute something 111");  });
        _Schedule.AddWait(2f);
        _Schedule.AddMethod(()=> { Debug.Log("execute something 222");  });
        _Schedule.AddWait(2f);
        _Schedule.AddMethod(()=> { Debug.Log("execute something 333");  });
        _Schedule.AddWait(2f);
        _Schedule.AddMethod(()=> { Debug.Log("execute something 444");  });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _Schedule.Start((complete, failure)=> {
                Debug.Log($"complete:{complete} failure:{failure}"); 
            },
            (progress)=>
            {
                //Debug.Log($"progress:{progress}");
            });
        }
        _Schedule.Tick(Time.deltaTime);
    }
}