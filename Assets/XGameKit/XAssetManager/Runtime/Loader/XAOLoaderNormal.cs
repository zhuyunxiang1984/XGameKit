using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XAssetManager
{
    public class XAOLoaderNormal : XAOLoader
    {
        public override bool IsDone
        {
            get { return m_Request != null && m_Request.isDone; }
        }

        public override Object GetValue()
        {
            return m_Request != null ? m_Request.asset : null;
        }

        protected AssetBundleRequest m_Request;

        public override T Load<T>(AssetBundle assetBundle, string assetName)
        {
            return assetBundle.LoadAsset<T>(assetName);
        }

        public override void LoadAsync<T>(AssetBundle assetBundle, string assetName)
        {
            m_Request = assetBundle.LoadAssetAsync<T>(assetName);
        }

        public override void StopAsync()
        {
            m_Request = null;
        }

        public override void Tick(float deltaTime)
        {
        }
        
    }

}