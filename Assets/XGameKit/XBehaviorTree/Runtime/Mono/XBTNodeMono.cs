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
        
#if UNITY_EDITOR
        private void Update()
        {
            //根节点不做强制设置名字
            if (transform.parent == null)
                return;
            var dictAllTaskClass = XBTUtilities.GetAllTaskClassDictEditor();
            if (!dictAllTaskClass.ContainsKey(className))
            {
                gameObject.name = $"{className} is not exist";
                return;
            }

            var data = dictAllTaskClass[className];
            if (string.IsNullOrEmpty(data.Item2))
            {
                gameObject.name = data.Item1;
            }
            else
            {
                gameObject.name = data.Item2;
            }
        }
    }
#endif
}
