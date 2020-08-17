using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XEntitas;

public class XEntitasSample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        XArrayMask mask = new XArrayMask(128);
        mask.Add(0);
        mask.Add(1);
        mask.Add(56);
        mask.Add(48);
        mask.Remove(48);
        Debug.Log(mask.ToString());

        Debug.Log(mask.Check(56));
        Debug.Log(mask.Check(48));
        Debug.Log(mask.Check(97));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
