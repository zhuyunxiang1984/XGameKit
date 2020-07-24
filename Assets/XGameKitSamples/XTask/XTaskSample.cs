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
            m_writer.WriteByte(1);
            m_writer.WriteInt16(2);
            m_writer.WriteInt32(3);
            m_writer.WriteFloat(3.14f);
            m_writer.WriteString("hello world! 你好吗？");
            Debug.Log(m_writer.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var data = m_writer.GetData();
            if (data != null)
            {
                m_reader.SetBytes(data);
                m_reader.Reset();
                Debug.Log(m_reader.ReadByte());
                Debug.Log(m_reader.ReadInt16());
                Debug.Log(m_reader.ReadInt32());
                Debug.Log(m_reader.ReadFloat());
                Debug.Log(m_reader.ReadString());

            }
        }
           
    }
}