using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using XGameKit.Core;

#endif

namespace XGameKit.XAssetManager
{
    public class XABRelationShipConfig : ScriptableObject
    {
#if UNITY_EDITOR
        public const string AssetPathStatic = "Assets/XGameKitSettings/Runtime/XABRelationShipStaticConfig.asset";
        public const string AssetPathHotfix = "Assets/XGameKitSettings/Runtime/XABRelationShipHotfixConfig.asset";
#endif
        
        [System.Serializable]
        public class Data
        {
            [HorizontalGroup, LabelText("资源"), LabelWidth(30), DisplayAsString]
            public string AssetName;
            [HorizontalGroup, LabelText("包名"), LabelWidth(30), DisplayAsString]
            public string AssetBundleName;
#if UNITY_EDITOR  
            [LabelText("资源路径"), LabelWidth(30), DisplayAsString]
            public string AssetPath;
#endif
        }

        public List<Data> Datas = new List<Data>();
        
#if UNITY_EDITOR
        protected Dictionary<string, Data> m_flags = new Dictionary<string,Data>();
        public void Clear()
        {
            Datas.Clear();
            m_flags.Clear();
        }

        public void AddData(string assetPath, string bundleName)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                //检索文件
                var files = Directory.GetFiles(assetPath);
                foreach (var file in files)
                {
                    if (file.EndsWith(".meta"))
                        continue;
                    AddData(Path.GetFileNameWithoutExtension(file), bundleName, file);
                }
                //检索文件夹
                var paths = Directory.GetDirectories(assetPath);
                foreach (var path in paths)
                {
                    AddData(path, bundleName);
                }
            }
            else
            {
                AddData(Path.GetFileNameWithoutExtension(assetPath), bundleName, assetPath);
            }
        }
        public void AddData(string assetName, string bundleName, string assetPath)
        {
            if (m_flags.ContainsKey(assetName))
            {
                XDebug.Log(XABConst.Tag, $"资源{assetName}重复  {assetPath} 已经存在于{m_flags[assetName].AssetPath}");
                return;
            }
            var data = new Data();
            data.AssetName = assetName.ToLower();
            data.AssetBundleName = bundleName.ToLower();
            data.AssetPath = assetPath;
            Datas.Add(data);
            m_flags.Add(assetName, data);
        }

        public Dictionary<string, bool> GetBundleNames()
        {
            var result = new Dictionary<string, bool>();
            foreach (var data in Datas)
            {
                if (result.ContainsKey(data.AssetBundleName))
                    continue;
                result.Add(data.AssetBundleName, true);
            }
            return result;
        }
#endif
    }
}

