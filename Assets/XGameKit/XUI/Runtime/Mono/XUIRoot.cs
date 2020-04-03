using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XUI
{
    public class XUIRoot : MonoBehaviour
    {
        public static XUIRoot instance { get; private set; }

        public static XUIRoot CreateInstance(string path)
        {
            var go = GameObject.Find("XUIRoot");
            if (go != null)
            {
                instance = go.GetComponent<XUIRoot>();
                return null;
            }
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("场景中没有XUIManagerRoot!!! ");
                return null;
            }
            var pfb = Resources.Load(path);
            if (pfb == null)
            {
                Debug.LogErrorFormat("资源不存在!!! {0}", path);
                return null;
            }
            go = Instantiate(pfb, Vector3.zero, Quaternion.identity) as GameObject;
            DontDestroyOnLoad(go);
            instance = go.GetComponent<XUIRoot>();
            return instance;
        }

        public Camera uiCamera;
        public XUICanvasManager uiCanvasManager;
        public Transform uiUnusedNode;

        //遮罩
        public GameObject uiMask;
        //闪屏
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarningFormat("{0} 只允许存在一个", ToString());
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        private void OnEnable()
        {
            HideMask();
        }

        public void ShowMask(Transform node)
        {
            uiMask.transform.SetParent(node, false);
            uiMask.transform.SetSiblingIndex(0);
        }
        public void HideMask()
        {
            uiMask.transform.SetParent(uiUnusedNode, false);
        }

        //启用/禁用UI绘制
        public void EnableUICamera(bool on)
        {
            if (uiCamera == null)
                return;
            uiCamera.enabled = on;
        }

        //UGUI坐标转换到屏幕坐标
        public Vector2 UGUI2ScreenPoint(RectTransform trans)
        {
            var output = RectTransformUtility.WorldToScreenPoint(uiCamera, trans.position);
            //Debug.Log(trans.position + " -> " + output);
            return output;
        }
        //屏幕坐标转换到UGUI坐标
        public Vector2 ScreenPoint2UGUI(RectTransform trans, Vector2 screenPoint)
        {
            var output = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(trans, screenPoint, uiCamera, out output);
            return output;
        }
        //UGUI坐标转换到另一个UGUI控件内局部坐标
        public Vector2 UGUI2UGUI(RectTransform trans1, RectTransform trans2)
        {
            var screenPoint = UGUI2ScreenPoint(trans1);
            return ScreenPoint2UGUI(trans2, screenPoint);
        }
        
        //检测屏幕坐标是否在指定UGUI控件内
        public bool RectangleContainsScreenPoint(RectTransform trans, Vector2 screenPoint)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(trans, screenPoint, uiCamera);
        }
    }
}