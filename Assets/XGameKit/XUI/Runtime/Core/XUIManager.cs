﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIManager : IXService
    {
        public static T AllocParam<T>() where T: class, IXPoolable, new()
        {
            return XObjectPool.Alloc<T>();
        }
        
        //
        public XUIRoot uiRoot { get; protected set; }
        //msgmanager
        public XMsgManager MsgManager { get; protected set; } = new XMsgManager();
        //evtmanager
        public XEventManager EventManager { get; protected set; } = new XEventManager();
        //assetloader
        public IXUIAssetLoader AssetLoader { get; protected set; } = new XUIAssetLoaderDefault();
        public IXUILocalizationLoader LocalizationLoader { get; protected set; }=new XUILocalizationLoaderDefault();

        public XUITextureManager TextureManager { get; protected set; } = new XUITextureManager();

        //windowlist
        protected Dictionary<string, XUIWindow> m_dictWindows = new Dictionary<string, XUIWindow>();
        protected List<XUIWindow> m_listWindows = new List<XUIWindow>();
        protected List<string> m_listDestroy = new List<string>();
        
        //排序的窗口列表（层级，打开时间）
        protected List<XUIWindow> m_ltSort = new List<XUIWindow>();
        protected bool m_ltSortChanged;
        //当前有遮罩的窗口
        protected XUIWindow m_maskWindow;
        
        public XUIManager()
        {
            uiRoot = XUIRoot.CreateInstance("XUIRoot");
            TextureManager.Init(AssetLoader, LocalizationLoader);
        }
        public void Dispose()
        {
            foreach (var window in m_listWindows)
            {
                window.Term();
            }
            m_listWindows.Clear();
            m_dictWindows.Clear();
            m_listDestroy.Clear();
            m_ltSort.Clear();
        }

        public void Tick(float step)
        {
            foreach (var window in m_listWindows)
            {
                window.Tick(step);
                if (window.CurState == XUIWindow.EnumState.Remove)
                {
                    m_listDestroy.Add(window.name);
                }
            }
            //处理销毁的窗口
            if (m_listDestroy.Count > 0)
            {
                foreach (var name in m_listDestroy)
                {
                    _DestroyWindow(name);
                }
                m_listDestroy.Clear();
            }
            if (m_ltSortChanged)
            {
                //处理mask位置
                _UpdateSortAndMask();
                m_ltSortChanged = false;
            }
        }
        
        public bool IsShow(string name)
        {
            if (!m_dictWindows.ContainsKey(name))
                return false;
            return m_dictWindows[name].CurState == XUIWindow.EnumState.Show;
        }
        public void ShowWindow(string name, object param = null)
        {
            XDebug.Log(XUIConst.Tag,$"ShowWindow {name}");
            XUIWindow window = null;
            if (m_dictWindows.ContainsKey(name))
            {
                window = m_dictWindows[name];
            }
            else
            {
                var paramBundle = new XUIParamBundle();
                paramBundle.AssetLoader = AssetLoader;
                paramBundle.LocalizationLoader = LocalizationLoader;
                paramBundle.EventManager = EventManager;
                paramBundle.MsgManager = MsgManager;
                paramBundle.uiRoot = uiRoot;
                paramBundle.TextureManager = TextureManager;
                
                window = XObjectPool.Alloc<XUIWindow>();
                window.Init(this, paramBundle, name, param);
                m_listWindows.Add(window);
                m_dictWindows.Add(name, window);
            }
            window.DstState = XUIWindow.EnumState.Show;
            window.openTick = Time.realtimeSinceStartup;
        }

        public void HideWindow(string name)
        {
            XDebug.Log(XUIConst.Tag,$"HideWindow {name}");
            if (!m_dictWindows.ContainsKey(name))
                return;
            XUIWindow window = m_dictWindows[name];
            window.DstState = XUIWindow.EnumState.Hide;
        }

        //销毁窗口对象
        void _DestroyWindow(string name)
        {
            if (!m_dictWindows.ContainsKey(name))
                return;
            XUIWindow window = m_dictWindows[name];
            window.Term();
            m_listWindows.Remove(window);
            m_dictWindows.Remove(name);
        }

        //设置canvas的排序和mask位置
        void _UpdateSortAndMask()
        {
            int max = m_ltSort.Count;

            XUIWindow topMask = null;
            for (int i = 0; i < max; ++i)
            {
                var window = m_ltSort[i];
                window.canvas.sortingOrder = i * XUIConst.LayerPaddingInner;

                if (window.mono.mask)
                {
                    topMask = window;
                }
            }
            if (topMask == null)
            {
                uiRoot.HideMask();
                m_maskWindow = null;
                return;
            }
            if (topMask == m_maskWindow)
                return;
            uiRoot.ShowMask(topMask.canvas.transform);
            m_maskWindow = topMask;
            Debug.Log($"mask is under {m_maskWindow.name}");
        }

        public int GetSort(XUIWindow window)
        {
            int index = -1;
            for (int i = 0; i < m_ltSort.Count; ++i)
            {
                var temp = m_ltSort[i];
                if (temp == window)
                {
                    index = i;
                    break;
                }
                //层级越低排在越前面（越早绘制）
                if (window.layer < temp.layer)
                {
                    index = i;
                    break;
                }
                //层级相同，打开时间越小排在越前面（越早绘制）
                if (window.layer == temp.layer && window.openTick < temp.openTick)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                index = m_ltSort.Count;
            XDebug.Log(XUIConst.Tag,$"GetSort {window.name} {index}");
            return index;
        }
        public void AddSort(XUIWindow window, int index)
        {
            if (index != -1)
            {
                m_ltSort.Insert(index, window);
            }
            else
            {
                m_ltSort.Add(window);
            }
            m_ltSortChanged = true;
            Debug.Log("addslot" + m_ltSort.Count);
        }
        public void DelSort(XUIWindow window)
        {
            m_ltSort.Remove(window);
            m_ltSortChanged = true;
            Debug.Log("delslot" + m_ltSort.Count);
        }
    }
}
