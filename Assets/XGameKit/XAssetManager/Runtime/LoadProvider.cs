using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using XGameKit.Core;
using Object = UnityEngine.Object;

namespace XGameKit.XAssetManager
{
    
    public class LoadProvider : ILoadProvider
    {
        public class AssetData
        {
            public enum EnumState
            {
                None = 0,
                Loading,
                Completed,
            }
            public EnumState state;
            public Object asset;
            public int referenceCount;


            public void Reset()
            {
                state = EnumState.None;
                asset = null;
                referenceCount = 0;
            }
        
        }
        public class BundleData
        {
            public enum EnumState
            {
                None = 0,
                Loading,
                Completed,
            }
            public EnumState state;
            public AssetBundle bundle;
            public int referenceCount;

            public void Reset()
            {
                state = EnumState.None;
                bundle = null;
                referenceCount = 0;
            }
        }
        
        private XAssetDescription _description;
        private Dictionary<string, AssetData> _assetDatas = new Dictionary<string, AssetData>();
        private Dictionary<string, BundleData> _bundleDatas = new Dictionary<string, BundleData>();
        
        public void SetDescription(XAssetDescription description)
        {
            _description = description;
        }
        #region 资源加载
        
        public T LoadAsset<T>(string assetName) where T : Object
        {
            IncreaseAssetReference(assetName);
            return LoadAssetInternal<T>(assetName);
        }

        public IEnumerator LoadAssetAsync<T>(string assetName, Action<T> OnComplete) where T : Object
        {
            IncreaseAssetReference(assetName);
            yield return LoadAssetAsyncInternal<T>(assetName, OnComplete);
            var assetData = GetAssetData(assetName);
            if (assetData.referenceCount < 1)
            {
                assetData.Reset();
                var bundleName = _description.GetBundleByAsset(assetName);
                UnloadBundle(bundleName);
            }
            OnComplete?.Invoke(assetData.asset as T);
        }
        
        public void UnloadAsset(string assetName)
        {
            var assetData = GetAssetData(assetName);
            if (assetData == null || assetData.referenceCount < 1)
                return;
            DecreaseAssetReference(assetName);
            if (assetData.state == AssetData.EnumState.Completed && assetData.referenceCount < 1)
            {
                assetData.Reset();
                var bundleName = _description.GetBundleByAsset(assetName);
                UnloadBundle(bundleName);
            }
        }

        private AssetData GetAssetData(string assetName)
        {
            if (_assetDatas.ContainsKey(assetName))
                return _assetDatas[assetName];
            return null;
        }
        private AssetData GetOrCreateAssetData(string assetName)
        {
            if (_assetDatas.ContainsKey(assetName))
                return _assetDatas[assetName];
            var assetData = new AssetData();
            assetData.Reset();
            _assetDatas.Add(assetName, assetData);
            return assetData;
        }

        private void IncreaseAssetReference(string assetName)
        {
            var assetData = GetOrCreateAssetData(assetName);
            ++assetData.referenceCount;
        }

        private void DecreaseAssetReference(string assetName)
        {
            var assetData = GetOrCreateAssetData(assetName);
            --assetData.referenceCount;
        }
        private T LoadAssetInternal<T>(string assetName) where T : Object
        {
            var assetData = GetOrCreateAssetData(assetName);
            if (assetData.asset != null)
            {
                return assetData.asset as T;
            }
            var bundleName = _description.GetBundleByAsset(assetName);
            var bundle = LoadBundle(bundleName);
            assetData.asset = bundle.LoadAsset<T>(assetName);
            return assetData.asset as T;
        }
        private IEnumerator LoadAssetAsyncInternal<T>(string assetName, Action<T> OnComplete) where T : Object
        {
            var assetData = GetOrCreateAssetData(assetName);
            if (assetData.state == AssetData.EnumState.Completed)
            {
                yield return null;
                yield break;
            }

            if (assetData.state == AssetData.EnumState.Loading)
            {
                while (assetData.state != AssetData.EnumState.Completed)
                    yield return null;
                yield break;
            }

            assetData.state = AssetData.EnumState.Loading;
            
            var bundleName = _description.GetBundleByAsset(assetName);
            yield return LoadBundleAsync(bundleName);
            var bundleData = GetOrCreateBundleData(bundleName);
            var request = bundleData.bundle.LoadAssetAsync<T>(assetName);
            yield return request;
            if (assetData.state == AssetData.EnumState.Completed)
            {
                yield break;
            }
            assetData.state = AssetData.EnumState.Completed;
            assetData.asset = request.asset;
        }
        
