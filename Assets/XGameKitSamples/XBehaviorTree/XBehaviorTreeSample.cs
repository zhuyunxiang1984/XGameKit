using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.XBehaviorTree;

public class XBehaviorTreeSample : MonoBehaviour
{
    public GameObject prefab;

    protected XBTBehavior m_behavior = new XBTBehavior();

    protected XBTNode m_root;
    // Start is called before the first frame update
    void Start()
    {
        //m_root = prefab.GetComponent<XBTNodeCompositeMono>().GetNode();
        XBTClassFactory.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_behavior.Start(m_root, null);
        }
        m_behavior.Update(null, Time.deltaTime);
    }
}
