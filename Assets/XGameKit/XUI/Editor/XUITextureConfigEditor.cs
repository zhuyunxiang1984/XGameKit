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
            [HorizontalGroup("1"), LabelText("build-in"), LabelWidth(50)]
            public bool BuildIn;
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
        public List<AtlasConf> AtlasConfs = new List<AtlasConf>();
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

            //生成Atlas
            _GenerateSpriteAtlas(_GetAtlasFolders(false), atlasFolderPath, false);
            _GenerateSpriteAtlas(_GetAtlasFolders(true), atlasFolderPathBuildIn, true);
            
            //统计名字
            _CollectSprite(atlasFolderPath);
        }
        
        //--
        Dictionary<string, AtlasConf> _GetAtlasFolders(bool buildIn)
        {
            //统计需要生成的图集
            Dictionary<string, AtlasConf> result = new Dictionary<string, AtlasConf>();
            foreach (var atlasConf in AtlasConfs)
            {
                if (atlasConf.BuildIn != buildIn)
                    continue;
                if (result.ContainsKey(atlasConf.Name))
                {
                    Debug.LogError($"图集名称重复 {atlasConf.Name}");
                    continue;
                }

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
        void _CollectSprite(string atlasFolderPath)
        {
            var config = AssetDatabase.LoadAssetAtPath<XUITextureConfig>(XUITextureConfig.configPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<XUITextureConfig>();
                AssetDatabase.CreateAsset(config, XUITextureConfig.configPath);
            }

            config.Clear();
            foreach (var atlasInfo in AtlasConfs)
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
                        config.AddData(name, _GetPath(assetPath, atlasInfo.BuildIn), 
                            atlasInfo.Name, 
                            _GetPath(file, false),
                            atlasInfo.BuildIn);
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
                        var name = Path.GetFileNameWithoutExtension(file);
                        config.AddData(name, _GetPath(file, buildIn),
                            null, 
                            _GetPath(file,false),
                            buildIn, rawFolder.Language);
                    }
                }
            }

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        string _MakeAtlasPath(string root, string atlasName)
        {
            return $"{root}/{atlasName}.spriteatlas";
        }

        string _GetPath(string assetPath, bool buildin)
        {
            if (buildin)
            {
                //resources下的路径,从resources开始，不包括扩展名
                int start = assetPath.IndexOf("Resources/") + 10;
                int end = assetPath.LastIndexOf(".");
                return assetPath.Substring(start, end - start);
            }
            return assetPath.Substring(assetPath.IndexOf("Assets/"));
            
        }
    }
}