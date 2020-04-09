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
        public const string configPath = "Assets/XGameKitSettings/Runtime/XUITextureConfig.asset";

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
            //[HideInInspector]
            public string assetPath; //用于开发模式
#endif
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

        protected Dictionary<string, TextureInfo> m_commons = new Dictionary<string, TextureInfo>();

        protected Dictionary<string, Dictionary<string, TextureInfo>> m_complex =
            new Dictionary<string, Dictionary<string, TextureInfo>>();

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

        public static Sprite GetSprite(string name)
        {
            var textureConfig = AssetDatabase.LoadAssetAtPath<XUITextureConfig>(configPath);
            if (textureConfig == null)
                return null;
            textureConfig.InitData();
            var textureInfo = textureConfig.GetData(name);
            if (textureInfo == null)
                return null;
            return AssetDatabase.LoadAssetAtPath<Sprite>(textureInfo.assetPath);
        }
#endif

        public void InitData()
        {
            m_commons.Clear();
            m_complex.Clear();
            foreach (var data in configData)
            {
                var dict = _GetOrCreateDatas(data.language);
                if (dict.ContainsKey(data.name))
                {
                    Debug.LogError($"texture名字重复 name:{data.name} language:{data.language}");
                    continue;
                }

                dict.Add(data.name.ToLower(), data);
            }
        }

        public TextureInfo GetData(string name, string language = Common)
        {
            name = name.ToLower();
            var dict = _GetOrCreateDatas(language);
            if (!dict.ContainsKey(name))
            {
                Debug.LogError($"texture 不存在 name:{name} language:{language}");
                return null;
            }

            return dict[name];
        }

        Dictionary<string, TextureInfo> _GetOrCreateDatas(string language = Common)
        {
            if (language == Common)
                return m_commons;
            if (m_complex.ContainsKey(language))
                return m_complex[language];
            var datas = new Dictionary<string, TextureInfo>();
            m_complex.Add(language, datas);
            return datas;
        }
    }
}