        #endregion
        
        
        #region 资源包加载
        
        public AssetBundle LoadBundle(string bundleName)
        {
            IncreaseBundleReference(bundleName);
            return LoadBundleInternal(bundleName);
        }
        
        public IEnumerator LoadBundleAsync(string bundleName)
        {
            IncreaseBundleReference(bundleName);
            yield return LoadBundleAsyncInternal(bundleName);
            var bundleData = GetOrCreateBundleData(bundleName);
            if (bundleData.referenceCount < 1)
            {
                bundleData.bundle.Unload(true);
                bundleData.Reset();
            }
        }
        
        public void UnloadBundle(string bundleName)
        {
            var bundleData = GetBundleData(bundleName);
            if (bundleData == null || bundleData.referenceCount < 1)
                return;
            DecreaseBundleReference(bundleName);
            UnloadBundleInternal(bundleName);
        }
        
        public BundleData GetBundleData(string bundleName)
        {
            if (_bundleDatas.ContainsKey(bundleName))
                return _bundleDatas[bundleName];
            return null;
        }

        public BundleData GetOrCreateBundleData(string bundleName)
        {
            if (_bundleDatas.ContainsKey(bundleName))
                return _bundleDatas[bundleName];
            var bundleData = new BundleData();
            _bundleDatas.Add(bundleName, bundleData);
            return bundleData;
        }
        private void IncreaseBundleReference(string bundleName)
        {
            var bundleData = GetOrCreateBundleData(bundleName);
            ++bundleData.referenceCount;
            var dependencies = _description.GetDependencies(bundleName);
            foreach (var dependency in dependencies)
            {
                IncreaseBundleReference(dependency);
            }
        }
        private void DecreaseBundleReference(string bundleName)
        {
            var bundleData = GetOrCreateBundleData(bundleName);
            --bundleData.referenceCount;
            var dependencies = _description.GetDependencies(bundleName);
            foreach (var dependency in dependencies)
            {
                DecreaseBundleReference(dependency);
            }
        }
        private AssetBundle LoadBundleInternal(string bundleName)
        {
            var dependencies = _description.GetDependencies(bundleName);
            foreach (var dependency in dependencies)
            {
                LoadBundleInternal(dependency);
            }
            var bundleData = GetOrCreateBundleData(bundleName);
            if (bundleData.state == BundleData.EnumState.Completed)
            {
                return bundleData.bundle;
            }
            //TODO:
            var bundle = AssetBundle.LoadFromFile("");
            bundleData.state = BundleData.EnumState.Completed;
            bundleData.bundle = bundle;
            return bundle;
        }
        private IEnumerator LoadBundleAsyncInternal(string bundleName)
        {
            var dependencies = _description.GetDependencies(bundleName);
            foreach (var dependency in dependencies)
            {
                yield return LoadBundleAsyncInternal(dependency);
            }
            var bundleData = GetOrCreateBundleData(bundleName);
            if (bundleData.state == BundleData.EnumState.Completed)
            {
                yield break;
            }
            if (bundleData.state == BundleData.EnumState.Loading)
            {
                while (bundleData.state != BundleData.EnumState.Completed)
                {
                    yield return null;
                }
                yield break;
            }
            bundleData.state = BundleData.EnumState.Loading;
            //TODO:
            var createRequest = AssetBundle.LoadFromFileAsync("");
            yield return createRequest;
            //在异步加载过程中，bundle已经被同步加载了，那么就不同再设置一次了，视为异步白加载了
            if (bundleData.state == BundleData.EnumState.Completed)
            {
                createRequest.assetBundle.Unload(true);
                yield break;
            }
            bundleData.state = BundleData.EnumState.Completed;
            bundleData.bundle = createRequest.assetBundle;
        }
        private void UnloadBundleInternal(string bundleName)
        {
            //TODO:
            var dependencies = _description.GetDependencies(bundleName);
            foreach (var dependency in dependencies)
            {
                UnloadBundleInternal(dependency);
            }
            var bundleData = GetOrCreateBundleData(bundleName);
            if (bundleData.state == BundleData.EnumState.Completed && bundleData.referenceCount < 1)
            {
                bundleData.bundle.Unload(true);
                bundleData.Reset();
            }
        }
        
        #endregion
        
    }
}

