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
        protected class CanvasData : IXPoolable
        {
            public Canvas canvas;
            public int layer;
            public long openTick;

            public void Reset()
            {
                canvas = null;
                layer = 0;
                openTick = 0;
            }
        }
        protected List<CanvasData> m_datas = new List<CanvasData>();
        
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

        public Canvas AppendCanvas(int layer, long openTick)
        {
            int index = -1;
            for (int i = 0; i < m_datas.Count; ++i)
            {
                //层级越低排在越前面（越早绘制）
                if (m_datas[i].layer > layer)
                {
                    index = i;
                    break;
                }
                //层级相同，打开时间越大排在越前面（越早绘制）
                if (m_datas[i].layer == layer && m_datas[i].openTick > openTick)
                {
                    index = i;
                    break;
                }
            }
            var canvas = _GetOrCreateCanvas();
            canvas.sortingOrder = layer;
            canvas.gameObject.SetActive(true);
            
            var data = XObjectPool.Alloc<CanvasData>();
            data.canvas = canvas;
            data.layer = layer;
            if (index == -1)
            {
                canvas.transform.SetSiblingIndex(m_datas.Count);
                m_datas.Add(data);
            }
            else
            {
                canvas.transform.SetSiblingIndex(index);
                m_datas.Insert(index, data);
            }
            return canvas;
        }

        public void RemoveCanvas(Canvas canvas)
        {
            int index = -1;
            for (int i = 0; i < m_datas.Count; ++i)
            {
                if (m_datas[i].canvas == canvas)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                m_datas.RemoveAt(index);
            }
            canvas.gameObject.SetActive(false);
            canvas.transform.SetAsLastSibling();
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
            foreach (var data in m_datas)
            {
                data.canvas.name = $"{prefab.name}_{data.layer}";
            }

            int index = 0;
            foreach (var go in m_caches)
            {
                go.name = $"{prefab.name}_cache{index++}";
            }
        }
    }

}