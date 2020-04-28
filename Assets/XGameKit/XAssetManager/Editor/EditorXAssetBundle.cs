using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XAssetManager;

namespace XGameKit.XAssetManager
{
    public class EditorXAssetBundle
    {
        #region 工具菜单
        
        [MenuItem("XGameKit/XAssetManager/开发配置")]
        static void RuntimeSetting()
        {
            EditorWindow window = EditorWindow.GetWindow<EditorXAssetBundleEditSettingWindow>("开发配置");
            window.Show();
        }
        
        [MenuItem("XGameKit/XAssetManager/资源打包")]
        static void BuildAssetBundle()
        {
            EditorWindow window = EditorWindow.GetWindow<EditorXAssetBundleBuildWindow>("资源打包");
            window.Show();
        }
        
        #endregion

        #region 编辑函数

        public static BuildTarget GetBuildTarget(EnumPlatform platform)
        {
            var buildTarget = BuildTarget.NoTarget;
            switch (platform)
            {
                case EnumPlatform.Android:
                    buildTarget = BuildTarget.Android;
                    break;
                case EnumPlatform.Windows:
                    buildTarget = BuildTarget.StandaloneWindows64;
                    break;
                case EnumPlatform.iOS:
                    buildTarget = BuildTarget.iOS;
                    break;
            }
            return buildTarget;
        }

        //输出路径
        public static string GetOutput(EnumPlatform platform)
        {
            return $"{Application.dataPath}/../AssetBundles/{platform.ToString()}";
        }
        
        public static string GetOutputBuild(EnumPlatform platform)
        {
            return $"{GetOutput(platform)}/Build";
        }

        public static string GetOutputStatic(EnumPlatform platform)
        {
            return $"{GetOutput(platform)}/Static";
        }
        public static string GetOutputHotfix(EnumPlatform platform)
        {
            return $"{GetOutput(platform)}/Hotfix";
        }
        
        
        #endregion
    }
}