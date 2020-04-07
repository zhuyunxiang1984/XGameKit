using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace XGameKit.XUI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class XUILabel : XUIViewMono
    {
        [LabelText("默认文本")]
        public string defaultLanguageText;
        
        protected TextMeshProUGUI m_labelCache;
        protected TextMeshProUGUI m_label
        {
            get
            {
                if (m_labelCache == null)
                    m_labelCache = GetComponent<TextMeshProUGUI>();
                return m_labelCache;
            }
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(defaultLanguageText))
            {
                SetLanguageText(defaultLanguageText);
            }
        }

        public void SetText(string text)
        {
            m_label.text = text;
        }

        public void SetLanguageText(string text)
        {
            m_label.text = m_paramBundle.LocalizationLoader.GetLanguageText(text);
        }

        public string GetLanguageText(string text)
        {
            return m_paramBundle.LocalizationLoader.GetLanguageText(text);
        }
    }
}

