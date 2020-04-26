using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    
    public class EditorXAssetBundleBuildWindow : EditorWindow
    {
        private BuildTarget m_BuildTarget;
        private void OnGUI()
        {
            m_BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(m_BuildTarget);
            if (GUILayout.Button("打包"))
            {
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

