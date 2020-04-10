using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XGameKit.XUI
{
    public class XUITextureConfig : ScriptableObject
    {
        public const string Common = "common";
        public const string configPathAB = "Assets/XGameKitSettings/Runtime/XUITextureConfig.asset";
        public const string configPathBI = "Assets/XGameKitSettings/Runtime/Resources/XUITextureConfig.asset";
        public const string configPathBIRuntime = "XUITextureConfig";

        [Serializable]
        public class TextureInfo
        {
            [HorizontalGroup("1"), LabelText("名称"), LabelWidth(30), DisplayAsString]
            public string name; //图片名
            [HorizontalGroup("1", Width = 0.2f), LabelText("语言"), LabelWidth(30), DisplayAsString]
            public string language; //语言
            [HorizontalGroup("1", Width = 0.2f), LabelText("图集"), LabelWidth(30), DisplayAsString]
            public string atlasName; //是否是图集中资源
            [HorizontalGroup("1", Width = 0.2f), LabelText("build-in"), LabelWidth(50), DisplayAsString]
            public bool buildin; //是否是buildin资源

            [LabelText("路径"), LabelWidth(30), DisplayAsString]
            public string path; //路径
            
#if UNITY_EDITOR
            [HideInInspector]
            public string assetPath; //用于开发模式
#endif
            public string ToLog()
            {
                return $"name:{name} path:{path} buildin:{buildin} atlasName:{atlasName}";
            }
        }

#if UNITY_EDITOR
        
        [LabelText("过滤"), OnValueChanged("Filter")]
        public string filterWord;
        [LabelText("图元信息"), ShowIf("IsFilterWordExist")]
        public List<TextureInfo> filters = new List<TextureInfo>();
        void Filter()
        {
            filters.Clear();
            foreach (var data in configData)
            {
                if (data.name.ToLower().Contains(filterWord.ToLower()))
                {
                    filters.Add(data);
                }
            }
        }
        bool IsFilterWordExist()
        {
            return !string.IsNullOrEmpty(filterWord);
        }
#endif
        
        [LabelText("图元信息"), HideIf("IsFilterWordExist")]
        public List<TextureInfo> configData = new List<TextureInfo>();
        
#if UNITY_EDITOR

        public void Clear()
        {
            configData.Clear();
        }

        public void AddData(string name, string path, string atlasName, string assetPath, bool buildin, string language = "")
        {
            var data = new TextureInfo();
            data.language = string.IsNullOrEmpty(language) ? Common : language;
            data.name = name;
            data.path = path;
            data.atlasName = atlasName;
            data.assetPath = assetPath;
            data.buildin = buildin;
            configData.Add(data);
        }
#endif

    }
}