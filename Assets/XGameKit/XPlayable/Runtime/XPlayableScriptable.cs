using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XGameKit.Core
{
    /// <summary>
    /// 脚本动画
    /// </summary>
    public abstract class XPlayableScriptable : XPlayableBase
    {
        [LabelText("动画时长")]
        public float AnimLength = 0.5f;
    
        protected override float _GetPlayTime(float time)
        {
            return time > 0 ? time : AnimLength;
        }
    }
}
