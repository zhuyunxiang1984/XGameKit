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
            var mode = (EnumResMode)EditorPrefs.GetInt(XABConst.EKResMode, XABConst.EKResModeDefaultValue);
            EditorGUI.BeginChangeCheck();
            mode = (EnumResMode)EditorGUILayout.EnumPopup("选择模式", mode);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(XABConst.EKResMode, (int)mode);
            }

            if (mode == EnumResMode.Local)
            {
                //设置路径
                var path = EditorPrefs.GetString(XABConst.EKResPath, XABConst.EKResPathDefaultValue);
                GUILayout.BeginHorizontal();
                GUILayout.TextField(path);
                if (GUILayout.Button("选择路径"))
                {
                    path = EditorUtility.OpenFolderPanel("选择路径", path, string.Empty);
                    Debug.Log(path);
                    EditorPrefs.SetString(XABConst.EKResPath, path);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
            }

            if (mode == EnumResMode.Remote)
            {
                //设置网址
                var url = EditorPrefs.GetString(XABConst.EKResUrl);
                EditorGUI.BeginChangeCheck();
                url = EditorGUILayout.TextField("网址", url);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(XABConst.EKResUrl, url);
                }
            }

            //选择平台
            var plarform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResRunPlatform, XABConst.EKResRunPlatformDefaultValue);
            EditorGUI.BeginChangeCheck();
            plarform = (EnumPlatform)EditorGUILayout.EnumPopup("选择平台",plarform);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(XABConst.EKResRunPlatform, (int)plarform);
            }
            
            //设置秘钥
            var enableEncryptKey = EditorPrefs.GetBool(XABConst.EKResEnableEncrypt);
            EditorGUI.BeginChangeCheck();
            enableEncryptKey = EditorGUILayout.Toggle("启用加密",enableEncryptKey);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(XABConst.EKResEnableEncrypt, enableEncryptKey);
            }
            var encryptKey = EditorPrefs.GetString(XABConst.EKResEncryptKey);
            EditorGUI.BeginChangeCheck();
            encryptKey = EditorGUILayout.TextField("设置秘钥",encryptKey);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(XABConst.EKResEncryptKey, encryptKey);
            }

        }
    }


}
