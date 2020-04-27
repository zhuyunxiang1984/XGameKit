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

        //输出路径
        public static string GetOutput(BuildTarget target)
        {
            return $"{Application.dataPath}/../AssetBundles/{target.ToString()}";
        }
        
        public static string GetOutputBuild(BuildTarget target)
        {
            return $"{GetOutput(target)}/Build";
        }

        public static string GetOutputStatic(BuildTarget target)
        {
            return $"{GetOutput(target)}/Static";
        }
        public static string GetOutputHotfix(BuildTarget target)
        {
            return $"{GetOutput(target)}/Hotfix";
        }
        
        
        #endregion
    }
}