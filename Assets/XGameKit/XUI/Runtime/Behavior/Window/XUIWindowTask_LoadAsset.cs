using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.XBehaviorTree;

namespace XGameKit.XUI
{
    [BTTaskMemo("[XUI]加载资源")]
    public class XUIWindowTask_LoadAsset : XBTTask<XUIWindow>
    {
        protected GameObject m_asset;
        public override void OnEnter(XUIWindow obj)
        {
            m_asset = null;
            obj.uiManager.AssetLoader.LoadAssetAsyn<GameObject>(obj.resName, (asset)=> m_asset = asset);
        }

        public override void OnLeave(XUIWindow obj)
        {
        }

        public override EnumTaskStatus OnUpdate(XUIWindow obj, float elapsedTime)
        {
            if (m_asset == null)
                return EnumTaskStatus.Running;

            if (obj.gameObject == null)
            {
                obj.gameObject = GameObject.Instantiate(m_asset, obj.uiManager.uiRoot.uiUnusedNode);
                obj.gameObject.SetActive(false);
            }
            if (obj.mono == null)
                obj.mono = obj.gameObject.GetComponent<XUIWindowMono>();
            obj.mono.Init(obj.name, obj.paramBundle);
            obj.cacheTime = obj.mono.cacheTime;
            
            return EnumTaskStatus.Success;
        }
    }


}
