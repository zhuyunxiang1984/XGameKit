using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    //序列化字典
    [Serializable]
    public class XSerialDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] 
        List<TKey> m_keys;
        [SerializeField] 
        List<TValue> m_values;

        public Dictionary<TKey, TValue> datas { get; protected set; }
        public XSerialDictionary()
        {
            datas = new Dictionary<TKey, TValue>();
        }
        public void OnBeforeSerialize()
        {
            m_keys = new List<TKey>(datas.Keys);
            m_values = new List<TValue>(datas.Values);
        }

        public void OnAfterDeserialize()
        {
            var count = Math.Min(m_keys.Count, m_values.Count);
            datas = new Dictionary<TKey, TValue>(count);
            for (var i = 0; i < count; ++i)
            {
                datas.Add(m_keys[i], m_values[i]);
            }
        }

        public void Clear()
        {
            datas.Clear();
        }
        public void Add(TKey key, TValue value)
        {
            datas.Add(key, value);
        }
        public bool Remove(TKey key)
        {
            return datas.Remove(key);
        }
        public bool ContainsKey(TKey key)
        {
            return datas.ContainsKey(key);
        }
        public TValue this[TKey key]
        {
            get { return datas[key]; }
            set { datas[key] = value; }
        }
    }
}