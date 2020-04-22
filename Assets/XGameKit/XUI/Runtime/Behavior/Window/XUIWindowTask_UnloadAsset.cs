using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.XBehaviorTree;

namespace XGameKit.XUI
{
    [BTTaskMemo("[XUI]卸载资源")]
    public class XUIWindowTask_UnloadAsset : XBTTask<XUIWindow>
    {
        public override void OnEnter(XUIWindow obj)
        {
            if (obj.mono != null)
            {
                obj.mono.Term();
                obj.mono = null;
            }
            if (obj.gameObject != null)
            {
                GameObject.Destroy(obj.gameObject);
                obj.gameObject = null;
            }
            obj.paramBundle.AssetLoader.UnloadAsset(obj.resName);
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override EnumTaskStatus OnUpdate(XUIWindow obj, float elapsedTime)
        {
            return EnumTaskStatus.Success;
        }
    }


}
