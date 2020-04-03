using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUICanvasManager : MonoBehaviour
    {
        public GameObject prefab;

        protected Stack<Canvas> m_caches = new Stack<Canvas>();
        protected List<Canvas> m_ltCanvas = new List<Canvas>();
        
        private void Awake()
        {
            prefab.SetActive(false);
            foreach (Transform child in transform)
            {
                if (child.gameObject == prefab)
                    continue;
                child.gameObject.name = prefab.name + m_caches.Count;
                child.gameObject.SetActive(false);
                m_caches.Push(child.GetComponent<Canvas>());
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            _UpdateCanvasName();
        }
#endif

        public Canvas AppendCanvas(int index, int layer)
        {
            var canvas = _GetOrCreateCanvas();
            canvas.gameObject.SetActive(true);
            canvas.transform.SetSiblingIndex(index);
            m_ltCanvas.Add(canvas);
            return canvas;
        }

        public void RemoveCanvas(Canvas canvas)
        {
            canvas.gameObject.SetActive(false);
            canvas.transform.SetAsLastSibling();
            m_ltCanvas.Remove(canvas);
            m_caches.Push(canvas);
        }

        Canvas _GetOrCreateCanvas()
        {
            if (m_caches.Count > 0)
                return m_caches.Pop();
            return Instantiate(prefab, transform).GetComponent<Canvas>();
        }
        
        void _UpdateCanvasName()
        {
            foreach (var canvas in m_ltCanvas)
            {
                canvas.name = $"{prefab.name}_{canvas.sortingOrder}";
            }
            int index = 0;
            foreach (var go in m_caches)
            {
                go.name = $"{prefab.name}_cache{index++}";
            }
        }
    }

}