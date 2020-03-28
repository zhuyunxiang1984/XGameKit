#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace XGameKit.Core
{
    public class XDebugConfig : ScriptableObject
    {
        [Serializable]
        public class LoggerInfo
        {
            [HorizontalGroup("1")]
            [HideLabel]
            public bool active = true;

            [HorizontalGroup("1")] 
            [LabelText("名字"), LabelWidth(40)]
            public string name;

            [HorizontalGroup("1")]
            [LabelText("颜色"), LabelWidth(40)]
            public Color color = Color.white;
        }

        [LabelText("定制logger")] 
        public List<LoggerInfo> listLoggerInfo = new List<LoggerInfo>();

        [Button("应用配置", ButtonSizes.Medium)]
        void Apply()
        {
            XDebug.Initialize();
            XDebug.SetLogClass(XDebug.ReflectClassType);
            EditorUtility.DisplayDialog("提示", "应用成功", "OK");
        }

        public Dictionary<string, XDebugLogger> CreateLoggers()
        {
            var loggers = new Dictionary<string, XDebugLogger>();
            foreach (var info in listLoggerInfo)
            {
                if (loggers.ContainsKey(info.name))
                {
                    Debug.LogError($"重复命名Logger {info.name}");
                    continue;
                }
                loggers.Add(info.name, _CreateLogger(info));
            }
            return loggers;
        }
        
        protected XDebugLogger _CreateLogger(LoggerInfo info)
        {
            var newInst = new XDebugLogger();
            newInst.mute = !info.active;
            newInst.prefix = info.name;
            newInst.color = _ColorToHex(info.color);
            return newInst;
        }

        //颜色值转换16进制
        string _ColorToHex(Color color)
        {
            return string.Format("{0}{1}{2}{3}",
                ((int) (color.r * 255)).ToString("X2"),
                ((int) (color.g * 255)).ToString("X2"),
                ((int) (color.b * 255)).ToString("X2"),
                ((int) (color.a * 255)).ToString("X2"));
        }
    }

}


#endif

