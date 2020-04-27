using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XAssetManager;

public class XAssetManagerSample : MonoBehaviour
{
    protected XAssetManager m_assetManager;
    // Start is called before the first frame update
    void Start()
    {
        XDebug.Initialize();
        m_assetManager = XService.AddService<XAssetManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_assetManager.LoadAssetAsyn<GameObject>("Box1", (name, asset) =>
            {
                Debug.Log($"complete {name} {asset}");
                GameObject.Instantiate(asset, Random.insideUnitSphere * 50f, Quaternion.identity);
            });
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_assetManager.LoadAssetAsyn<GameObject>("Box2", (name, asset) =>
            {
                Debug.Log($"complete {name} {asset}");
            });
        }
    }
}
