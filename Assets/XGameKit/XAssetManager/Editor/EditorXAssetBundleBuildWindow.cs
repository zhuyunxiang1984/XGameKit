using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    
    public class EditorXAssetBundleBuildWindow : EditorWindow
    {
        private BuildTarget m_BuildTarget = BuildTarget.NoTarget;
        private void OnGUI()
        {
            m_BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(m_BuildTarget);

            if (GUILayout.Button("设置包名"))
            {
                var config = EditorXAssetBundle.GetOrCreateConfig<EditorXABSettingConfig>(EditorXABSettingConfig.AssetPath);
                Selection.activeObject = config;
            }
            if (GUILayout.Button("打包"))
            {
                if (m_BuildTarget == BuildTarget.NoTarget)
                {
                    return;
                }
                var output = EditorXAssetBundle.GetOutput(m_BuildTarget);
                XUtilities.MakePathExist(output);
                var manifest = BuildPipeline.BuildAssetBundles(output, BuildAssetBundleOptions.ChunkBasedCompression, m_BuildTarget);
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
                var relationShipStaticConfig = EditorXAssetBundle.GetOrCreateConfig<XABRelationShipConfig>(XABRelationShipConfig.AssetPathStatic);
                var relationShipHotfixConfig = EditorXAssetBundle.GetOrCreateConfig<XABRelationShipConfig>(XABRelationShipConfig.AssetPathHotfix);
                
                var dictStaticBundleNames = relationShipStaticConfig.GetBundleNames();
                var dictHotfixBundleNames = relationShipHotfixConfig.GetBundleNames();
                foreach (var bundleName in dictStaticBundleNames.Keys)
                {
                    var dependencies = manifest.GetAllDependencies(bundleName);
                    //Debug.Log($"{bundleName} 依赖数量 {dependencies.Length}");
                    foreach (var dependency in dependencies)
                    {
                        //Debug.Log("check " + dependency);
                        if (dictHotfixBundleNames.ContainsKey(dependency))
                        {
                            XDebug.LogError(XABConst.Tag, $"static:{bundleName} 依赖 hotfix:{dependency}");
                            EditorUtility.DisplayDialog("错误", "跟包资源不允许依赖更新资源！！", "OK");
                            return;
                        }
                    }
                }
                
                //打包完成后，将static和hotfix分开目录
                var outputStatic = EditorXAssetBundle.GetOutputStatic(m_BuildTarget);
                foreach (var bundleName in dictStaticBundleNames.Keys)
                {
                    var src = $"{output}/{bundleName}";
                    var dst = $"{outputStatic}/{bundleName}";
                    XUtilities.MakePathExist(dst);
                    File.Copy(src, dst);
                }
                var outputHotfix = EditorXAssetBundle.GetOutputHotfix(m_BuildTarget);
                foreach (var bundleName in dictHotfixBundleNames.Keys)
                {
                    var src = $"{output}/{bundleName}";
                    var dst = $"{outputHotfix}/{bundleName}";
                    XUtilities.MakePathExist(dst);
                    File.Copy(src, dst);
                }
            }

            if (GUILayout.Button("打开目录"))
            {
                var output = EditorXAssetBundle.GetOutput(m_BuildTarget);
                Debug.Log(output);
                output = output.Replace("/","\\");
                System.Diagnostics.Process.Start("explorer.exe", output);
            }
        }
    }
}

