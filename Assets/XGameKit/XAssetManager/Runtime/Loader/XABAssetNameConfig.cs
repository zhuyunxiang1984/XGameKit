using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.IO;

using XGameKit.Core;

#if UNITY_EDITOR
using UnityEditor;


namespace XGameKit.XAssetManager
{
    [System.Serializable]
    public class XABAssetNameConfig : ScriptableObject
    {
        public const string AssetPath = "Assets/XGameKitSettings/Editor/EditorXABAssetNameConfig.asset";
        
        [System.Serializable]
        public class Data
        {
            [HorizontalGroup, LabelText("资源"), LabelWidth(30), DisplayAsString]
            public string AssetName;
            [HorizontalGroup, LabelText("包名"), LabelWidth(30), DisplayAsString]
            public string AssetBundleName;
            [LabelText("资源路径"), LabelWidth(30), DisplayAsString]
            public string AssetPath;
        }

        [LabelText("跟包资源")]
        public List<Data> StaticDatas = new List<Data>();
        [LabelText("热更资源")]
        public List<Data> HotfixDatas = new List<Data>();
        
        protected Dictionary<string, Data> m_flags = new Dictionary<string,Data>();
        public void Clear()
        {
            StaticDatas.Clear();
            HotfixDatas.Clear();
            m_flags.Clear();
        }

        public void AddData(string assetPath, string bundleName, bool isStatic)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                //检索文件
                var files = Directory.GetFiles(assetPath);
                foreach (var file in files)
                {
                    if (file.EndsWith(".meta"))
                        continue;
                    AddData(Path.GetFileNameWithoutExtension(file), bundleName, file, isStatic);
                }
                //检索文件夹
                var paths = Directory.GetDirectories(assetPath);
                foreach (var path in paths)
                {
                    AddData(path, bundleName, isStatic);
                }
            }
            else
            {
                AddData(Path.GetFileNameWithoutExtension(assetPath), bundleName, assetPath, isStatic);
            }
        }
        public void AddData(string assetName, string bundleName, string assetPath, bool isStatic)
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
            m_flags.Add(assetName, data);
            if (isStatic)
            {
                StaticDatas.Add(data);
            }
            else
            {
                HotfixDatas.Add(data);
            }
        }
        
        protected static bool m_initedEditor = false;
        protected static Dictionary<string, string> m_AssetToAssetPath = new Dictionary<string, string>();
        public static void InitEditor()
        {
            var config = AssetDatabase.LoadAssetAtPath<XABAssetNameConfig>(AssetPath);
            
            m_AssetToAssetPath.Clear();
            foreach (var data in config.HotfixDatas)
            {
                m_AssetToAssetPath.Add(data.AssetName, data.AssetPath);
            }
            foreach (var data in config.StaticDatas)
            {
                m_AssetToAssetPath.Add(data.AssetName, data.AssetPath);
            }
        }
        public static string GetAssetPath(string assetName)
        {
            if (!m_initedEditor)
            {
                InitEditor();
                m_initedEditor = true;
            }

            if (!m_AssetToAssetPath.ContainsKey(assetName))
            {
                Debug.LogError($"{assetName} 没有处于资源管理中");
                return string.Empty;
            }
            return m_AssetToAssetPath[assetName];
        }
    }
}


#endif
