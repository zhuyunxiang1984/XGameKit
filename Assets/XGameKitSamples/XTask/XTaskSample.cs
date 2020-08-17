using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

public class XTaskSample : MonoBehaviour
{
    protected XTaskSchedule _Schedule = new XTaskSchedule();

    protected XStreamBinaryWriter m_writer = new XStreamBinaryWriter();
    protected XStreamBinaryReader m_reader = new XStreamBinaryReader();
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
        _Schedule.Tick(Time.deltaTime);

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
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_writer.Reset();
            for (int i = 0; i < 1024; ++i)
            {
                m_writer.WriteInt32(i);

            }
            Debug.Log(m_writer.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var data = m_writer.GetData();
            if (data != null)
            {
                m_reader.SetBytes(data);
                m_reader.Reset();
                for (int i = 0; i < 1024; ++i)
                {
                    Debug.Log(m_reader.ReadInt32());

                }
            }
        }
           
    }
}