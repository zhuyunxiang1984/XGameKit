using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;

namespace XGameKit.Core
{
    public static class XDebug
    {
#if UNITY_EDITOR
        public const string CONFIG_PATH = "Assets/XGameKitSettings/Editor/XDebugConfig.asset";
#endif
        //初始化
        public static void Initialize()
        {
            Reset();
#if UNITY_EDITOR
            var config = AssetDatabase.LoadAssetAtPath<XDebugConfig>(CONFIG_PATH);
            if (config == null)
                return;

            var loggers = config.CreateLoggers();
            foreach (var pairs in loggers)
            {
                AddLogger(pairs.Key, pairs.Value);
            }
#endif
        }
        
        public static Type ReflectClassType;
        //填充静态类
        public static void SetLogClass(Type classType = null)
        {
            ReflectClassType = classType;

            if (ReflectClassType == null)
                return;
            var fields = ReflectClassType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            for (int i = 0; i < fields.Length; ++i)
            {
                var field = fields[i];

                if (_Loggers.ContainsKey(field.Name))
                {
                    field.SetValue(null, _Loggers[field.Name]);
                }
                else
                {
                    field.SetValue(null, new XDebugLogger()
                    {
                        mute = false,
                        prefix = field.Name,
                        color = string.Empty,
                    });
                }
            }
        }

#if !UNITY_EDITOR
        private const string ConditionalSymbol = "XDEBUG";
#endif
        //保留logger名
        private const string DefaultLogName = "Default";
        
        private static XDebugLogger _DefaultLogger = new XDebugLogger()
        {
            mute = false,
            prefix = string.Empty,
            color = string.Empty,
        };
        private static Dictionary<string, XDebugLogger> _Loggers = new Dictionary<string, XDebugLogger>();

#if UNITY_EDITOR
        public static void Reset()
        {
            _DefaultLogger = new XDebugLogger()
            {
                mute = false,
                prefix = string.Empty,
                color = string.Empty,
            };
            _Loggers.Clear();
        }
        public static void AddLogger(string tag, XDebugLogger logger)
        {
            if (tag == DefaultLogName)
            {
                _DefaultLogger = logger;
                return;
            }
            if (_Loggers.ContainsKey(tag))
                return;
            _Loggers.Add(tag, logger);
        }
#endif

#if !UNITY_EDITOR
        [Conditional(ConditionalSymbol)]
#endif
        public static void Log(string message)
        {
            _DefaultLogger.prefix = string.Empty;
            _DefaultLogger.Log(message);
        }
#if !UNITY_EDITOR
        [Conditional(ConditionalSymbol)]
#endif
        public static void LogError(string message)
        {
            _DefaultLogger.prefix = string.Empty;
            _DefaultLogger.LogError(message);
        }
#if !UNITY_EDITOR
        [Conditional(ConditionalSymbol)]
#endif
        public static void LogWarning(string message)
        {
            _DefaultLogger.prefix = string.Empty;
            _DefaultLogger.LogWarning(message);
        }
        
        //创建一个支持多行的log
        public static XDebugMutiLogger CreateMutiLogger(string tag)
        {
            return new XDebugMutiLogger(_GetLogger(tag));
        }
        
        static XDebugLogger _GetLogger(string tag)
        {
            if (_Loggers.ContainsKey(tag))
                return _Loggers[tag];
            _DefaultLogger.prefix = tag;
            return _DefaultLogger;
        }
        
#if !UNITY_EDITOR
        [Conditional(ConditionalSymbol)]
#endif
        public static void Log(string tag, string message)
        {
            _GetLogger(tag).Log(message);
        }
#if !UNITY_EDITOR
        [Conditional(ConditionalSymbol)]
#endif
        public static void LogError(string tag, string message)
        {
            _GetLogger(tag).LogError(message);
        }
#if !UNITY_EDITOR
        [Conditional(ConditionalSymbol)]
#endif
        public static void LogWarning(string tag, string message)
        {
            _GetLogger(tag).LogWarning(message);
        }
        
    }
}
