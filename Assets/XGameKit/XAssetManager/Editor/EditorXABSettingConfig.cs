using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    //包名设置
    public class EditorXABSettingConfig : ScriptableObject
    {
        public const string AssetPath = "Assets/XGameKitSettings/Editor/EditorXABSettingConfig.asset";
            
        [System.Serializable]
        public class Data
        {
            [HorizontalGroup(Width = 0.1f), LabelText("跟包"), LabelWidth(30)]
            public bool isStatic;
            [HorizontalGroup(Width = 0), HideLabel]
            public Object Object;
            [HorizontalGroup(Width = 0), HideLabel]
            public string BundleName;
            [HorizontalGroup(Width = 0.1f), LabelText("分割"), LabelWidth(30)]
            public bool SplitChildren;
        }
        public List<Data> Datas = new List<Data>();

        [Button("更新包名", ButtonSizes.Medium)]
        public void UpdateBundleName()
        {
            ClearBundleName();
            
            var settings = _CollectSettingList(Datas);
            //设置包名
            foreach (var setting in settings)
            {
                _SetBundleName(setting.assetPath, setting.bundleName);
            }

            //设置资源与包名的关系
            var assetNameConfig = XUtilities.GetOrCreateConfig<XABAssetNameConfig>(XABAssetNameConfig.AssetPath);
            assetNameConfig.Clear();
            foreach (var setting in settings)
            {
                assetNameConfig.AddData(setting.assetPath, setting.bundleName, setting.isStatic);
            }
            AssetDatabase.SaveAssets();

            //打印log
            var logger = XDebug.CreateMutiLogger(XABConst.Tag);
            logger.Append($"==包名设置== 总共:{settings.Count}条");
            foreach (var setting in settings)
            {
                _SetBundleName(setting.assetPath, setting.bundleName);
            }
            logger.Log();
            
            
        }

        //清除所有包名
        public void ClearBundleName()
        {
            var bundleNames = AssetDatabase.GetAllAssetBundleNames();
            foreach (var bundleName in bundleNames)
            {
                AssetDatabase.RemoveAssetBundleName(bundleName, true);
            }
        }

        struct SettingData
        {
            public bool isStatic;
            public string assetPath;
            public string bundleName;
        }
        //统计需要设置的列表
        List<SettingData> _CollectSettingList(List<Data> datas)
        {
            var result = new List<SettingData>();
            var flags = new Dictionary<string, SettingData>();
            foreach (var data in datas)
            {
                if (data.Object == null)
                    continue;
                var assetPath = AssetDatabase.GetAssetPath(data.Object);
                var settings = new List<string>();
                if (data.SplitChildren && AssetDatabase.IsValidFolder(assetPath))
                {
                    //检索文件
                    var files = Directory.GetFiles(assetPath);
                    foreach (var file in files)
                    {
                        if (file.EndsWith(".meta"))
                            continue;
                        settings.Add(file);
                    }
                    //检索文件夹
                    var paths = Directory.GetDirectories(assetPath);
                    foreach (var path in paths)
                    {
                        settings.Add(path);
                    }
                }
                else
                {
                    settings.Add(assetPath);
                }
                foreach (var setting in settings)
                {
                    var childAssetPath = setting;
                    if (flags.ContainsKey(childAssetPath))
                    {
                        Debug.LogError($"资源:{childAssetPath} 已经存在于{flags[childAssetPath].bundleName}!!");
                        continue;
                    }
                    var settingData = new SettingData();
                    settingData.isStatic = data.isStatic;
                    settingData.assetPath = childAssetPath;
                    if (string.IsNullOrEmpty(data.BundleName))
                    {
                        settingData.bundleName = Path.GetFileNameWithoutExtension(childAssetPath) + ".assetbundle";
                    }
                    else
                    {
                        settingData.bundleName = data.BundleName+ ".assetbundle";
                    }
                    result.Add(settingData);
                    flags.Add(childAssetPath, settingData);
                }
            }
            return result;
        }

        //设置包名
        void _SetBundleName(string assetPath, string bundleName)
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
                return;
            importer.assetBundleName = bundleName;
            importer.SaveAndReimport();
        }
    }

}
