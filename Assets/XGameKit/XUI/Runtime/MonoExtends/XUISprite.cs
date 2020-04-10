using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using Action = System.Action;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XGameKit.XUI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class XUISprite : XUIViewMono
    {
        protected Image m_imageCache;
        protected Image m_image
        {
            get
            {
                if (m_imageCache == null)
                    m_imageCache = GetComponent<Image>();
                return m_imageCache;
            }
        }
        
        [HorizontalGroup(), LabelText("默认图元")]
        public string defaultTex;
        [HorizontalGroup(Width = 0.1f), HideLabel]
        public bool autoSet;

        private void Start()
        {
            if (autoSet)
            {
                SetTexture(defaultTex);
            }
        }
        public void SetTexture(string name)
        {
            if (m_paramBundle.TextureManager == null)
                return;
            m_paramBundle.TextureManager.LoadSpriteAsyn(name, delegate(Sprite sprite)
            {
                m_image.overrideSprite = sprite;
            });
        }

#if UNITY_EDITOR
        private const string whiteblockName = "whiteblock";
        private const string whiteblockPath = "Assets/XGameKit/XUI/Runtime/Sprites/whiteblock.png";
        private void Update()
        {
            if (Application.isPlaying)
                return;
            if (m_image.sprite != null && m_image.sprite.name != whiteblockName)
            {
                defaultTex = m_image.sprite.name;
                m_image.sprite = null;
                EditorUtility.SetDirty(this);
            }
            if (m_image.sprite == null)
                m_image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(whiteblockPath);

            if (m_image.overrideSprite == null || m_image.overrideSprite.name != defaultTex)
            {
                m_image.overrideSprite = XUITextureManager.GetSpriteEditor(defaultTex);
            }
        }
#endif
    }

}