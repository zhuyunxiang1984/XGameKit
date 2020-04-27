using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
#if UNITY_EDITOR
using UnityEditor;

namespace XGameKit.XAssetManager
{
    public class XAOLoaderSimulate : XAOLoader
    {
        public override bool IsDone
        {
            get { return true; }
        }

        public override Object GetValue()
        {
            return m_assetObject;
        }

        protected Object m_assetObject;

        public override T Load<T>(AssetBundle assetBundle, string assetName)
        {
            var assetPath = XABAssetNameConfig.GetAssetPath(assetName);
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public override void LoadAsync<T>(AssetBundle assetBundle, string assetName)
        {
            var assetPath = XABAssetNameConfig.GetAssetPath(assetName);
            m_assetObject = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public override void StopAsync()
        {
            
        }

        public override void Tick(float deltaTime)
        {
            
        }
        
    }

}
#endif

