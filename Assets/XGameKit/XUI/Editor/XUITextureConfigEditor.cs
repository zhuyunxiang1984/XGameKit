using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUITextureConfigEditor : ScriptableObject
    {
        public const string configPathEditor = "Assets/XGameKitSettings/Editor/XUITextureConfigEditor.asset";
        
        #region 工具菜单

        [MenuItem("XGameKit/XUITextureManager/创建XUITextureConfigEditor", priority = 0)]
        static void CreateUISpriteConfigAsset()
        {
            var config = AssetDatabase.LoadAssetAtPath<XUITextureConfigEditor>(configPathEditor);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<XUITextureConfigEditor>();
                AssetDatabase.CreateAsset(config, configPathEditor);
            }
        }
        
        #endregion
        
        [System.Serializable]
        public class AtlasConf
        {
            [HorizontalGroup("1", Width = 0.2f), LabelText("名称"), LabelWidth(30)]
            public string Name;
            [HorizontalGroup("1"), HideLabel]
            public SpriteAtlas atlas;
            [HorizontalGroup("1", Width = 0.4f), HideLabel]
            public List<Object> Folders;
        }
        
        [System.Serializable]
        public class RawxxConf
        {
            [HorizontalGroup(Width = 0.2f), LabelText("语言"), LabelWidth(30)] 
            public string Language;
            [HorizontalGroup, HideLabel]
            public List<Object> Folders;
        }

        [LabelText("图集路径")]
        public Object AtlasFolder;
        [LabelText("图集路径(build-in)")]
        public Object AtlasFolderBuildIn;
        [LabelText("图集配置")]
        public List<AtlasConf> AtlasConfsAB = new List<AtlasConf>();
        [LabelText("图集配置(build-in)")]
        public List<AtlasConf> AtlasConfsBI = new List<AtlasConf>();
        [LabelText("图元配置")]
        public List<RawxxConf> RawxxConfs = new List<RawxxConf>();
        
        
        [Button("生成", ButtonSizes.Medium)]
        void GenerateConfig()
        {
            _GenerateConfigInternal();
        }

        void _GenerateConfigInternal()
        {
            var atlasFolderPath = AssetDatabase.GetAssetPath(AtlasFolder);
            var atlasFolderPathBuildIn = AssetDatabase.GetAssetPath(AtlasFolderBuildIn);
            //Debug.Log($"{atlasFolderPath} {atlasFolderPathBuildIn}");
            if (!AssetDatabase.IsValidFolder(atlasFolderPath))
            {
                EditorUtility.DisplayDialog(string.Empty, $"请正确设置图集文件夹！", "OK");
                return;
            }
            if (!AssetDatabase.IsValidFolder(atlasFolderPathBuildIn))
            {
                EditorUtility.DisplayDialog(string.Empty, $"请正确设置图集文件夹(build-in)！", "OK");
                return;
            }

            if (!_CheckAtlasName())
                return;
            //生成Atlas
            _GenerateSpriteAtlas(_GetAtlasFolders(AtlasConfsAB), atlasFolderPath, false);
            _GenerateSpriteAtlas(_GetAtlasFolders(AtlasConfsBI), atlasFolderPathBuildIn, true);
            
            //统计名字
            _GenerateTextureConfig();
            
            //
            XUITextureManager.InitEditor();
        }
        
        //重名检测
        bool _CheckAtlasName()
        {
            Dictionary<string, bool> flags = new Dictionary<string, bool>();
            foreach (var temp in AtlasConfsAB)
            {
                if (flags.ContainsKey(temp.Name))
                {
                    XDebug.LogError($"图集命名重复 {name}");
                    return false;
                }
                flags.Add(temp.Name, true);
            }
            foreach (var temp in AtlasConfsBI)
            {
                if (flags.ContainsKey(temp.Name))
                {
                    XDebug.LogError($"图集命名重复 {name}");
                    return false;
                }
                flags.Add(temp.Name, true);
            }
            return true;
        }
        
        //--
        Dictionary<string, AtlasConf> _GetAtlasFolders(List<AtlasConf> list)
        {
            //统计需要生成的图集
            Dictionary<string, AtlasConf> result = new Dictionary<string, AtlasConf>();
            foreach (var atlasConf in list)
            {
                var folderList = new List<Object>();
                foreach (var folder in atlasConf.Folders)
                {
                    if (folder == null)
                        continue;
                    var path = AssetDatabase.GetAssetPath(folder);
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        EditorUtility.DisplayDialog(string.Empty, $"{atlasConf.Name} path:{path} 不是文件夹", "OK");
                        continue;
                    }
                    folderList.Add(folder);
                }
                result.Add(atlasConf.Name, atlasConf);
            }
            return result;
        }
        
        //创建atlas
        void _GenerateSpriteAtlas(Dictionary<string, AtlasConf> dictFolders, string output, bool buildin)
        {
            Dictionary<string, SpriteAtlas> dictSpriteAtlas = new Dictionary<string, SpriteAtlas>();
            var files = Directory.GetFiles(output, "*.spriteatlas", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var assetPath = file.Replace(Application.dataPath, string.Empty);
                //Debug.Log($"{assetPath}");
                var asset = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
                if (asset == null)
                {
                    Debug.LogError($"{assetPath}不存在");
                    continue;
                }
                var atlasName = Path.GetFileNameWithoutExtension(assetPath);
                if (!dictFolders.ContainsKey(atlasName))
                {
                    XDebug.Log(XUIConst.Tag,$"{atlasName} 目前是多余的 assetPath:{assetPath}");
                    continue;
                }
                dictSpriteAtlas.Add(atlasName, asset);
            }
            //更新或创建图集
            foreach (var pairs in dictFolders)
            {
                var atlasName = pairs.Key;
                if (string.IsNullOrEmpty(atlasName))
                    continue;
                SpriteAtlas atlasAsset = null;
                if (dictSpriteAtlas.ContainsKey(atlasName))
                {
                    atlasAsset = dictSpriteAtlas[atlasName];
                }
                else
                {
                    atlasAsset = new SpriteAtlas();
                    SpriteAtlasPackingSettings spriteAtlasPackingSettings = new SpriteAtlasPackingSettings()
                    {
                        blockOffset = 1,
                        enableRotation = false,
                        enableTightPacking = false,
                        padding = 2,
                    };
                    atlasAsset.SetPackingSettings(spriteAtlasPackingSettings);
                    var savepath = _MakeAtlasPath(output, atlasName);
                    XUtilities.MakePathExist(savepath);
                    AssetDatabase.CreateAsset(atlasAsset, savepath);
                }
                atlasAsset.SetIncludeInBuild(buildin);
                //添加文件夹
                atlasAsset.Remove(atlasAsset.GetPackables());
                atlasAsset.Add(pairs.Value.Folders.ToArray());
                pairs.Value.atlas = atlasAsset;
                AssetDatabase.SaveAssets();
            }
        }
        
        //统计sprite，生成运行时配置
        void _GenerateTextureConfig()
        {
            var configAB = _CreateTextureConfigAB();
            var configBI = _CreateTextureConfigBI();

            configAB.Clear();
            configBI.Clear();
            foreach (var atlasInfo in AtlasConfsAB)
            {
                foreach (var folder in atlasInfo.Folders)
                {
                    if (folder == null)
                        continue;
                    var path = AssetDatabase.GetAssetPath(folder);
                    if (!AssetDatabase.IsValidFolder(path))
                        continue;
                    var files = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        var assetPath = AssetDatabase.GetAssetPath(atlasInfo.atlas);
                        configAB.AddData(name, assetPath, 
                            atlasInfo.Name,  assetPath, false);
                    }
                }
            }
            foreach (var atlasInfo in AtlasConfsBI)
            {
                foreach (var folder in atlasInfo.Folders)
                {
                    if (folder == null)
                        continue;
                    var path = AssetDatabase.GetAssetPath(folder);
                    if (!AssetDatabase.IsValidFolder(path))
                        continue;
                    var files = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        var assetPath = AssetDatabase.GetAssetPath(atlasInfo.atlas);
                        configBI.AddData(name, _GetResourcePath(assetPath), 
                            atlasInfo.Name, assetPath,true);
                    }
                }
            }
            foreach (var rawFolder in RawxxConfs)
            {
                foreach (var folder in rawFolder.Folders)
                {
                    var path = AssetDatabase.GetAssetPath(folder);
                    if (!AssetDatabase.IsValidFolder(path))
                        continue;
                    bool buildIn = path.IndexOf("Resources/") != -1;
                    var files = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        //file = file.Replace("\\", "/");
                        var name = Path.GetFileNameWithoutExtension(file);
                        if (buildIn)
                        {
                            configBI.AddData(name, _GetResourcePath(file),
                                null, _GetAssetPath(file),true, rawFolder.Language);
                        }
                        else
                        {
                            configAB.AddData(name, _GetAssetPath(file),
                                null, _GetAssetPath(file),false, rawFolder.Language);
                        }
                    }
                }
            }
            EditorUtility.SetDirty(configAB);
            EditorUtility.SetDirty(configBI);
            AssetDatabase.SaveAssets();
        }

        XUITextureConfig _CreateTextureConfigAB()
        {
            var config = AssetDatabase.LoadAssetAtPath<XUITextureConfig>(XUITextureConfig.configPathAB);
            if (config != null)
                return config;
            config = ScriptableObject.CreateInstance<XUITextureConfig>();
            XUtilities.MakePathExist(XUITextureConfig.configPathAB);
            AssetDatabase.CreateAsset(config, XUITextureConfig.configPathAB);
            return config;
        }
        XUITextureConfig _CreateTextureConfigBI()
        {
            var config = AssetDatabase.LoadAssetAtPath<XUITextureConfig>(XUITextureConfig.configPathBI);
            if (config != null)
                return config;
            config = ScriptableObject.CreateInstance<XUITextureConfig>();
            XUtilities.MakePathExist(XUITextureConfig.configPathBI);
            AssetDatabase.CreateAsset(config, XUITextureConfig.configPathBI);
            return config;
        }
        
        string _MakeAtlasPath(string root, string atlasName)
        {
            root = root.Replace("\\", "/");
            return $"{root}/{atlasName}.spriteatlas";
        }

        string _GetAssetPath(string path)
        {
            path = path.Replace("\\", "/");
            int index = path.IndexOf("Assets/");
            if (index == -1)
            {
                Debug.LogError($"{path}不是Asset目录");
                return string.Empty;
            }
            return path.Substring(index).Replace("\\", "/");
        }

        string _GetResourcePath(string path)
        {
            path = path.Replace("\\", "/");
            //resources下的路径,从resources开始，不包括扩展名
            int start = path.IndexOf("Resources/") + 10;
            if (start == -1)
            {
                Debug.LogError($"{path}不是Resources目录");
                return string.Empty;
            }

            int end = path.LastIndexOf(".");
            if (end == -1)
                return path.Substring(start);
            return path.Substring(start, end - start);
        }
    }
}