using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XGameKit.XUI
{
    public class XUITextureManager
    {
        protected XUIManager m_manager;
        protected XUITextureConfig m_config;

        public void Init(XUIManager manager)
        {
            m_manager = manager;
        }
        public Sprite LoadTexture(string name)
        {
            return null;
        }
        public void LoadTextureAsyn(string name, Action<Sprite> OnComplete = null)
        {
            
        }
    }
}

