using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XGameKit.XAssetManager
{
    public class EditorXAssetBundleEditSettingWindow : EditorWindow
    {
        
        private void OnGUI()
        {
            //选择模式
            var mode = (EnumEditorRunMode)EditorPrefs.GetInt(XABConst.EditorRunModeKey);
            EditorGUI.BeginChangeCheck();
            mode = (EnumEditorRunMode)EditorGUILayout.EnumPopup("选择模式", mode);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(XABConst.EditorRunModeKey, (int)mode);
            }

            if (mode == EnumEditorRunMode.Local)
            {
                //设置路径
                var path = EditorPrefs.GetString(XABConst.EditorRunPathKey);
                GUILayout.BeginHorizontal();
                GUILayout.TextField(path);
                if (GUILayout.Button("选择路径"))
                {
                    path = EditorUtility.OpenFolderPanel("选择路径", path, string.Empty);
                    Debug.Log(path);
                    EditorPrefs.SetString(XABConst.EditorRunPathKey, path);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
            }

            if (mode == EnumEditorRunMode.Remote)
            {
                //设置网址
                var url = EditorPrefs.GetString(XABConst.EditorRunUrlKey);
                EditorGUI.BeginChangeCheck();
                url = EditorGUILayout.TextField("网址", url);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(XABConst.EditorRunUrlKey, url);
                }
            }

            //选择平台
            var buildTarget = (BuildTarget)EditorPrefs.GetInt(XABConst.EditorRunTargetKey);
            EditorGUI.BeginChangeCheck();
            buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("选择平台",buildTarget);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(XABConst.EditorRunTargetKey, (int)buildTarget);
            }
            
            //设置秘钥
            var enableEncryptKey = EditorPrefs.GetBool(XABConst.EditorRunEnableEncryptKey);
            EditorGUI.BeginChangeCheck();
            enableEncryptKey = EditorGUILayout.Toggle("启用加密",enableEncryptKey);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(XABConst.EditorRunEnableEncryptKey, enableEncryptKey);
            }
            var encryptKey = EditorPrefs.GetString(XABConst.EditorRunEncryptKey);
            EditorGUI.BeginChangeCheck();
            encryptKey = EditorGUILayout.TextField("设置秘钥",encryptKey);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(XABConst.EditorRunEncryptKey, encryptKey);
            }

        }
    }


}
