using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XGameKit.XAssetManager;

namespace XGameKit.XAssetManager
{
    public class EditorXAssetBundle
    {
        public const string configPathEditor = "Assets/XGameKitSettings/Editor/EditorXAssetBundleSettingSO.asset";
        
        #region 工具菜单

        [MenuItem("XGameKit/XAssetManager/创建EditorXAssetBundleSettingSO")]
        static void CreateUISpriteConfigAsset()
        {
            var config = AssetDatabase.LoadAssetAtPath<EditorXAssetBundleSettingSO>(configPathEditor);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<EditorXAssetBundleSettingSO>();
                AssetDatabase.CreateAsset(config, configPathEditor);
            }
            Selection.activeObject = config;
        }
        
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
            return $"{Application.dataPath}/../AssetBundles/{target.ToString()}";
        }

        #endregion
    }
}