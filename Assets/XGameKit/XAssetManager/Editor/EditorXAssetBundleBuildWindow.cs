using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XUI;

namespace XGameKit.XAssetManager
{
    
    public class EditorXAssetBundleBuildWindow : EditorWindow
    {
        private void OnGUI()
        {
            //设置包名
            if (GUILayout.Button("配置资源"))
            {
                var config = XUtilities.GetOrCreateConfig<EditorXABSettingConfig>(EditorXABSettingConfig.AssetPath);
                Selection.activeObject = config;
            }
            if (GUILayout.Button("更新包名"))
            {
                var config = XUtilities.GetOrCreateConfig<EditorXABSettingConfig>(EditorXABSettingConfig.AssetPath);
                config.UpdateBundleName();
            }
            
            var platform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResBuildPlatform, XABConst.EKResBuildPlatformDefaultValue);
            EditorGUI.BeginChangeCheck();
            platform = (EnumPlatform)EditorGUILayout.EnumPopup("选择平台", platform);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(XABConst.EKResBuildPlatform, (int)platform);
            }
            
            
            //设置打包路径
            var buildPath = EditorPrefs.GetString(XABConst.EKResBuildPath, XABConst.EKResBuildPathDefaultValue);
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField("输出路径", buildPath);
            if (GUILayout.Button("选择路径", GUILayout.Width(80)))
            {
                var sel = EditorUtility.OpenFolderPanel("选择路径", buildPath, string.Empty);
                if (!string.IsNullOrEmpty(buildPath))
                {
                    buildPath = sel;
                }
            }
            if (GUILayout.Button("默认路径", GUILayout.Width(80)))
            {
                buildPath = XABConst.EKResBuildPathDefaultValue;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(XABConst.EKResBuildPath, buildPath);
            }
            
            if (GUILayout.Button("打包"))
            {
                BuildAssetBundle(platform);
            }
            if (GUILayout.Button("打开目录"))
            {
                var output = EditorXAssetBundle.GetOutput(platform);
                Debug.Log(output);
                output = output.Replace("/","\\");
                System.Diagnostics.Process.Start("explorer.exe", output);
            }
        }

        //资源打包
        public static void BuildAssetBundle(EnumPlatform platform)
        {
            var buildTarget = EditorXAssetBundle.GetBuildTarget(platform);
            if (buildTarget == BuildTarget.NoTarget)
            {
                return;
            }

            var outputBuild = EditorXAssetBundle.GetOutputBuild(platform);
            XUtilities.MakePathExist(outputBuild);
            var manifest = BuildPipeline.BuildAssetBundles(outputBuild, BuildAssetBundleOptions.ChunkBasedCompression,
                buildTarget);

            var logger = XDebug.CreateMutiLogger(XABConst.Tag);
            logger.Append("===打包完成===");
            foreach (var assetBundle in manifest.GetAllAssetBundles())
            {
                logger.Append(assetBundle);
            }

            logger.Log();

            /*
             * 打包完成后，需要检测一下static包是否依赖了hotfix包，这是不允许的!
             * 最好能在设置包名的时候就检测，但是没有找到简便的方式去检测依赖项，
             * 这里使用打包完成后的manifest做依赖项检测，以后再说
             */
            var assetNameConfig = XUtilities.GetOrCreateConfig<XABAssetNameConfig>(XABAssetNameConfig.AssetPath);

            //--
            //打包完成后，将static和hotfix分开目录
            XABManifest manifestStatic = new XABManifest();
            XABManifest manifestHotfix = new XABManifest();

            var outputStatic = EditorXAssetBundle.GetOutputStatic(platform);
            var outputHotfix = EditorXAssetBundle.GetOutputHotfix(platform);

            List<string> staticBundleNames = new List<string>();
            List<string> hotfixBundleNames = new List<string>();

            foreach (var data in assetNameConfig.StaticDatas)
            {
                manifestStatic.SetAssetNameLink(data.AssetName, data.AssetBundleName);
                if (!staticBundleNames.Contains(data.AssetBundleName))
                    staticBundleNames.Add(data.AssetBundleName);
            }

            foreach (var data in assetNameConfig.HotfixDatas)
            {
                manifestHotfix.SetAssetNameLink(data.AssetName, data.AssetBundleName);
                if (!hotfixBundleNames.Contains(data.AssetBundleName))
                    hotfixBundleNames.Add(data.AssetBundleName);
            }

            //检测依赖关系，不允许跟包资源依赖热更资源
            bool dependencyError = false;
            foreach (var bundleName in staticBundleNames)
            {
                var dependencies = manifest.GetAllDependencies(bundleName);
                //Debug.Log($"{bundleName} 依赖数量 {dependencies.Length}");
                foreach (var dependency in dependencies)
                {
                    //Debug.Log("check " + dependency);
                    if (hotfixBundleNames.Contains(dependency))
                    {
                        XDebug.LogError(XABConst.Tag, $"static:{bundleName} 依赖 hotfix:{dependency}");
                        dependencyError = true;
                    }
                }
            }

            if (dependencyError)
            {
                EditorUtility.DisplayDialog("错误", "跟包资源不允许依赖更新资源！！\n错误依赖请看控制台打印！！", "OK");
                return;
            }

            XUtilities.CleanFolder(outputStatic);
            XUtilities.CleanFolder(outputHotfix);
            XUtilities.MakePathExist(outputStatic);
            XUtilities.MakePathExist(outputHotfix);
            foreach (var bundleName in staticBundleNames)
            {
                var src = $"{outputBuild}/{bundleName}";
                var dst = $"{outputStatic}/{bundleName}";

                File.Copy(src, dst);

                manifestStatic.SetDependency(bundleName, manifest.GetAllDependencies(bundleName));
            }

            foreach (var bundleName in hotfixBundleNames)
            {

                var src = $"{outputBuild}/{bundleName}";
                var dst = $"{outputHotfix}/{bundleName}";

                File.Copy(src, dst);

                manifestHotfix.SetDependency(bundleName, manifest.GetAllDependencies(bundleName));
            }

            //将依赖关系写入文件夹
            var jsonStatic = JsonUtility.ToJson(manifestStatic);
            var pathStatic = $"{outputStatic}/manifest.json";
            File.WriteAllText(pathStatic, jsonStatic);

            var jsonHotfix = JsonUtility.ToJson(manifestHotfix);
            var pathHotfix = $"{outputHotfix}/manifest.json";
            File.WriteAllText(pathHotfix, jsonHotfix);
        }
    }
}

