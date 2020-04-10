using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.U2D;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.U2D;

#endif

namespace XGameKit.XUI
{
    public class XUITextureManager
    {
        protected IXUIAssetLoader m_AssetLoader;
        protected IXUILocalizationLoader m_LocalizationLoader;
        
        protected Dictionary<string, Dictionary<string, XUITextureConfig.TextureInfo>> m_datas =
            new Dictionary<string, Dictionary<string, XUITextureConfig.TextureInfo>>();

        public void Init(IXUIAssetLoader assetLoader, IXUILocalizationLoader localizationLoader)
        {
            m_AssetLoader = assetLoader;
            m_LocalizationLoader = localizationLoader;
            
            var configBI = Resources.Load<XUITextureConfig>(XUITextureConfig.configPathBIRuntime);
            AddConfig(configBI.configData);
            
            //临时在这里初始化，应该在assetloader初始化完毕时执行
            var configAB = m_AssetLoader.LoadAsset<XUITextureConfig>(XUITextureConfig.configPathAB);
            AddConfig(configAB.configData);

            SpriteAtlasManager.atlasRequested += _OnAtlasRequested;
        }

        public void Term()
        {
            m_AssetLoader = null;
            m_LocalizationLoader = null;
            
            SpriteAtlasManager.atlasRequested += _OnAtlasRequested;
        }

        void _OnAtlasRequested(string path, Action<SpriteAtlas> callback)
        {
            Debug.Log($"_OnAtlasRequested {path}");
            var sa =  Resources.Load<SpriteAtlas>(path);
            callback(sa);
        }
        public void Clear()
        {
            m_datas.Clear();
        }
        public void AddConfig(List<XUITextureConfig.TextureInfo> configData)
        {
            foreach (var data in configData)
            {
                AddData(data.name, data);
            }
        }
        public void AddData(string name, XUITextureConfig.TextureInfo info)
        {
            name = name.ToLower();
            var dict = _GetOrCreateDatas(info.language);
            if (dict.ContainsKey(name))
            {
                Debug.LogError($"已经存在 {name}");
                return;
            }
            dict.Add(name, info);
        }
        public XUITextureConfig.TextureInfo GetData(string name, string language = null)
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

        Dictionary<string, XUITextureConfig.TextureInfo> _GetOrCreateDatas(string language = null)
        {
            if (string.IsNullOrEmpty(language))
                language = XUITextureConfig.Common;
            if (m_datas.ContainsKey(language))
                return m_datas[language];
            var datas = new Dictionary<string, XUITextureConfig.TextureInfo>();
            m_datas.Add(language, datas);
            return datas;
        }
        
        //加载sprite
        public Sprite LoadSprite(string name)
        {
            var textureInfo = GetData(name);
            if (textureInfo == null)
                return null;
            return LoadSprite(textureInfo);
        }
        
        public void LoadSpriteAsyn(string name, Action<Sprite> OnComplete = null)
        {
            var textureInfo = GetData(name);
            if (textureInfo == null)
            {
                OnComplete?.Invoke(null);
                return;
            }
            LoadSpriteAsyn(textureInfo, OnComplete);
        }
        public Sprite LoadSprite(XUITextureConfig.TextureInfo textureInfo)
        {
            if (textureInfo.buildin)
            {
                return _LoadSpriteBI(textureInfo);
            }
            return _LoadSpriteAB(textureInfo);
        }
        public void LoadSpriteAsyn(XUITextureConfig.TextureInfo textureInfo, Action<Sprite> OnComplete = null)
        {
            if (textureInfo.buildin)
            {
                OnComplete?.Invoke(_LoadSpriteBI(textureInfo));
            }
            else
            {
                _LoadSpriteABAsyn(textureInfo, OnComplete);
            }
        }

        protected Sprite _LoadSpriteBI(XUITextureConfig.TextureInfo textureInfo)
        {
            Sprite sprite = null;
            if (string.IsNullOrEmpty(textureInfo.atlasName))
            {
                sprite = Resources.Load<Sprite>(textureInfo.path);
            }
            else
            {
                var atlas = Resources.Load<SpriteAtlas>(textureInfo.path);
                if (atlas != null)
                {
                    sprite = atlas.GetSprite(textureInfo.name);
                }
            }
            if (sprite == null)
            {
                Debug.LogError($"没有找到sprite {textureInfo.ToLog()}");
                return null;
            }
            return sprite;
        }

        protected Sprite _LoadSpriteAB(XUITextureConfig.TextureInfo textureInfo)
        {
            Sprite sprite = null;
            if (string.IsNullOrEmpty(textureInfo.atlasName))
            {
                sprite = m_AssetLoader.LoadAsset<Sprite>(textureInfo.path);
            }
            else
            {
                var atlas = m_AssetLoader.LoadAsset<SpriteAtlas>(textureInfo.path);
                Debug.Log($"LoadAsset SpriteAtlas {textureInfo.path} {atlas}");
                if (atlas != null)
                {
                    sprite = atlas.GetSprite(textureInfo.name);
                }
            }
            if (sprite == null)
            {
                Debug.LogError($"没有找到sprite {textureInfo.ToLog()}");
                return null;
            }
            return sprite;
        }
        protected void _LoadSpriteABAsyn(XUITextureConfig.TextureInfo textureInfo, Action<Sprite> OnComplete)
        {
            if (string.IsNullOrEmpty(textureInfo.atlasName))
            {
                m_AssetLoader.LoadAssetAsyn<Sprite>(textureInfo.path, sprite =>
                {
                    if (sprite == null)
                    {
                        Debug.LogError($"没有找到sprite {textureInfo.ToLog()}");
                    }
                    OnComplete?.Invoke(sprite);
                });
            }
            else
            {
                m_AssetLoader.LoadAssetAsyn<SpriteAtlas>(textureInfo.path, spriteAtlas =>
                {
                    Debug.Log($"LoadAsset SpriteAtlas {textureInfo.path} {spriteAtlas}");
                    var sprite = spriteAtlas.GetSprite(textureInfo.name);
                    if (sprite == null)
                    {
                        Debug.LogError($"没有找到sprite {textureInfo.ToLog()}");
                    }
                    OnComplete?.Invoke(sprite);
                });
            }
        }
        
#if UNITY_EDITOR
        protected static bool m_initedEditor = false;
        public static XUITextureManager InstEditor { get; protected set; } = new XUITextureManager();
        public static void InitEditor()
        {
            var configAB = AssetDatabase.LoadAssetAtPath<XUITextureConfig>(XUITextureConfig.configPathAB);
            var configBI = AssetDatabase.LoadAssetAtPath<XUITextureConfig>(XUITextureConfig.configPathBI);
            
            InstEditor.Clear();
            InstEditor.AddConfig(configAB.configData);
            InstEditor.AddConfig(configBI.configData);
        }

        public static Sprite GetSpriteEditor(string name, string language = null)
        {
            if (!m_initedEditor)
            {
                InitEditor();
                m_initedEditor = true;
            }
            var textureInfo = InstEditor.GetData(name, language);
            if (textureInfo == null)
                return null;
            if (string.IsNullOrEmpty(textureInfo.atlasName))
            {
                return AssetDatabase.LoadAssetAtPath<Sprite>(textureInfo.assetPath);
            }
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(textureInfo.assetPath);
            if (atlas == null)
            {
                Debug.LogError($"{textureInfo.path} 不存在");
                return null;
            }
            return atlas.GetSprite(textureInfo.name);
        }
#endif
        
    }
}

