using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XGameKit.Core.Editor
{
    public static class XDebugEditor
    {
        [MenuItem("XGameKit/XDebug/Setting")]
        static void XDebugSetting()
        {
            string assetPath = XDebug.CONFIG_PATH;
            var config = AssetDatabase.LoadAssetAtPath<XDebugConfig>(assetPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<XDebugConfig>();
                XUtilities.EnsurePath(assetPath);
                AssetDatabase.CreateAsset(config, assetPath);
            }
            Selection.activeObject = config;
        }

        
        
    }
    
}