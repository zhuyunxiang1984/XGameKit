using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    [ExecuteInEditMode]
    public class XBTNodeMono : MonoBehaviour
    {
        public string className;
        
        private void Update()
        {
            if (string.IsNullOrEmpty(className))
            {
                gameObject.name = "NoTask";
            }
            else
            {
                gameObject.name = className;
            }
        }
    }
}
