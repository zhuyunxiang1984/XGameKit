using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public interface IXService
    {
        void Dispose();
        void Tick(float deltaTime);
    }
    public class XService : MonoBehaviour
    {
        private static XService _instance = null;

        private static void _CreateInstance()
        {
            if (_instance != null)
                return;
            var go = new GameObject("XService");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<XService>();
        }
        
        public static T AddService<T>(string name = null) where T : class, IXService, new()
        {
            _CreateInstance();
            return _instance._AddService<T>(name);
        }

        public static void DelService<T>(string name = null) where T : class, IXService
        {
            _CreateInstance();
            _instance._DelService<T>(name);
        }

        public static T GetService<T>() where T : class, IXService
        {
            _CreateInstance();
            return _instance._GetService<T>();
        }

        private Dictionary<string, IXService> _dictServices = new Dictionary<string, IXService>();
        private List<IXService> _listServices = new List<IXService>();
        
        private void Update()
        {
            if (_listServices.Count < 1)
                return;
            foreach (var service in _listServices)
            {
                service.Tick(Time.deltaTime);
            }
        }

        T _AddService<T>(string name) where T : class, IXService, new()
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;
            if (_dictServices.ContainsKey(name))
            {
                Debug.LogError($"Service已经存在 {name}");
                return null;
            }

            var service = new T();
            _dictServices.Add(name, service);
            _listServices.Add(service);
            return service;
        }

        void _DelService<T>(string name) where T : class, IXService
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;
            if (!_dictServices.ContainsKey(name))
                return;
            var service = _dictServices[name];
            service.Dispose();
            _dictServices.Remove(name);
            _listServices.Remove(service);
        }

        T _GetService<T>() where T : class, IXService
        {
            var name = typeof(T).Name;
            if (!_dictServices.ContainsKey(name))
            {
                Debug.LogError($"Service没有注册 {name}");
                return null;
            }
            return _dictServices[name] as T;
        }
    }
}
