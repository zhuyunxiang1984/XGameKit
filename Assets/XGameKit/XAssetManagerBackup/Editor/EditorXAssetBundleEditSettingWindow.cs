using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XGameKit.XAssetManager.Backup
{
    public class EditorXAssetBundleEditSettingWindow : EditorWindow
    {
        
        private void OnGUI()
        {
            var mode = _OnGUI_SetMode();

            switch (mode)
            {
                case EnumResMode.Local:
                    _OnGUI_SetLocalResPath();
                    _OnGUI_SetRunPlatform();
                    _OnGUI_SetEncrypt();
                    break;
                case EnumResMode.Remote:
                    _OnGUI_SetLocalResPath();
                    _OnGUI_SetRemoteUrl();
                    _OnGUI_SetRunPlatform();
                    _OnGUI_SetEncrypt();
                    break;
                case EnumResMode.Simulate:
                    break;
                case EnumResMode.Release:
                    break;
            }
        }

        EnumResMode _OnGUI_SetMode()
        {
            //选择模式
            var mode = (EnumResMode)EditorPrefs.GetInt(XABConst.EKResMode, XABConst.EKResModeValue);
            EditorGUI.BeginChangeCheck();
            mode = (EnumResMode)EditorGUILayout.EnumPopup("选择模式", mode);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(XABConst.EKResMode, (int)mode);
            }
            return mode;
        }
        void _OnGUI_SetLocalResPath()
        {
            //本地路径
            var path = EditorPrefs.GetString(XABConst.EKResPath, XABConst.EKResPathValue);
                
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField("本地路径", path);
            if (GUILayout.Button("选择路径", GUILayout.Width(80)))
            {
                var sel = EditorUtility.OpenFolderPanel("选择路径", path, string.Empty);
                if (!string.IsNullOrEmpty(sel))
                {
                    path = sel;
                }
            }
            if (GUILayout.Button("默认路径", GUILayout.Width(80)))
            {
                path = XABConst.EKResPathValue;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(XABConst.EKResPath, path);
            }
        }
        void _OnGUI_SetRemoteUrl()
        {
            //设置网址
            var url = EditorPrefs.GetString(XABConst.EKResUrl, XABConst.EKResUrlValue);
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField("网址", url);
            if (GUILayout.Button("选择地址", GUILayout.Width(80)))
            {
                var sel = EditorUtility.OpenFolderPanel("选择地址", url, string.Empty);
                if (!string.IsNullOrEmpty(sel))
                {
                    url = $"file://{sel}";
                }
            }
            if (GUILayout.Button("默认地址", GUILayout.Width(80)))
            {
                url = XABConst.EKResUrlValue;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(XABConst.EKResUrl, url);
            }

        }
        void _OnGUI_SetRunPlatform()
        {
            //选择平台
            var plarform = (EnumPlatform)EditorPrefs.GetInt(XABConst.EKResRunPlatform, XABConst.EKResRunPlatformValue);
            EditorGUI.BeginChangeCheck();
            plarform = (EnumPlatform)EditorGUILayout.EnumPopup("选择平台",plarform);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(XABConst.EKResRunPlatform, (int)plarform);
            }
        }
        void _OnGUI_SetEncrypt()
        {
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
