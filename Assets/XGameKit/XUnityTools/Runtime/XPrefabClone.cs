using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XUI
{
    public abstract class XPrefabClone<T> : MonoBehaviour where T : Component
    {
        public GameObject prefab;

        protected Stack<T> m_caches = new Stack<T>();
        protected List<T> m_clones = new List<T>();

        private void Awake()
        {
            prefab.SetActive(false);
            foreach (Transform child in transform)
            {
                if (child.gameObject == prefab)
                    continue;
                GameObject go = child.gameObject;
                go.SetActive(false);
                var component = go.GetComponent<T>();
                if (component == null)
                    continue;
                go.name = prefab.name + m_caches.Count;
                m_caches.Push(component);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            _UpdateName();
        }
#endif
        public T AppendClone(int index)
        {
            var clone = _GetOrCreateClone();
            clone.gameObject.SetActive(true);
            clone.transform.SetSiblingIndex(index);
            m_clones.Add(clone);
            return clone;
        }

        public void RemoveClone(T clone)
        {
            var go = clone.gameObject;
            go.SetActive(false);
            go.transform.SetAsLastSibling();
            m_clones.Remove(clone);
            m_caches.Push(clone);
        }

        T _GetOrCreateClone()
        {
            if (m_caches.Count > 0)
                return m_caches.Pop();
            return Instantiate(prefab, transform).GetComponent<T>();
        }
        
        protected virtual void _UpdateName()
        {
            for (int i = 0; i < m_clones.Count; ++i)
            {
                var go = m_clones[i];
                go.name = $"{prefab.name}_{i}";
            }
            int index = 0;
            foreach (var go in m_caches)
            {
                go.name = $"{prefab.name}_cache{index++}";
            }
        }
    }

    
}