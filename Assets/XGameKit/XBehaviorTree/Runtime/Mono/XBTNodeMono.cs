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
#if UNITY_EDITOR
        public string memo; //描述用于注释和理解
#endif
        public string className;
        
#if UNITY_EDITOR
        private void Update()
        {
            if (string.IsNullOrEmpty(memo))
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
            else
            {
                gameObject.name = memo;
            }
        }
    }
#endif
}
