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
        
        [MenuItem("XGameKit/XAssetManager/打包资源")]
        static void BuildAssetBundle()
        {
            EditorWindow window = EditorWindow.GetWindow<EditorXAssetBundleBuildWindow>();
            window.Show();
        }
        
        #endregion

        #region 编辑函数

        //输出路径
        public static string GetOutput(BuildTarget target)
        {
            return $"{Application.dataPath}/../AssetBundles/{target.ToString()}/Build";
        }

        public static string GetOutputStatic(BuildTarget target)
        {
            return $"{Application.dataPath}/../AssetBundles/{target.ToString()}/Static";
        }
        public static string GetOutputHotfix(BuildTarget target)
        {
            return $"{Application.dataPath}/../AssetBundles/{target.ToString()}/Hotfix";
        }
        
        //获取或创建一个配置
        public static T GetOrCreateConfig<T>(string assetPath) where T : ScriptableObject
        {
            var config = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (config != null)
                return config;
            config = ScriptableObject.CreateInstance<T>();
            XUtilities.MakePathExist(assetPath);
            AssetDatabase.CreateAsset(config, assetPath);
            return config;
        }
        #endregion
    }
}